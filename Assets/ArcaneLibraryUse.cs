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
            wsService.Emit(new AttackEvent(new string[] { "some-id" }));
        };

        wsService.OnInitialized += () =>
        {
            Debug.Log("Client initialized!");
            wsService.Emit(new AttackEvent(new string[] { "some-id" }));
        };

    }

    void Update()
    {
        if (wsService != null)
        {
            wsService.ws.DispatchMessageQueue();
        }
    }
}
