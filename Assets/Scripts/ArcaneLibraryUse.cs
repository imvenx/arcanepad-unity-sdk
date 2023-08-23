using System.Collections;
using System.Collections.Generic;
using ArcanepadSDK.CustomModels;
using ArcanepadSDK.Models;
using Newtonsoft.Json;
using UnityEngine;

public class ArcaneLibraryUse : MonoBehaviour
{
    public static WebSocketService<string> wsService;

    void Awake()
    {
        string url = "wss://localhost:3005";

#if DEBUG || UNITY_EDITOR
        url = "ws://localhost:3009";
#endif

        wsService = new WebSocketService<string>(url, ArcaneDeviceType.view);

        // wsService.OnConnected += () =>
        // {
        //     Debug.Log("Client connected!");
        //     wsService.Emit(new AttackEvent(5), MessageDestinataries.views);
        // };

        // wsService.OnInitialized += () =>
        // {
        //     Debug.Log("Client initialized!");
        //     wsService.Emit(new AttackEvent(5), MessageDestinataries.views);
        // };

        wsService.On(AEventName.Initialize, (InitializeEvent e, string from) =>
        {
            Debug.Log("waaaa");
        });

        wsService.On(CustomEventNames.Attack, (AttackEvent e, string from) =>
        {
            Debug.Log($"Received attack with damage: {e.damage}");
            Debug.Log("From: " + from);
        });

        wsService.On(AEventName.ClientConnect, (ClientConnect e, string from) =>
        {
            Debug.Log($"client connected with id: {e.clientId}");
        });

        wsService.On(AEventName.ClientDisconnect, (ClientDisconnect e, string from) =>
        {
            Debug.Log($"client disconnected with id: {e.clientId}");
        });
    }

    void Update()
    {
#if UNITY_EDITOR
        if (wsService != null)
        {
            wsService.ws.DispatchMessageQueue();
        }
#endif
    }

    void Close()
    {
        wsService.ws.Close();
    }
}
