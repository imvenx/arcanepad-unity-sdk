using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArcanepadSDK;
using ArcanepadSDK.Models;
using UnityEngine;



public class Arcane : MonoBehaviour
{
    public static WebSocketService<string> msg;
    public static IList<ArcaneDevice> devices;
    // public static List<ArcanePad> Pads = new List<ArcanePad>();
    private static List<string> internalViewsIds = new List<string>();
    private static List<string> internalPadsIds = new List<string>();
    public static List<string> iframeViewsIds = new List<string>();
    public static List<string> iframePadsIds = new List<string>();
    private static TaskCompletionSource<InitialState> _arcaneClientInitialized = new TaskCompletionSource<InitialState>();
    // private static TaskCompletionSource<ArcaneClientInitializeEvent> _ArcaneClientInitialized = new TaskCompletionSource<ArcaneClientInitializeEvent>();

    public static Task<InitialState> ArcaneClientInitialized()
    {
        return _arcaneClientInitialized.Task;
    }

    void Awake()
    {
        string url = "wss://localhost:3005";
#if DEBUG || UNITY_EDITOR
        url = "ws://localhost:3009";
#endif

        devices = new List<ArcaneDevice>();
        msg = new WebSocketService<string>(url, ArcaneDeviceType.view);

        Action unsubscribeInit = null;
        unsubscribeInit = msg.On(AEventName.Initialize, (InitializeEvent e, string from) => Initialize(e, unsubscribeInit));

        msg.On(AEventName.RefreshGlobalState, (RefreshGlobalStateEvent e) => RefreshGlobalState(e.refreshedGlobalState));
    }

    void Update()
    {
#if UNITY_EDITOR
        if (msg != null)
        {
            msg.ws.DispatchMessageQueue();
        }
#endif
    }

    void OnDestroy()
    {
        msg.ws.Close();
    }

    void OnDisable()
    {
        msg.ws.Close();
    }

    private void Initialize(InitializeEvent e, Action unsubscribeInit)
    {
        RefreshGlobalState(e.globalState);

        Debug.Log("Client initialized");

        var pads = GetPads(e.globalState.devices);

        var initialState = new InitialState(pads);

        _arcaneClientInitialized.SetResult(initialState);

        unsubscribeInit();
    }

    private void RefreshGlobalState(GlobalState globalState)
    {
        devices = globalState.devices;

        RefreshClientsIds(devices);
    }

    public List<ArcanePad> GetPads(IList<ArcaneDevice> _devices)
    {
        List<ArcanePad> Pads = new List<ArcanePad>();

        var padDevices = _devices.Where(device => device.deviceType == ArcaneDeviceType.pad && device.clients.Any(c => c.clientType == ArcaneClientType.iframe)).ToList();

        padDevices.ForEach(padDevice =>
        {
            var iframeClientId = padDevice.clients.FirstOrDefault(c => c.clientType == ArcaneClientType.iframe)?.id;
            var internalClientId = padDevice.clients.FirstOrDefault(c => c.clientType == ArcaneClientType.@internal)?.id;

            if (string.IsNullOrEmpty(iframeClientId))
            {
                Debug.LogError("Tried to set pad but iframeClientId was not found");
            }

            if (string.IsNullOrEmpty(internalClientId))
            {
                Debug.LogError("Tried to set pad but internalClientId was not found");
            }

            Pads.Add(new ArcanePad(
                deviceId: padDevice.id,
                internalId: internalClientId,
                iframeId: iframeClientId,
                isConnected: true
            ));
        });

        return Pads;
    }


    // void InitPads(IList<ArcaneDevice> _devices)
    // {
    //     var padDevices = _devices.Where(device => device.deviceType == ArcaneDeviceType.pad).ToList();

    //     padDevices.ForEach(padDevice =>
    //     {
    //         var iframeClientId = padDevice.clients.FirstOrDefault(c => c.clientType == ArcaneClientType.iframe).id;
    //         var internalClientId = padDevice.clients.FirstOrDefault(c => c.clientType == ArcaneClientType.@internal).id;

    //         if (string.IsNullOrEmpty(iframeClientId))
    //         {
    //             Debug.LogError("Tried to set pad but iframeClientId was not found");
    //         }

    //         if (string.IsNullOrEmpty(internalClientId))
    //         {
    //             Debug.LogError("Tried to set pad but internalClientId was not found");
    //         }

    //         var padExists = Pads.Any(p => p.DeviceId == padDevice.id);

    //         if (!padExists)
    //         {
    //             Pads.Add(new ArcanePad(
    //                 deviceId: padDevice.id,
    //                 internalId: internalClientId,
    //                 iframeId: iframeClientId,
    //                 isConnected: true
    //             ));
    //         }
    //     });
    // }

    void RefreshClientsIds(IList<ArcaneDevice> _devices)
    {
        internalPadsIds = _devices.Where(device => device.deviceType == ArcaneDeviceType.pad).SelectMany(device => device.clients
            .Where(client => client.clientType == ArcaneClientType.@internal).Select(client => client.id)).ToList();

        internalViewsIds = _devices.Where(device => device.deviceType == ArcaneDeviceType.view).SelectMany(device => device.clients
            .Where(client => client.clientType == ArcaneClientType.@internal).Select(client => client.id)).ToList();

        iframePadsIds = _devices.Where(device => device.deviceType == ArcaneDeviceType.pad).SelectMany(device => device.clients
            .Where(client => client.clientType != ArcaneClientType.@internal).Select(client => client.id)).ToList();

        iframeViewsIds = _devices.Where(device => device.deviceType == ArcaneDeviceType.view).SelectMany(device => device.clients
            .Where(client => client.clientType != ArcaneClientType.@internal).Select(client => client.id)).ToList();
    }
}

