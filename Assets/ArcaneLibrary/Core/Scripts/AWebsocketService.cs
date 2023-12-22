using System;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ArcanepadSDK.Models;
using ArcanepadSDK.AUtils;
using System.Linq;
using ArcanepadSDK.Types;

public class AWebSocketService<CustomEventNameType>
{
    public delegate void EventCallback(Dictionary<string, object> data, string from);
    public WebSocket Ws;
    private Dictionary<string, List<EventCallback>> eventHandlers = new Dictionary<string, List<EventCallback>>();
    private string Url { get; set; }
    private const int ReconnectionDelayMilliseconds = 1000; // It lint's as unused but is being used in an #if
    public string DeviceType { get; set; }
    public string ClientType { get; set; }

    public string ClientId { get; private set; }
    public string DeviceId { get; private set; }
    private ArcaneInitParams _arcaneInitParams { get; set; }
    public string Protocol { get; private set; }
    public string Host { get; private set; }
    private ArcaneClientInitData ClientInitData { get; set; }

    public AWebSocketService(ArcaneInitParams arcaneInitParams)
    {
        // Url = arcaneInitParamsurl;
        // DeviceType = arcaneInitParamsdeviceType;
        _arcaneInitParams = arcaneInitParams;
        InitWebSocket();
    }

    private async void InitWebSocket()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        InitAsExternalClient();
#else
        InitAsIframeClent();
#endif

        if (ClientInitData == null)
        {
            Debug.LogError("ArcaneError: ClientInitData is null on InitializeWebSocket");
            return;
        }

        if (string.IsNullOrEmpty(Url))
        {
            Debug.LogError("ArcaneError: Url is null or empty on InitializeWebSocket");
            return;
        }

        string clientInitDataStr = JsonConvert.SerializeObject(ClientInitData);
        Url = Url + "?clientInitData=" + clientInitDataStr;
        Ws = new WebSocket(Url);

        Ws.OnOpen += OnOpen;
        Ws.OnError += OnError;
        Ws.OnClose += OnClose;
        Ws.OnMessage += OnMessage;

        On(AEventName.Initialize, (InitializeEvent e, string from) => OnInitialize(e));

        await Ws.Connect();
    }

    private void InitAsExternalClient()
    {
        Debug.Log("Initializing Unity Client as External...");

        // if (string.IsNullOrEmpty(arcaneInitParams.arcaneCode))
        // {
        //     var errMsg = "Arcane Error: Need to specify ArcaneCode on the Arcane component inspector view. To get the arcane code go to ArcanePad App, it should be displayed on top or on connect option.";
        //     Debug.LogError(errMsg);
        //     throw new Exception("Thrown Exception: " + errMsg);
        // }

        Protocol = "ws";
        // Host = "192.168." + arcaneInitParams.arcaneCode;
        Host = "127.0.0.1";
        DeviceType = _arcaneInitParams.deviceType;
        Url = Protocol + "://" + Host + ":" + _arcaneInitParams.reverseProxyPort;

        ClientType = ArcaneClientType.external;
        ClientInitData = new ArcaneClientInitData
        {
            clientType = ClientType,
            deviceType = DeviceType
        };
    }

    private void InitAsIframeClent()
    {
        Debug.Log("Initializing Unity Client as Iframe...");

        var isWebEnv = Application.platform == RuntimePlatform.WebGLPlayer;

        if (!isWebEnv)
        {
            Debug.LogError("Trying to initialize as Iframe but isWebEnv is false");
            throw new Exception("Trying to initialize as Iframe but isWebEnv is false");
        }

        if (!AUtils.IsIframe())
        {
            Debug.LogError("Trying to initialize as Iframe but isIframe is false");
            throw new Exception("Trying to initialize as Iframe but isIframe is false");
        }

        var queryParams = AUtils.GetQueryParams();
        DeviceId = queryParams["deviceId"];
        if (string.IsNullOrEmpty(DeviceId))
        {
            Debug.LogError("DeviceId is null or empty on InitAsIframeClient");
            throw new Exception("DeviceId is null or empty on InitAsIframeClient");
        }

        ClientType = ArcaneClientType.iframe;
        ClientInitData = new ArcaneClientInitData
        {
            clientType = ClientType,
            deviceId = DeviceId
        };

        Host = AUtils.GetHost();
        Protocol = "wss";
        Url = Protocol + "://" + Host + ":" + _arcaneInitParams.port;
    }

    public void OnInitialize(InitializeEvent e)
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

        DeviceType = Arcane.Devices.FirstOrDefault(d => d.id == DeviceId).deviceType;

        Debug.Log("Client initialized. ClientId: " + ClientId + " DeviceId: " + DeviceId + " DeviceType: " + DeviceType);
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
        await Task.Delay(ReconnectionDelayMilliseconds); 
        Debug.Log($"Trying to reconnect..."); // TODO: DOES RECONNECT WORK?
        await Ws.Connect();
#endif
    }

    private void OnMessage(byte[] message)
    {
        try
        {
            string decodedMessage = System.Text.Encoding.UTF8.GetString(message);
            // Debug.Log("Decoded message: " + decodedMessage);

            ArcaneMessageFrom parsedEvent = JsonConvert.DeserializeObject<ArcaneMessageFrom>(decodedMessage);

            //  if (parsedEvent.e != null && parsedEvent.e.ContainsKey("name") &&
            //      eventHandlers.TryGetValue(parsedEvent.e["name"].ToString(), out List<EventCallback> handlers))
            if (eventHandlers.TryGetValue(parsedEvent.e["name"].ToString(), out List<EventCallback> handlers))
            {
                var handlersCopy = new List<EventCallback>(handlers);
                foreach (var handler in handlersCopy)
                {
                    // Debug.Log("Invoking handler: " + handler.Method.Name);
                    handler(parsedEvent.e, parsedEvent.from);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception thrown on onmessage" + JsonConvert.SerializeObject(e));
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
        // Debug.Log("Registering handler" + eventName + callback);
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