using System.Collections.Generic;
using System.Linq;
using ArcanepadSDK.CustomModels;
using ArcanepadSDK.Models;
using Newtonsoft.Json;
using UnityEngine;

public class Arcane : MonoBehaviour
{
    public static WebSocketService<string> msg;
    private IList<ArcaneDevice> devices;
    private IList<string> internalViewsIds;
    private IList<string> internalPadsIds;
    public IList<string> userViewsIds;
    public IList<string> userPadsIds;

    void Awake()
    {
        devices = new List<ArcaneDevice>();

        string url = "wss://localhost:3005";

#if DEBUG || UNITY_EDITOR
        url = "ws://localhost:3009";
#endif

        msg = new WebSocketService<string>(url, ArcaneDeviceType.view);

        msg.On(AEventName.Initialize, (InitializeEvent e, string from) =>
        {
            Debug.Log("Client initialized");
            // clients.Add(new ArcaneClient(e.assignedClientId,))

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
