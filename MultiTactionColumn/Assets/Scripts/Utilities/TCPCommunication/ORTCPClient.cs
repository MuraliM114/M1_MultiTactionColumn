using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using M1.Utilities;
using UniRx;
using UnityEngine;

public enum eClientState
{
    Connecting,
    Connected,
    Disconnected
}

public enum eClientStartConnection
{
    DontConnect,
    Awake,
    Start
}

public class ORTCPClient : MonoBehaviour
{
    public delegate void TCPServerMessageReceivedEvent(TCPEventParams eventParams);

    public event TCPServerMessageReceivedEvent OnTCPMessageReceived;

    public bool   verbose    = true;
    public string clientName = "";

    private bool  autoConnectOnDisconnect        = true;
    private bool  autoConnectOnConnectionRefused = true;
    private float disconnectTryInterval          = 3.0f;
    private float connectionRefusedTryInterval   = 3.0f;

    private string hostName = "127.0.0.1";
    public int port = 1933;
    public eTCPSocketType socketType = eTCPSocketType.Text;
    private int bufferSize = 1024;

    private eClientState clientState = eClientState.Disconnected;
    private NetworkStream stream;
    private StreamWriter streamWriter;
    private StreamReader streamReader;
    private Thread readThread;
    private TcpClient client;
    private Queue<eTCPEventType> eventQueue = new Queue<eTCPEventType>();
    private Queue<string> messageQueue = new Queue<string>();
    private Queue<SocketPacket> packetsQueue = new Queue<SocketPacket>();

    private TCPMultiServer serverDelegate;

    public bool IsConnected
    {
        get { return clientState == eClientState.Connected; }
    }

    public eClientState State
    {
        get { return clientState; }
    }

    public TcpClient tcpClient
    {
        get { return client; }
    }

    public static ORTCPClient CreateClientInstance(string name, TcpClient tcpClient, TCPMultiServer serverDelegate)
    {
        GameObject go = new GameObject(name);
        ORTCPClient client = go.AddComponent<ORTCPClient>();
        client.SetTcpClient(tcpClient);
        client.serverDelegate = serverDelegate;
        client.verbose = false;
        return client;
    }

    private void Awake()
    {
        //Can create custom static bools for dontdestroy on load in here
        //if(!exists) exists = true; DontDestroyOnLoad(this.gameObject)
    }

    void Start()
    {
        if (!this.IsConnected)
            Connect();

        Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Where(x => eventQueue.Count > 0)
            .Subscribe(x =>
            {
                eTCPEventType eventType = eventQueue.Dequeue();

                TCPEventParams eventParams = new TCPEventParams();
                eventParams.eventType = eventType;
                eventParams.client = this;
                eventParams.socket = client;

                if (eventType == eTCPEventType.Connected)
                {
                    if(serverDelegate != null)
                        serverDelegate.OnServerConnect(eventParams);
                }
                else if (eventType == eTCPEventType.Disconnected)
                {
                    if(serverDelegate != null)
                        serverDelegate.OnClientDisconnect(eventParams);

                    streamReader.Close();
                    streamWriter.Close();
                    client.Close();
                }
                else if (eventType == eTCPEventType.DataReceived)
                {
                    if (socketType == eTCPSocketType.Text)
                    {
                        eventParams.message = messageQueue.Dequeue();

                        if (OnTCPMessageReceived != null)
                            OnTCPMessageReceived(eventParams);
                    }
                    else if (socketType == eTCPSocketType.Binary)
                    {
                        eventParams.packet = packetsQueue.Dequeue();
                    }

                    if(serverDelegate != null)
                        serverDelegate.OnDataReceived(eventParams);
                }
                else if (eventType == eTCPEventType.ConnectionRefused)
                {
                    if(verbose)
                        print("[TCPClient] Connection refused. Trying again...");

                    if (autoConnectOnConnectionRefused)
                        ORTimer.Execute(this.gameObject, connectionRefusedTryInterval, "OnConnectionRefusedTimer");
                }
            });
    }

