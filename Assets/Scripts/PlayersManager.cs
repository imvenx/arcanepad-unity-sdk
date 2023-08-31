using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ArcanepadSDK.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Unity.VisualScripting.Dependencies.NCalc;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public List<IframePad> iframePads = new List<IframePad>();

    async void Start()
    {
        await Arcane.ArcaneClientInitialized();

        InitPlayersManager();
    }

    void InitPlayersManager()
    {
        Debug.Log(JsonConvert.SerializeObject(Arcane.userPadsIds));

        Arcane.userPadsIds.ForEach(iframePadId =>
        {
            var device = Arcane.devices.FirstOrDefault(d => d.clients.Any(c => c.id == iframePadId));

            if (device == null)
            {
                Debug.LogError($"Device not found on disconnecting client for clientId: {iframePadId}");
                return;
            }

            var internalClient = device.clients.FirstOrDefault(c => c.clientType == "internal");

            if (internalClient == null)
            {
                Debug.LogError($"Internal Id not found for iframe pad with id: {iframePadId}");
                return;
            }

            var newIframePad = new IframePad(iframePadId, device.id, internalClient.id, true);
            iframePads.Add(newIframePad);
            CreatePlayer(newIframePad);
        });

        iframePads.ForEach(iframePad => Debug.Log(iframePad.ClientId));

        Arcane.msg.On(AEventName.IframePadConnect, (IframePadConnectEvent e) =>
        {
            var padAlreadyCreated = iframePads.Any(p => p.ClientId == e.IframeId);

            if (padAlreadyCreated) return;

            var newIframePad = new IframePad(e.IframeId, e.DeviceId, e.InternalClientId, true);
            CreatePlayer(newIframePad);
            iframePads.Add(newIframePad);

        });
    }

    void CreatePlayer(IframePad pad)
    {
        GameObject newPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        PlayerController playerComponent = newPlayer.GetComponent<PlayerController>();
        playerComponent.Initialize(pad);
    }
}
