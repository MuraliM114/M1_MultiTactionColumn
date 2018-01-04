using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class TCPAbstractMultiServer : MonoBehaviour
{
    protected class NewConnection
    {
        public TcpClient tcpClient;

        public NewConnection(TcpClient client)
        {
            this.tcpClient = client;
        }
    }

    protected int clientID;
    protected Dictionary<int, ORTCPClient> clients;
    protected TcpListener tcpListener;
    protected Queue<NewConnection> newConnections;
    protected bool isListening;

    public int ClientCount
    {
        get { return clients.Count; }
    }

    public bool IsListening
    {
        get { return isListening; }
    }

    protected int SaveClient(ORTCPClient clientToSave)
    {
        int currentClientID = clientID;
        clients.Add(currentClientID, clientToSave);
        clientID++;
        return currentClientID;
    }

    protected int RemoveClient(int clientIDToRemove)
    {
        ORTCPClient client = GetClient(clientIDToRemove);
        if (client == null)
            return clientIDToRemove;

        client.Disconnect();
        clients.Remove(clientIDToRemove);
        Destroy(client.gameObject);
        return clientIDToRemove;
    }

    protected int RemoveClient(ORTCPClient clientToRemove)
    {
        int clientToRemoveID = GetClientID(clientToRemove);
        if (clientToRemoveID < 0)
        {
            Destroy(clientToRemove.gameObject);
            return -1;
        }
        return RemoveClient(clientToRemoveID);
    }

    protected TcpClient GetTcpClient(int clientIDToGet)
    {
        ORTCPClient client = null;
        if (!clients.TryGetValue(clientIDToGet, out client))
            return null;
        return client.tcpClient;
    }

    protected ORTCPClient GetClient(int clientIDToGet)
    {
        ORTCPClient client = null;
        if (clients.TryGetValue(clientIDToGet, out client))
            return client;
        return null;
    }

    protected int GetClientID(ORTCPClient clientToGet)
    {
        foreach(KeyValuePair<int, ORTCPClient> client in clients)
            if (client.Value == clientToGet)
                return client.Key;
        return -1;
    }

    protected int GetClientID(TcpClient tcpClientToGet)
    {
        foreach(KeyValuePair<int, ORTCPClient> client in clients)
            if (client.Value.tcpClient == tcpClientToGet)
                return client.Key;
        return -1;
    }

    protected void AcceptClient()
    {
        tcpListener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClientCallback), tcpListener);
    }

    protected void AcceptTcpClientCallback(IAsyncResult asyncResult)
    {
        TcpListener _tcpListener = (TcpListener) asyncResult.AsyncState;
        TcpClient _tcpClient = tcpListener.EndAcceptTcpClient(asyncResult);

        if (_tcpListener != null && _tcpClient.Connected)
        {
            newConnections.Enqueue(new NewConnection(_tcpClient));
            AcceptClient();
        }
    }
}
