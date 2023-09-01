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
            CreatePlayer(pad);
        });

        // Arcane.Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) => CreatePlayerIfNotExist(e.Pad));

        /////// ON IFRAME PAD CONNECT => CREATE PLAYER IF NO EXIST
        /////// ON IFRAME PAD DISCONNECT => REMOVE PLAYER   

        Arcane.Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) =>
        {
            var playerExists = Players.Any(p => p.Pad.IframeId == e.IframeId);
            if (playerExists) return;

            var pad = new ArcanePad(deviceId: e.DeviceId, internalId: e.InternalClientId, iframeId: e.IframeId, isConnected: true);

            CreatePlayer(pad);
        });

        Arcane.Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) =>
        {
            var playerExists = Players.Any(p => p.Pad.IframeId == e.IframeId);
            if (playerExists) return;

            var pad = new ArcanePad(deviceId: e.DeviceId, internalId: e.InternalClientId, iframeId: e.IframeId, isConnected: true);

            CreatePlayer(pad);
        });

        Arcane.Msg.On(AEventName.IframePadDisconnect, (IframePadDisconnectEvent e) =>
        {
            var player = Players.FirstOrDefault(p => p.Pad.IframeId == e.IframeId);

            DestroyPlayer(player);
        });

    }

    void CreatePlayer(ArcanePad pad)
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController playerComponent = newPlayer.GetComponent<PlayerController>();
        playerComponent.Initialize(pad);

        Players.Add(playerComponent);
    }

    void DestroyPlayer(PlayerController playerComponent)
    {
        Players.Remove(playerComponent);
        Destroy(playerComponent.gameObject);
    }

}