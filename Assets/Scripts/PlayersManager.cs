using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ArcanepadSDK.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Unity.VisualScripting.Dependencies.NCalc;
using ArcanepadSDK;
using System.Collections;
using System;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<PlayerController> Players = new List<PlayerController>();
    public bool GameStarted { get; private set; }
    async void Start()
    {
        var initalPads = await Arcane.ArcaneClientInitialized();

        initalPads.ForEach(pad =>
        {
            CreatePlayerIfNotExist(pad);
        });

        // Arcane.Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) => CreatePlayerIfNotExist(e.Pad));

        /////// ON IFRAME PAD CONNECT => CREATE PLAYER IF NO EXIST
        /////// ON IFRAME PAD DISCONNECT => REMOVE PLAYER 

        // StartCoroutine(EventTest());

        // Arcane.Msg.On("EventName1", () =>
        // {
        //     Console.WriteLine("EventName1 triggered.");
        // });

        // Arcane.Msg.On("EventName1", (dataDict, from) =>
        // {
        //     Console.WriteLine($"EventName1 triggered by {from}.");
        // });

        // // Event handling with Action<CustomEventType>
        // Arcane.Msg.On<ArcaneBaseEvent>("EventName2", (eventData, from) =>
        // {
        //     Console.WriteLine($"EventName2 triggered by {from} with data: {eventData}");
        // });

        // // Removing a specific callback for EventName1
        // Action<Dictionary<string, object>, string> myAction = (dataDict, from) => { Console.WriteLine($"EventName1 triggered by {from}."); };
        // Arcane.Msg.On("EventName1", () => { });
        // // Arcane.Msg.Off("EventName1", () => { });

        // // Removing all callbacks for EventName1
        // Arcane.Msg.Off("EventName1");
    }

    // private IEnumerator EventTest()
    // {
    //     var cb = asd();

    //     Arcane.Msg.On(AEventName.IframePadConnect, cb);

    //     Debug.Log("Event is ON");

    //     yield return new WaitForSeconds(3); // Wait 3 seconds

    //     Arcane.Msg.Off(AEventName.IframePadConnect, cb);

    //     Debug.Log("Event is OFF");
    // }


    // private Action<IframePadConnectEvent> asd()
    // {
    //     return (IframePadConnectEvent e) =>
    //     {
    //         Debug.Log("asdasd" + e.DeviceId);
    //     };
    // }

    /////////// START GAME () => OFF (IFRAME PAD CONNECT, CREATE PLAYER IF NO EXIST)  DO NOTHING ON CONNECT UNLESS IS A RECONNECT
    /////////// START GAME () => OFF (IFRAME PAD DOSCPMMECT, REMOVE PLAYER)  ADD PAUSE ON DISCONNECT  

    void CreatePlayerIfNotExist(ArcanePad pad)
    {
        var playerExists = Players.Any(p => p.Pad.IframeId == pad.IframeId);
        if (playerExists) return;

        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController playerComponent = newPlayer.GetComponent<PlayerController>();
        playerComponent.Initialize(pad);

        Players.Add(playerComponent);
    }
}


// var padExists = pads.Any(p => p.IframeId == e.IframeId);

// if (padExists) return; // Is a reconnection

// var newArcanePad =
// new ArcanePad(
//     deviceId: e.DeviceId,
//     internalId: e.InternalClientId,
//     iframeId: e.IframeId,
//     isConnected: true
// );
// CreatePlayer(newArcanePad);
// // Arcane.Pads.Add(newArcanePad);





// Debug.Log(JsonConvert.SerializeObject(Arcane.userPadsIds));

// Arcane.userPadsIds.ForEach(iframePadId =>
// {
//     var device = Arcane.devices.FirstOrDefault(d => d.clients.Any(c => c.id == iframePadId));

//     if (device == null)
//     {
//         Debug.LogError($"Device not found on disconnecting client for clientId: {iframePadId}");
//         return;
//     }

//     var internalClient = device.clients.FirstOrDefault(c => c.clientType == "internal");

//     if (internalClient == null)
//     {
//         Debug.LogError($"Internal Id not found for iframe pad with id: {iframePadId}");
//         return;
//     }

//     var newIframePad = new IframePad(iframePadId, device.id, internalClient.id, true);
//     iframePads.Add(newIframePad);
//     CreatePlayer(newIframePad);
// });

// iframePads.ForEach(iframePad => Debug.Log(iframePad.ClientId));

// Arcane.msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) =>
// {
//     var padAlreadyCreated = iframePads.Any(p => p.ClientId == e.IframeId);

//     if (padAlreadyCreated) return;

//     var newIframePad = new IframePad(e.IframeId, e.DeviceId, e.InternalClientId, true);
//     CreatePlayer(newIframePad);
//     iframePads.Add(newIframePad);

// });