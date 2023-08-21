using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;
using Newtonsoft.Json;

public interface IWebSocketService { }

public class WebSocketService<CustomEventNameType> : IWebSocketService
{
    public delegate void EventCallback(Dictionary<string, object> data, string from);
    public delegate void GenericEventCallback<T>(T eventData, string from) where T : BaseEvent;

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
        // this.deviceType = deviceType == ArcaneDeviceType.View ? "view" : deviceType == ArcaneDeviceType.Pad ? "pad" : "none";
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
        ws.OnMessage += OnInit;

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
        await Task.Delay(reconnectionDelayMilliseconds);
        await ws.Connect();
    }

    private void OnMessage(byte[] message)
    {
        string decodedMessage = System.Text.Encoding.UTF8.GetString(message);
        EventFrom parsedEvent = JsonConvert.DeserializeObject<EventFrom>(decodedMessage);

        if (eventHandlers.TryGetValue(parsedEvent.e["name"].ToString(), out List<EventCallback> handlers))
        {
            foreach (var handler in handlers)
            {
                handler(parsedEvent.e, parsedEvent.from);
            }
        }
    }

    private void OnInit(byte[] messageData)
    {
        string decodedMessage = System.Text.Encoding.UTF8.GetString(messageData);
        AssignDataInitEvent initMsg = JsonConvert.DeserializeObject<AssignDataInitEvent>(decodedMessage);

        if (initMsg.name != EventName.AssignDataInit)
        {
            Debug.LogWarning($"Received unexpected event before init: {decodedMessage}. Ignoring.");
            return;
        }

        if (string.IsNullOrEmpty(initMsg.assignedClientId) || string.IsNullOrEmpty(initMsg.assignedDeviceId))
        {
            Debug.LogWarning($"Missing client id or device id on init: {decodedMessage}.");
            return;
        }

        Debug.Log($"Assigned ID: {initMsg.assignedClientId}");
        clientId = initMsg.assignedClientId;
        deviceId = initMsg.assignedDeviceId;

        // PlayerPrefs.SetString("ws-id", initMsg.assignedClientId);

        ws.OnMessage -= OnInit;
        ws.OnMessage += OnMessage;
        OnInitialized?.Invoke();

        this.Emit(new OtherClientConnectedEvent(clientId), "all");
    }

    public void Emit(BaseEvent e, string[] to)
    {
        var eventTo = new EventTo(e, to);
        string eventToStr = JsonConvert.SerializeObject(eventTo);

        if (ws.State == WebSocketState.Open)
        {
            ws.SendText(eventToStr);
        }
    }
    public void Emit(BaseEvent e, string to)
    {
        var eventTo = new EventTo(e, to);
        string eventToStr = JsonConvert.SerializeObject(eventTo);

        if (ws.State == WebSocketState.Open)
        {
            ws.SendText(eventToStr);
        }
    }


    public void On<CustomEventType>(string eventName, Action<CustomEventType, string> callback) where CustomEventType : BaseEvent
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

public class BaseEvent
{
    public string name;

    public BaseEvent(string name)
    {
        this.name = name;
    }
}

public class EventTo
{
    public BaseEvent e;
    public object to;
    public EventTo(BaseEvent e, string[] to)
    {
        this.e = e;
        this.to = to;
    }

    public EventTo(BaseEvent e, string to)
    {
        this.e = e;
        this.to = to;
    }
}

public class EventFrom
{
    public Dictionary<string, object> e;
    public string from;
    public EventFrom(Dictionary<string, object> e, string from)
    {
        this.e = e;
        this.from = from;
    }
}

public class AssignDataInitEvent : BaseEvent
{
    public string assignedClientId;
    public string assignedDeviceId;

    public AssignDataInitEvent(string assignedClientId, string assignedDeviceId) : base(EventName.AssignDataInit)
    {
        this.assignedClientId = assignedClientId;
        this.assignedDeviceId = assignedDeviceId;
    }
}

public class EventName
{
    public static string AssignDataInit = "AssignDataInit";
}

public class ArcaneClientType
{
    public static string web = "web";
    public static string iframe = "iframe";
}

public class ArcaneDeviceType
{
    public static readonly string pad = "pad";
    public static readonly string view = "view";
    public static readonly string none = "none";
}

public class MessageDestinataries
{
    public static string views = "views";
    public static string pads = "pads";
    public static string all = "all";
    public static string self = "self";
    public static string server = "server";
}

public class ArcaneDeviceEvent
{
    public string type; // vibrate, getRotationVector, endRotationVector
}

public class ArcaneClientInitData
{
    public string clientType;
    public string deviceType;
    public string deviceId;
}

public class AssignedDataInitEvent
{
    public string eventTag = "init";
    public string assignedClientId;
    public string assignedDeviceId;
}

public interface InitIframeQueryParams
{
    string deviceId { get; }
}

public class AttackEvent : BaseEvent
{
    public int damage;
    public AttackEvent(int damage) : base(CustomEventNames.Attack)
    {
        this.damage = damage;
    }
}

public class OtherClientConnectedEvent : BaseEvent
{
    public string clientId;
    public OtherClientConnectedEvent(string clientId) : base(ArcaneEventsNames.OtherClientConnected)
    {
        this.clientId = clientId;
    }
}

public class ArcaneEventsNames
{
    public static string OtherClientConnected = "OtherClientConnected";
}

public class CustomEventNames
{
    public static string Attack = "Attack";
}