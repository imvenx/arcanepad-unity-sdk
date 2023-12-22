using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ArcanepadSDK;
using ArcanepadSDK.Models;
using ArcanepadSDK.Types;
using UnityEditor;
using UnityEngine;

public class Arcane : MonoBehaviour
{
    public static AWebSocketService<string> Msg;
    public static IList<ArcaneDevice> Devices = new List<ArcaneDevice>();
    public static List<ArcanePad> Pads = new List<ArcanePad>();
    private static List<string> InternalViewsIds = new List<string>();
    private static List<string> InternalPadsIds = new List<string>();
    public static List<string> IframeViewsIds = new List<string>();
    public static List<string> IframePadsIds = new List<string>();
    public static ArcanePad Pad { get; private set; }
    public string LibraryVersion { get; } = "1.6.0";
    [DllImport("__Internal")]
    private static extern void SetFullScreen();

    [SerializeField]
    private static ArcaneDeviceTypeEnum DeviceType;
    // [SerializeField]
    // private static string Port = "3685";
    // [SerializeField]
    // private static string ReverseProxyPort = "3689";

    private static ArcaneInitParams _arcaneInitParams;
    // [SerializeField]
    // private string ArcaneCode = "";

    private static TaskCompletionSource<InitialState> _arcaneClientInitialized = new TaskCompletionSource<InitialState>();
    // private static TaskCompletionSource<ArcaneClientInitializeEvent> _ArcaneClientInitialized = new TaskCompletionSource<ArcaneClientInitializeEvent>();

    public static Task<InitialState> ArcaneClientInitialized()
    {
        return _arcaneClientInitialized.Task;
    }

    void Awake()
    {
#if !UNITY_EDITOR && UNITY_WEBGL
    SetFullScreen();
#endif

        DontDestroyOnLoad(this);

        // string url = "wss://localhost:3005";
        // #if DEBUG || UNITY_EDITOR || UNITY_STANDALONE
        // url = "ws://localhost:3009";
        // #endif
    }

    public static void Init(ArcaneInitParams arcaneInitParams = null)
    {
        if (arcaneInitParams == null) arcaneInitParams = new ArcaneInitParams();

        _arcaneInitParams = arcaneInitParams;

        var deviceType = DeviceType == ArcaneDeviceTypeEnum.view ? ArcaneDeviceType.view : ArcaneDeviceType.pad;
        // var arcaneInitParams = new ArcaneInitParams(deviceType, Port, ReverseProxyPort);

        Msg = new AWebSocketService<string>(arcaneInitParams);

        Action unsubscribeInit = null;
        unsubscribeInit = Msg.On(AEventName.Initialize, (InitializeEvent e, string from) => Initialize(e, unsubscribeInit));

        Msg.On(AEventName.RefreshGlobalState, (RefreshGlobalStateEvent e) => RefreshGlobalState(e.refreshedGlobalState));
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE 
        if (Msg != null && Msg.Ws != null)
        {
            Msg.Ws.DispatchMessageQueue();
        }
#endif
    }

    void OnDestroy()
    {
        Msg.Ws.Close();
    }

    void OnDisable()
    {
        Msg.Ws.Close();
    }

    private static void Initialize(InitializeEvent e, Action unsubscribeInit)
    {
        unsubscribeInit();

        RefreshGlobalState(e.globalState);

        var deviceType = Devices.FirstOrDefault(d => d.id == Msg.DeviceId).deviceType;

        if (deviceType == ArcaneDeviceType.pad) PadInitialization();
        if (deviceType == ArcaneDeviceType.view) ViewInitialization();

        var initialState = new InitialState(Pads);

        _arcaneClientInitialized.SetResult(initialState);

        // Msg.OnInitialize(e);
    }

    private static void PadInitialization()
    {
        Pad = Pads.FirstOrDefault(p => p.DeviceId == Msg.DeviceId);

        if (Pad == null)
        {
            Debug.LogError("Pad is null");
            return;
        }

        if (_arcaneInitParams.padOrientation == AOrientation.Landscape) Pad.SetScreenOrientationLandscape();
        if (_arcaneInitParams.padOrientation == AOrientation.Portrait) Pad.SetScreenOrientationPortrait();
    }

    private static void ViewInitialization()
    {
        if (_arcaneInitParams.hideMouse) Cursor.visible = false;
    }

    private static void RefreshGlobalState(GlobalState globalState)
    {
        Devices = globalState.devices;

        RefreshClientsIds(Devices);

        Pads = GetPads(Devices);
    }

    private static List<ArcanePad> GetPads(IList<ArcaneDevice> _devices)
    {
        var pads = new List<ArcanePad>();

        var padDevices = _devices.Where(device => device.deviceType == ArcaneDeviceType.pad).ToList();

        padDevices.ForEach(padDevice =>
        {
            var iframeClientId = padDevice.clients.FirstOrDefault(c => c.clientType == ArcaneClientType.iframe)?.id;
            var internalClientId = padDevice.clients.FirstOrDefault(c => c.clientType == ArcaneClientType.@internal)?.id;

            // if (string.IsNullOrEmpty(iframeClientId))
            // {
            //     Debug.LogError("Tried to set pad but iframeClientId was not found");
            // }

            // if (string.IsNullOrEmpty(internalClientId))
            // {
            //     Debug.LogError("Tried to set pad but internalClientId was not found");
            // }

            pads.Add(new ArcanePad(
                deviceId: padDevice.id,
                internalId: internalClientId,
                iframeId: iframeClientId,
                isConnected: true,
                user: padDevice.user
            ));
        });

        return pads;
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

    private static void RefreshClientsIds(IList<ArcaneDevice> _devices)
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

