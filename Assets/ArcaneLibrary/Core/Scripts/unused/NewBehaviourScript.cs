using UnityEngine;
using NativeWebSocket;

public class A_OldTest : MonoBehaviour
{
    WebSocket websocket;

    async void Start()
    {
        websocket = new WebSocket("ws://localhost:3689");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            Message myMessage = new Message("Greeting from unity!", "Hello, server! absolute url: " + Application.absoluteURL);
            SendMessageToServer(myMessage);
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed! " + e);
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage! " + System.Text.Encoding.UTF8.GetString(bytes));
        };

        // waiting for messages
        await websocket.Connect();
    }

    async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    public async void SendMessageToServer(Message message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            string json = JsonUtility.ToJson(message);
            await websocket.SendText(json);
        }
    }
}

[System.Serializable]
public class Message
{
    public string type;
    public string content;

    public Message(string type, string content)
    {
        this.type = type;
        this.content = content;
    }
}