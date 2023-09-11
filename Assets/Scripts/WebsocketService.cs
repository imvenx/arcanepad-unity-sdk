using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ArcanepadSDK.Models;

public class WebSocketService<CustomEventNameType>
{
    public delegate void EventCallback(Dictionary<string, object> data, string from);
    public delegate void GenericEventCallback<T>(T eventData, string from) where T : ArcaneBaseEvent;

    public WebSocket Ws;
    private Dictionary<string, List<EventCallback>> eventHandlers = new Dictionary<string, List<EventCallback>>();
    private string Url { get; set; }
    private const int ReconnectionDelayMilliseconds = 1000;
    private string DeviceType { get; set; }
    public string ClientId { get; private set; }
    public string DeviceId { get; private set; }

    // public event Action OnConnected;
    // public event Action OnInitialized;

    public WebSocketService(string url, string deviceType)
    {
        Url = url;
        DeviceType = deviceType;
        InitWebSocket();
    }

    private async void InitWebSocket()
    {
        var clientInitData = new ArcaneClientInitData
        {
            clientType = "external",
            deviceId = "unity-dev",
            deviceType = DeviceType
        };

        string clientInitDataStr = JsonConvert.SerializeObject(clientInitData);

        Url = Url + "?clientInitData=" + clientInitDataStr;
        Ws = new WebSocket(Url);

        Ws.OnOpen += OnOpen;
        Ws.OnError += OnError;
        Ws.OnClose += OnClose;
        Ws.OnMessage += OnMessage;

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
            ClientId = e.assignedClientId;
            DeviceId = e.assignedDeviceId;
            Debug.Log("Client initialized with clientId: " + ClientId + " and deviceId: " + DeviceId);
        });

        await Ws.Connect();
    }

    private void OnOpen()
    {
        Debug.Log("WebSocket connection opened.");
    }

    private void OnError(string errorMessage)
    {
        Debug.LogError($"WebSocket Error: {errorMessage}");
    }

    private async void OnClose(WebSocketCloseCode closeCode)
    {
        Debug.Log($"WebSocket connection closed with code: {closeCode}");
        await Task.CompletedTask;

        // if (closeCode == WebSocketCloseCode.Normal) return;
#if !UNITY_EDITOR
        await Task.Delay(reconnectionDelayMilliseconds); 
        Debug.Log($"Trying to reconnect..."); // TODO: DOES RECONNECT WORK?
        await ws.Connect();
#endif
    }

    private void OnMessage(byte[] message)
    {
        string decodedMessage = System.Text.Encoding.UTF8.GetString(message);
        ArcaneMessageFrom parsedEvent = JsonConvert.DeserializeObject<ArcaneMessageFrom>(decodedMessage);

        if (eventHandlers.TryGetValue(parsedEvent.e["name"].ToString(), out List<EventCallback> handlers))
        {
            var handlersCopy = new List<EventCallback>(handlers);
            foreach (var handler in handlersCopy)
            {
                handler(parsedEvent.e, parsedEvent.from);
            }
        }

    }

    public void Emit(ArcaneBaseEvent e, IList<string> to)
    {
        var eventTo = new ArcaneMessageTo(e, to);
        string eventToStr = JsonConvert.SerializeObject(eventTo);

        if (Ws.State == WebSocketState.Open)
        {
            Ws.SendText(eventToStr);
        }
    }

    public void EmitToViews(ArcaneBaseEvent e)
    {
        Emit(e, Arcane.IframeViewsIds);
    }

    public void EmitToPads(ArcaneBaseEvent e)
    {
        Emit(e, Arcane.IframePadsIds);
    }

    public Action On<CustomEventType>(string eventName, Action<CustomEventType, string> callback) where CustomEventType : ArcaneBaseEvent
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

        return () => Off(eventName, eventCallback);
    }

    public Action On<CustomEventType>(string eventName, Action<CustomEventType> callback) where CustomEventType : ArcaneBaseEvent
    {
        return On<CustomEventType>(eventName, (eventData, _) => callback(eventData));
    }

    public Action On(string eventName, Action callback)
    {
        return On<ArcaneBaseEvent>(eventName, (_, __) => callback());
    }

    public void Off(string eventName, EventCallback eventCallback)
    {
        if (eventHandlers.ContainsKey(eventName))
        {
            eventHandlers[eventName].Remove(eventCallback);
        }
    }
}