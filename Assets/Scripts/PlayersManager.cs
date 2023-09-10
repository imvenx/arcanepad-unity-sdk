using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ArcanepadSDK.Models;
using ArcanepadSDK;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<PlayerController> players = new List<PlayerController>();
    public bool gameStarted { get; private set; }
    public static bool isGamePaused = false;
    async void Start()
    {
        var initialState = await Arcane.ArcaneClientInitialized();

        initialState.pads.ForEach(pad =>
        {
            createPlayer(pad);
        });

        Arcane.Msg.On(AEventName.OpenArcaneMenu, () =>
        {
            isGamePaused = true;
        });
        Arcane.Msg.On(AEventName.CloseArcaneMenu, () =>
        {
            isGamePaused = false;
        });

        Arcane.Msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) =>
        {
            var playerExists = players.Any(p => p.Pad.IframeId == e.iframeId);
            if (playerExists) return;

            var pad = new ArcanePad(deviceId: e.deviceId, internalId: e.internalId, iframeId: e.iframeId, isConnected: true,
            user: Arcane.Devices.FirstOrDefault(d => d.id == e.deviceId).user);

            createPlayer(pad);
        });

        Arcane.Msg.On(AEventName.IframePadDisconnect, (IframePadDisconnectEvent e) =>
        {
            var player = players.FirstOrDefault(p => p.Pad.IframeId == e.IframeId);

            if (player == null) Debug.LogError("Player not found to remove on disconnect");
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
        playerComponent.Pad.Dispose();
        players.Remove(playerComponent);
        Destroy(playerComponent.gameObject);
    }

}