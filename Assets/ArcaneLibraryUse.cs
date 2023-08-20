using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ArcaneLibraryUse : MonoBehaviour
{
    WebSocketService<string> wsService;

    void Start()
    {
        var url = "ws://localhost:3009";
        wsService = new WebSocketService<string>(url, ArcaneDeviceType.view);

        wsService.OnConnected += () =>
        {
            Debug.Log("Client connected!");
            wsService.Emit(new AttackEvent(5), new string[] { MessageDestinataries.views });
        };

        // wsService.OnInitialized += () =>
        // {
        //     Debug.Log("Client initialized!");
        //     wsService.Emit(new AttackEvent(), new string[] { MessageDestinataries.pads });
        // };

        wsService.On<AttackEvent>(CustomEventNames.Attack, (AttackEvent e, string from) =>
        {
            Debug.Log($"Received attack with damage: {e.damage}");
            Debug.Log("From: " + from);
        });
    }

    void Update()
    {
        if (wsService != null)
        {
            wsService.ws.DispatchMessageQueue();
        }
    }
}
