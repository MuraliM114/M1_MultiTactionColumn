using System.Net.Sockets;

public enum eTCPEventType
{
    None,
    Connected,
    Disconnected,
    ConnectionRefused,
    DataReceived
}

public enum eTCPSocketType
{
    Binary,
    Text
}

public class SocketPacket
{
    public byte[] bytes = null;
    public int bytesCount = 0;

    public SocketPacket(byte[] bytes, int bytesCount)
    {
        this.bytes = bytes;
        this.bytesCount = bytesCount;
    }
}

public class TCPEventParams
{
    public ORTCPClient client = null;
    public int clientID = 0;
    public string clientName = "";
    public TcpClient socket = null;
    public eTCPEventType eventType = eTCPEventType.None;
    public string message = "";
    public SocketPacket packet = null;
}
