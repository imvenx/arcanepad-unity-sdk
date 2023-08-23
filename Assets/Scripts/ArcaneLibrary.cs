using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ArcanepadSDK.Models;

public class WebSocketService<CustomEventNameType> : IWebSocketService
{
    public delegate void EventCallback(Dictionary<string, object> data, string from);
    public delegate void GenericEventCallback<T>(T eventData, string from) where T : ArcaneBaseEvent;

    public WebSocket ws;
    private Dictionary<string, List<EventCallback>> eventHandlers = new Dictionary<string, List<EventCallback>>();
    private string url;
    private const int reconnectionDelayMilliseconds = 1000;
    private string deviceType;
    private string clientId;
    private string deviceId;

    public event Action OnConnected;
    public event Action OnInitialized;

    public WebSocketService(string url, string deviceType)
    {
        this.url = url;
        this.deviceType = deviceType;
        InitWebSocket();
    }

    private async void InitWebSocket()
    {
        var clientInitData = new ArcaneClientInitData
        {
            clientType = "external",
            deviceId = "unity-dev",
            deviceType = deviceType
        };

        string clientInitDataStr = JsonConvert.SerializeObject(clientInitData);

        url = url + "?clientInitData=" + clientInitDataStr;
        ws = new WebSocket(url);

        ws.OnOpen += OnOpen;
        ws.OnError += OnError;
        ws.OnClose += OnClose;
        ws.OnMessage += OnMessage;
        // ws.OnMessage += OnInit;

        On(AEventName.Initialize, (InitializeEvent e, string from) =>
        {
            if (string.IsNullOrEmpty(e.assignedClientId))
            {
                Debug.LogError("Missing clientId on initialize");
                return;
            }
            if (string.IsNullOrEmpty(e.assignedDeviceId))
            {
                Debug.LogError("Missing deviceOd on initialize");
                return;
            }
            clientId = e.assignedClientId;
            deviceId = e.assignedDeviceId;
            Debug.Log("Client initialized with clientId: " + clientId + " and deviceId: " + deviceId);
        });

        await ws.Connect();
    }

    private void OnOpen()
    {
        Debug.Log("WebSocket connection opened.");
        OnConnected?.Invoke();
    }

    private void OnError(string errorMessage)
    {
        Debug.LogError($"WebSocket Error: {errorMessage}");
    }

    private async void OnClose(WebSocketCloseCode closeCode)
    {
        Debug.Log($"WebSocket connection closed with code: {closeCode}");
        await Task.CompletedTask;

#if !UNITY_EDITOR
        await Task.Delay(reconnectionDelayMilliseconds);
        Debug.Log($"Trying to reconnect...");
        await ws.Connect();
#endif
    }

    private void OnMessage(byte[] message)
    {
        string decodedMessage = System.Text.Encoding.UTF8.GetString(message);
        ArcaneMessageFrom parsedEvent = JsonConvert.DeserializeObject<ArcaneMessageFrom>(decodedMessage);

        if (eventHandlers.TryGetValue(parsedEvent.e["name"].ToString(), out List<EventCallback> handlers))
        {
            foreach (var handler in handlers)
            {
                handler(parsedEvent.e, parsedEvent.from);
            }
        }
    }

    public void Emit(ArcaneBaseEvent e, string[] to)
    {
        var eventTo = new ArcaneMessageTo(e, to);
        string eventToStr = JsonConvert.SerializeObject(eventTo);

        if (ws.State == WebSocketState.Open)
        {
            ws.SendText(eventToStr);
        }
    }
    public void Emit(ArcaneBaseEvent e, string to)
    {
        var eventTo = new ArcaneMessageTo(e, to);
        string eventToStr = JsonConvert.SerializeObject(eventTo);

        if (ws.State == WebSocketState.Open)
        {
            ws.SendText(eventToStr);
        }
    }

    public void On<CustomEventType>(string eventName, Action<CustomEventType, string> callback) where CustomEventType : ArcaneBaseEvent
    {
        try
        {
            EventCallback eventCallback = (dataDict, from) =>
            {
                CustomEventType eventData = JsonConvert.DeserializeObject<CustomEventType>(JsonConvert.SerializeObject(dataDict));
                callback(eventData, from);
            };

            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = new List<EventCallback>();
            }
            eventHandlers[eventName].Add(eventCallback);
        }
        catch (Exception e)
        {
            Debug.LogError("Exception on wsService.On" + e);
        }

    }

    public void Off(string eventName, EventCallback eventCallback)
    {
        if (eventHandlers.ContainsKey(eventName))
        {
            eventHandlers[eventName].Remove(eventCallback);
        }
    }

    public async void Close()
    {
        await ws.Close();
    }
}