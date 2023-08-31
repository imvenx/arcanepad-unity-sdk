using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArcanepadSDK;
using ArcanepadSDK.CustomModels;
using ArcanepadSDK.Models;
using Newtonsoft.Json;
using UnityEngine;

public class Arcane : MonoBehaviour
{
    public static WebSocketService<string> msg;
    public static IList<ArcaneDevice> devices;
    public static ArcanePad pad;
    private static List<string> internalViewsIds = new List<string>();
    private static List<string> internalPadsIds = new List<string>();
    public static List<string> userViewsIds = new List<string>();
    public static List<string> userPadsIds = new List<string>();
    private static TaskCompletionSource<ArcaneClientInitializeEvent> arcaneClientInitialized = new TaskCompletionSource<ArcaneClientInitializeEvent>();

    public static Task<ArcaneClientInitializeEvent> ArcaneClientInitialized()
    {
        return arcaneClientInitialized.Task;
    }

    void Awake()
    {
        string url = "wss://localhost:3005";

#if DEBUG || UNITY_EDITOR
        url = "ws://localhost:3009";
#endif

        devices = new List<ArcaneDevice>();
        msg = new WebSocketService<string>(url, ArcaneDeviceType.view);
        pad = new ArcanePad(msg);


        msg.On(AEventName.Initialize, (ArcaneClientInitializeEvent e, string from) =>
        {
            Debug.Log("Client initialized");
            devices = e.globalState.devices;
            // clients.Add(new ArcaneClient(e.assignedClientId,))
            arcaneClientInitialized.SetResult(e);
        });

        msg.On(AEventName.RefreshGlobalState, (RefreshGlobalStateEvent e, string from) =>
        {
            devices = e.refreshedGlobalState.devices;

            internalPadsIds = devices.Where(device => device.deviceType == ArcaneDeviceType.pad).SelectMany(device => device.clients
                .Where(client => client.clientType == ArcaneClientType.@internal).Select(client => client.id)).ToList();

            internalViewsIds = devices.Where(device => device.deviceType == ArcaneDeviceType.view).SelectMany(device => device.clients
                .Where(client => client.clientType == ArcaneClientType.@internal).Select(client => client.id)).ToList();

            userPadsIds = devices.Where(device => device.deviceType == ArcaneDeviceType.pad).SelectMany(device => device.clients
                .Where(client => client.clientType != ArcaneClientType.@internal).Select(client => client.id)).ToList();

            userViewsIds = devices.Where(device => device.deviceType == ArcaneDeviceType.view).SelectMany(device => device.clients
                .Where(client => client.clientType != ArcaneClientType.@internal).Select(client => client.id)).ToList();

        });

        msg.On(CustomEventNames.Attack, (AttackEvent e, string from) =>
        {
            Debug.Log($"Received attack with damage: {e.damage}");
            Debug.Log("From: " + from);
        });

        // wsService.On(AEventName.ClientConnect, (ClientConnect e, string from) =>
        // {
        //     Debug.Log($"client connected with id: {e.clientId}");
        // });

        // wsService.On(AEventName.ClientDisconnect, (ClientDisconnect e, string from) =>
        // {
        //     Debug.Log($"client disconnected with id: {e.clientId}");
        // });
    }

    void Start()
    {
        // Mimic setInterval with an infinite loop and a delay
        // Task.Run(async () =>
        // {
        //     while (true)
        //     {
        //         // Equivalent of Arcane.msg.emit(new StartGetRotationVectorEvent(), Arcane.internalPadsIds)
        //         msg.Emit(new StartGetRotationVectorEvent(), internalPadsIds);

        //         // Equivalent of setTimeout
        //         await Task.Delay(2000);

        //         // Equivalent of Arcane.msg.emit(new StopGetRotationVectorEvent(), Arcane.internalPadsIds)
        //         msg.Emit(new StopGetRotationVectorEvent(), internalPadsIds);

        //         // Wait for 2000ms before the next iteration (making it 4000ms total as in the original code)
        //         await Task.Delay(2000);
        //     }
        // });

        // msg.On(AEventName.GetRotationVector, (GetRotationVectorEvent e, string from) =>
        // {
        //     Debug.Log(e.x + " | " + e.y + " | " + e.z);
        // });
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

}
