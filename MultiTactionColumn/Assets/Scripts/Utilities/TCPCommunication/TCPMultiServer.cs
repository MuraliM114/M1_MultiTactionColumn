using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UniRx;

//using UniRx;

public class TCPMultiServer : TCPAbstractMultiServer
{
    public delegate void TCPServerMessageReceivedEvent(TCPEventParams eventParams);
    public event TCPServerMessageReceivedEvent OnTCPMessageReceived;

    public bool verbose = true;
    public int port = 1933;

    private static TCPMultiServer s_instance;
    public static TCPMultiServer Instance { get { return s_instance; } }

    private static bool exists = false;

    /// <summary>
    /// Awake assigns the static singleton.
    /// Because reloading a scene will cause Unity to duplicate objects, the static
    /// exists boolean will destroy any duplicate objects so there is only ever 1 of these.
    /// </summary>
    void Awake()
    {
        if (!exists)
        {
            s_instance = this;
            exists = true;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    void Start()
    {
        isListening = false;
        newConnections = new Queue<NewConnection>();
        clients = new Dictionary<int, ORTCPClient>();
        StartListening();

        Observable
            .Interval(TimeSpan.FromSeconds(1))
            .Where(x => newConnections.Count > 0)
            .Subscribe(x =>
            {
                NewConnection newConnection = newConnections.Dequeue();
                ORTCPClient client = ORTCPClient.CreateClientInstance("MultiserverClient", newConnection.tcpClient, this);

                int clientId = SaveClient(client);
                TCPEventParams eventParams = new TCPEventParams();
                eventParams.eventType = eTCPEventType.Connected;
                eventParams.client = client;
                eventParams.clientID = clientId;
                eventParams.socket = newConnection.tcpClient;

                if(verbose)
                    print("[TCPServer] New Client Connected: " + client.name);
            });
    }

    void OnDestroy()
    {
        
    }

    /// <summary>
    /// Send any reset or close commands here. Stop listening.
    /// </summary>
    void OnApplicationQuit()
    {
        DisconnectAllClients();
        StopListening();
    }

    public void OnServerConnect(TCPEventParams eventParams)
    {
        
    }

    public void OnClientDisconnect(TCPEventParams eventParams)
    {
        if(verbose)
            print("[TCPSever] OnClientDisconnect.");

        eventParams.clientID = GetClientID(eventParams.client);
        RemoveClient(eventParams.client);
    }

    public void OnDataReceived(TCPEventParams eventParams)
    {
        if (verbose)
            print("[TCPSever] OnDataReceived");
        eventParams.clientID = GetClientID(eventParams.client);

        if (OnTCPMessageReceived != null)
            OnTCPMessageReceived(eventParams);
    }

    public void StartListening()
    {
        StartListening(port);
    }

    public void StartListening(int portNumber)
    {
        if(verbose)
            print("[TCPServer] Start Listening on Port: " + portNumber);

        if (isListening)
            return;

        this.port = portNumber;
        isListening = true;
        newConnections.Clear();

        tcpListener = new TcpListener(IPAddress.Any, port);
        tcpListener.Start();
        AcceptClient();
    }

    public void StopListening()
    {
        isListening = false;

        if (tcpListener == null)
            return;
        tcpListener.Stop();
        tcpListener = null;
    }

    public void DisconnectAllClients()
    {
        if(verbose)
            print("[TCPSever] DisconnectAllClients");

        foreach (KeyValuePair<int, ORTCPClient> client in clients)
            client.Value.Disconnect();

        clients.Clear();
    }

    public void SendAllClientsMessage(string message)
    {
        if (verbose)
            print("[TCPServer] SendAllClientsMessage: " + message);

        foreach(KeyValuePair<int, ORTCPClient> client in clients)
            client.Value.Send(message);
    }

    public void DisconnectclientWithID(int clientToDisconnect)
    {
        if(verbose)
            print("[TCPServer] DisconnectClientWithID: " + clientToDisconnect);

        ORTCPClient client = GetClient(clientToDisconnect);
        if (client == null)
            return;
        client.Disconnect();
    }

    public void SendClientWithIDMEssage(int clientID, string message)
    {
        if(verbose)
            print("[TCPServer] SendClientWithIDMessage: " + clientID + " " + message);

        ORTCPClient client = GetClient(clientID);
        if (client == null)
            return;
        client.Send(message);
    }

    public ORTCPClient GetClientStatusWithID(int clientID)
    {
        ORTCPClient client = GetClient(clientID);
        if (client == null)
            return null;

        return client;
    }

}
