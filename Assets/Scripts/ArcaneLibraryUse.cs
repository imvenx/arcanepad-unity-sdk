using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ArcaneLibraryUse : MonoBehaviour
{
    WebSocketService<string> wsService;

    void Start()
    {
        string url = "wss://localhost:3005";

#if DEBUG || UNITY_EDITOR
        url = "ws://localhost:3009";
#endif

        wsService = new WebSocketService<string>(url, ArcaneDeviceType.view);

        wsService.OnConnected += () =>
        {
            Debug.Log("Client connected!");
            wsService.Emit(new AttackEvent(5), MessageDestinataries.views);
        };

        // wsService.OnInitialized += () =>
        // {
        //     Debug.Log("Client initialized!");
        //     wsService.Emit(new AttackEvent(), new string[] { MessageDestinataries.pads });
        // };

        wsService.On(CustomEventNames.Attack, (AttackEvent e, string from) =>
        {
            Debug.Log($"Received attack with damage: {e.damage}");
            Debug.Log("From: " + from);
        });

        wsService.On(ArcaneEventsNames.OtherClientConnected, (OtherClientConnectedEvent e, string from) =>
      {
          Debug.Log($"Other client connected with id: {e.clientId}");
          Debug.Log("From: " + from);
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
}
