using System;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class AWebSocketClient : MonoBehaviour
{
    // private ClientWebSocket client = new ClientWebSocket();

    // async void Start()
    // {
    //     // Bypass all SSL certificate validation
    //     ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

    //     try
    //     {
    //         Uri serverUri = new Uri("wss://192.168.0.30:3005/");
    //         await client.ConnectAsync(serverUri, CancellationToken.None);

    //         Debug.Log("WebSocket connection opened!");

    //         // Now that we're connected, start listening for messages
    //         _ = ListenForMessages();
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError("Exception during connection: " + e);
    //     }
    // }

    // private async System.Threading.Tasks.Task ListenForMessages()
    // {
    //     byte[] buffer = new byte[1024];

    //     while (client.State == WebSocketState.Open)
    //     {
    //         var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    //         if (result.MessageType == WebSocketMessageType.Text)
    //         {
    //             string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
    //             Debug.Log("Received: " + message);
    //         }
    //     }

    //     Debug.Log("WebSocket connection closed!");
    // }
}