    void OnDestory()
    {
        
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    private void ConnectCallback(IAsyncResult asyncResult)
    {
        try
        {
            TcpClient tcpClient = (TcpClient) asyncResult.AsyncState;
            tcpClient.EndConnect(asyncResult);
            SetTcpClient(tcpClient);
        }
        catch (Exception e)
        {
            eventQueue.Enqueue(eTCPEventType.ConnectionRefused);
            Debug.LogWarning("Connection Exception: " + e.Message);
        }
    }

    private void ReadData()
    {
        bool endOfStream = false;

        if (!endOfStream)
        {
            if (socketType == eTCPSocketType.Text)
            {
                String response = null;

                try
                {
                    response = streamReader.ReadLine();
                }
                catch (Exception e)
                {
                    e.ToString();
                }

                if (response != null)
                {
                    response = response.Replace(Environment.NewLine, "");
                    eventQueue.Enqueue(eTCPEventType.DataReceived);
                    messageQueue.Enqueue(response);
                }
                else
                    endOfStream = true;
            }
            else if (socketType == eTCPSocketType.Binary)
            {
                byte[] bytes = new byte[bufferSize];
                int bytesRead = stream.Read(bytes, 0, bufferSize);

                if (bytesRead == 0)
                    endOfStream = true;
                else
                {
                    eventQueue.Enqueue(eTCPEventType.DataReceived);
                    packetsQueue.Enqueue(new SocketPacket(bytes, bytesRead));
                }
            }
        }

        clientState = eClientState.Disconnected;
        client.Close();
        eventQueue.Enqueue(eTCPEventType.Disconnected);
    }

    private void OnDisconnectTimer(ORTimer timer)
    {
        Connect();
    }

    private void OnConnectionRefusedTimer(ORTimer timer)
    {
        
    }

    public void Connect()
    {
        Connect(hostName, port);
    }

    public void Connect(string hostName, int portNumber)
    {
        if(verbose)
            print("[TCPClient] Trying To Connect To: " + hostName + " " + portNumber);
        if (clientState == eClientState.Connected)
            return;

        this.hostName = hostName;
        this.port = portNumber;
        clientState = eClientState.Connecting;
        messageQueue.Clear();
        eventQueue.Clear();
        client = new TcpClient();
        client.BeginConnect(hostName, port, new AsyncCallback(ConnectCallback),client);
    }

    public void Disconnect()
    {
        clientState = eClientState.Disconnected;

        try
        {
            if (streamReader != null) streamReader.Close();
        }
        catch (Exception e)
        {
            e.ToString();
        }

        try
        {
            if (streamWriter != null) streamWriter.Close();
        }
        catch (Exception e)
        {
            e.ToString();
        }

        try
        {
            if (client != null)
                client.Close();
        }
        catch (Exception e)
        {
            e.ToString();
        }

        if (readThread != null)
        {
            readThread.Join(1000);

            if (readThread.ThreadState != ThreadState.Stopped)
            {
                Debug.Log("[TCPClient] Read Thread not closed. Aborting...");
                readThread.Abort();
                readThread = null;
            }
            else
                readThread = null;
        }
    }

    public void Send(string message)
    {
        if (verbose && IsConnected)
            print("[TCPClient] Sending Message: " + message);
        if (!IsConnected)
            return;
        streamWriter.WriteLine(message);
        streamWriter.Flush();
    }

    public void SendBytes(byte[] bytes, int offset, int size)
    {
        if (!IsConnected)
            return;
        stream.Write(bytes, offset, size);
        stream.Flush();
    }

    private void SetTcpClient(TcpClient tcpClient)
    {
        client = tcpClient;
        if (client.Connected)
        {
            stream = client.GetStream();
            streamReader = new StreamReader(stream);
            streamWriter = new StreamWriter(stream);
            clientState = eClientState.Connected;
            eventQueue.Enqueue(eTCPEventType.Connected);
            readThread = new Thread(ReadData);
            readThread.IsBackground = true;
            readThread.Start();
        }
        else
            clientState = eClientState.Disconnected;
    }

}
