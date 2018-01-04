using UnityEngine;

/// <summary>
/// Attach this to an empty game object with the MultiServer script.
/// This script will handle any incoming messages from a client to this server.
/// </summary>
public class ServerListenerObject : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	    TCPMultiServer.Instance.OnTCPMessageReceived += OnTCPMessage;
	}

    /// <summary>
    /// Determine what to do with the received event paramters.
    /// Message string can be parsed and used to delegate commands and what not.
    /// </summary>
    /// <param name="eventParams"></param>
    private void OnTCPMessage(TCPEventParams eventParams)
    {

    }

}
