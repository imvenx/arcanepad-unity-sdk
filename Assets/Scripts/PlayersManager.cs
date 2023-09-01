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
    public List<PlayerController> players = new List<PlayerController>();
    public bool gameStarted { get; private set; }
    async void Start()
    {
        var initalPads = await Arcane.ArcaneClientInitialized();

        initalPads.ForEach(pad =>
        {
            createPlayer(pad);
        });

        // Arcane.Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) => CreatePlayerIfNotExist(e.Pad));

        /////// ON IFRAME PAD CONNECT => CREATE PLAYER IF NO EXIST
        /////// ON IFRAME PAD DISCONNECT => REMOVE PLAYER   

        Arcane.msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) =>
        {
            var playerExists = players.Any(p => p.pad.iframeId == e.IframeId);
            if (playerExists) return;

            var pad = new ArcanePad(deviceId: e.DeviceId, internalId: e.InternalClientId, iframeId: e.IframeId, isConnected: true);

            createPlayer(pad);
        });

        Arcane.msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) =>
        {
            var playerExists = players.Any(p => p.pad.iframeId == e.IframeId);
            if (playerExists) return;

            var pad = new ArcanePad(deviceId: e.DeviceId, internalId: e.InternalClientId, iframeId: e.IframeId, isConnected: true);

            createPlayer(pad);
        });

        Arcane.msg.On(AEventName.IframePadDisconnect, (IframePadDisconnectEvent e) =>
        {
            var player = players.FirstOrDefault(p => p.pad.iframeId == e.IframeId);

            destroyPlayer(player);
        });

    }

    void createPlayer(ArcanePad pad)
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController playerComponent = newPlayer.GetComponent<PlayerController>();
        playerComponent.Initialize(pad);

        players.Add(playerComponent);
    }

    void destroyPlayer(PlayerController playerComponent)
    {
        players.Remove(playerComponent);
        Destroy(playerComponent.gameObject);
    }

}