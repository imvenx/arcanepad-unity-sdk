using System.Collections;
using System.Collections.Generic;
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
            var atkMsg = new AttackEvent(new string[] { MessageDestinataries.views }, 6);
            wsService.Emit(atkMsg);
        };

        wsService.OnInitialized += () =>
        {
            Debug.Log("Client initialized!");
            wsService.Emit(new AttackEvent(new string[] { MessageDestinataries.pads }, 11));
        };

        // wsService.On(CustomEventNames.Attack, OnAttacked);
        wsService.On(CustomEventNames.Attack, (AttackEvent attackEvent) =>
        {
            Debug.Log($"Received Attack event: {JsonUtility.ToJson(attackEvent).ToString()}");
        });

        // wsService.On(CustomEventNames.Other, (AttackEvent a, string b) =>
        // {
        //     Debug.Log("other");
        // });
    }

    void Update()
    {
        if (wsService != null)
        {
            wsService.ws.DispatchMessageQueue();
        }
    }

    // private void OnAttacked(AttackEvent attackData)
    // {
    //     Debug.Log($"Received Attack event: {JsonUtility.ToJson(attackData).ToString()}");
    // }
}
