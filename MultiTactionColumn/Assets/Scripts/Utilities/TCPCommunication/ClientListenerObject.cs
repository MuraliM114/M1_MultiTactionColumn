using System.Collections;
using UniRx;
using UnityEngine;

/// <summary>
/// Attach this and the ORTCPClient script to a gameObject to great a client and listener.
/// OnTCPServerMEssageReceived can be filled with received messages from the server and 
/// delegate what any received message does.
/// </summary>
public class ClientListenerObject : MonoBehaviour
{
    public ORTCPClient client;

    private static bool exists = false;

    void Awake()
    {
        if (!exists)
        {
            exists = true;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

	// Use this for initialization
	void Start ()
	{
	    if (client != null)
	        client.OnTCPMessageReceived += OnTCPServerMessageReceived;
	}

    private void OnTCPServerMessageReceived(TCPEventParams eventParams)
    {
        
    }
}
