using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArcanepadSDK;
using ArcanepadSDK.Models;
using UnityEngine;



public class Arcane : MonoBehaviour
{
    public static WebSocketService<string> Msg;
    public static IList<ArcaneDevice> Devices;
    // public static List<ArcanePad> Pads = new List<ArcanePad>();
    private static List<string> InternalViewsIds = new List<string>();
    private static List<string> InternalPadsIds = new List<string>();
    public static List<string> IframeViewsIds = new List<string>();
    public static List<string> IframePadsIds = new List<string>();
    private static TaskCompletionSource<List<ArcanePad>> _ArcaneClientInitialized = new TaskCompletionSource<List<ArcanePad>>();
    // private static TaskCompletionSource<ArcaneClientInitializeEvent> _ArcaneClientInitialized = new TaskCompletionSource<ArcaneClientInitializeEvent>();

    public static Task<List<ArcanePad>> ArcaneClientInitialized()
    {
        return _ArcaneClientInitialized.Task;
    }

    void Awake()
    {
        string url = "wss://localhost:3005";
#if DEBUG || UNITY_EDITOR
        url = "ws://localhost:3009";
#endif

        Devices = new List<ArcaneDevice>();
        Msg = new WebSocketService<string>(url, ArcaneDeviceType.view);

        Action<ArcaneClientInitializeEvent> myCallback = (ArcaneClientInitializeEvent e) => OnInitialize(e);

        Msg.On(AEventName.Initialize, (ArcaneClientInitializeEvent e, string from) => OnInitialize(e));

        //     Action unsubscribe = Msg.On<IframePadConnectEvent>(AEventName.IframePadConnect, (e, from) =>
        //     {
        //         Debug.Log("asdasd");
        //     });

        //     Action other = Msg.On<IframePadConnectEvent>(AEventName.IframePadConnect, (e, from) =>
        //   {
        //       Debug.Log("qweqwe");
        //   });

        //     await Task.Delay(5000);
        //     Debug.Log("offf!");
        //     unsubscribe();
    }

    void asd()
    {
        Debug.Log("asdasd");
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Msg != null)
        {
            Msg.ws.DispatchMessageQueue();
        }
#endif
    }

    void OnDestroy()
    {
        Msg.ws.Close();
    }

    void OnDisable()
    {
        Msg.ws.Close();
    }

    void RefreshArcaneState(GlobalState globalState)
    {
        Devices = globalState.devices;
        // InitPads(Devices);
        InitClientsIds(Devices);
    }

    private void OnInitialize(ArcaneClientInitializeEvent e)
    {
        RefreshArcaneState(e.globalState);

        var pads = GetPads(e.globalState.devices);

        _ArcaneClientInitialized.SetResult(pads);

        Debug.Log("Client initialized");
    }

    public List<ArcanePad> GetPads(IList<ArcaneDevice> _devices)
    {
        List<ArcanePad> Pads = new List<ArcanePad>();

        var padDevices = _devices.Where(device => device.deviceType == ArcaneDeviceType.pad).ToList();

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

    void InitClientsIds(IList<ArcaneDevice> _devices)
    {
        InternalPadsIds = _devices.Where(device => device.deviceType == ArcaneDeviceType.pad).SelectMany(device => device.clients
            .Where(client => client.clientType == ArcaneClientType.@internal).Select(client => client.id)).ToList();

        InternalViewsIds = _devices.Where(device => device.deviceType == ArcaneDeviceType.view).SelectMany(device => device.clients
            .Where(client => client.clientType == ArcaneClientType.@internal).Select(client => client.id)).ToList();

        IframePadsIds = _devices.Where(device => device.deviceType == ArcaneDeviceType.pad).SelectMany(device => device.clients
            .Where(client => client.clientType != ArcaneClientType.@internal).Select(client => client.id)).ToList();

        IframeViewsIds = _devices.Where(device => device.deviceType == ArcaneDeviceType.view).SelectMany(device => device.clients
            .Where(client => client.clientType != ArcaneClientType.@internal).Select(client => client.id)).ToList();
    }
}

