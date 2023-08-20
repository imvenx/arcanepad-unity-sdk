using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;
using System.Reflection;

public interface IWebSocketService { }

public class WebSocketService<T> : IWebSocketService
{
    public delegate void EventCallback<U>(U data);

    public WebSocket ws;
    private Dictionary<string, List<Delegate>> eventHandlers = new Dictionary<string, List<Delegate>>();
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

        url = url + "?clientInitData=" + JsonUtility.ToJson(clientInitData);
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
        T eventName = JsonUtility.FromJson<BaseEvent<T>>(decodedMessage).name; // Assuming event always has a name

        if (eventHandlers.TryGetValue(eventName.ToString(), out List<Delegate> callbacks))
        {
            foreach (var callback in callbacks)
            {
                // Try to invoke the callback with the decoded message
                try
                {
                    Type callbackType = callback.GetType();
                    MethodInfo invokeMethod = callbackType.GetMethod("Invoke");
                    Type eventType = invokeMethod.GetParameters()[0].ParameterType;
                    object eventObj = JsonUtility.FromJson(decodedMessage, eventType);
                    callback.DynamicInvoke(eventObj);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error processing event: {ex.Message}");
                }
            }
        }
    }


    private void OnInit(byte[] messageData)
    {
        string decodedMessage = System.Text.Encoding.UTF8.GetString(messageData);
        AssignDataInitEvent initMsg = JsonUtility.FromJson<AssignDataInitEvent>(decodedMessage);

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
    }

    public void Emit(EventFromTo<T> e)
    {

        string message = JsonUtility.ToJson(e);
        Debug.Log("toooo:!" + message);
        if (ws.State == WebSocketState.Open)
        {
            ws.SendText(message);
        }

        Debug.Log("sent mesage: " + message);
    }

    public void On<U>(string eventName, EventCallback<U> callback)
    {
        if (!eventHandlers.ContainsKey(eventName))
        {
            eventHandlers[eventName] = new List<Delegate>();
        }
        eventHandlers[eventName].Add(callback);
    }

    public void Off<U>(string eventName, EventCallback<U> callback)
    {
        if (eventHandlers.ContainsKey(eventName))
        {
            var callbackList = eventHandlers[eventName];
            callbackList.RemoveAll(del => del == (Delegate)callback);

            if (callbackList.Count == 0)
            {
                eventHandlers.Remove(eventName);
            }
        }
    }

    public async void Close()
    {
        await ws.Close();
    }
}

public class BaseEvent<T>
{
    public T name;

    public BaseEvent(T name)
    {
        this.name = name;
    }
}

public class EventTo<T> : BaseEvent<T>
{
    public string[] to;

    public EventTo(T name, string[] to) : base(name)
    {
        this.to = to;
    }
}

public class EventFrom<T> : BaseEvent<T>
{
    public string from;

    // public EventFrom(T name, string from) : base(name)
    public EventFrom(T name) : base(name)
    {
        // this.from = from;
    }
}

public class EventFromTo<T> : EventFrom<T>
{
    public string[] to;

    // public EventFromTo(T name, string[] to, string from) : base(name, from)
    public EventFromTo(T name, string[] to) : base(name)
    {
        this.to = to;
    }
}

public class AssignDataInitEvent : BaseEvent<string>
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
    public static readonly string AssignDataInit = "AssignDataInit";
}

// public enum EventName
// {
//     None,
//     Left,
//     Right,
//     Up,
//     Down,
//     Enter,
//     Info,
//     Extra,
//     Back,
//     AssignDataInit,
//     SetDeviceType,
//     EnterDevMode,
//     EnterApp,
//     ExitApp,
//     RefreshGlobalState
// }

// public enum ArcaneClientType
// {
//     Web,
//     Iframe
// }

public class ArcaneClientType
{
    public static readonly string web = "web";
    public static readonly string iframe = "iframe";
}

// public enum ArcaneDeviceType
// {
//     Pad,
//     View,
//     None
// }

public class ArcaneDeviceType
{
    public static readonly string pad = "pad";
    public static readonly string view = "view";
    public static readonly string none = "none";
}

// public class ArcaneCustomEvent
// {
//     public string name;
//     public dynamic val;
//     public string fromId;
//     public string[] toIds;
// }

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

// public interface InitIframeQueryParams
// {
//     string deviceId { get; }
// }

// public class MessageDestinataries
// {
//     public string[] ids;
//     public string view;
//     public string pads;
// }

public class MessageDestinataries
{
    public static readonly string views = "views";
    public static readonly string pads = "pads";
}

public class AttackEvent : EventFromTo<string>
{
    public int damage;
    public AttackEvent(string[] to, int damage) : base(CustomEventNames.Attack, to)
    {
        this.damage = damage;
    }
}

// public class AttackFromEvent : EventFrom<string>
// {
//     public int damage;
//     public AttackFromEvent(string from, int damage) : base(CustomEventNames.Attack, from)
//     {
//         this.damage = damage;
//     }
// }


// public enum CustomEventNames
// {
//     Attack
// }

public class CustomEventNames
{
    public static readonly string Attack = "Attack";
    public static readonly string Other = "Other";
}