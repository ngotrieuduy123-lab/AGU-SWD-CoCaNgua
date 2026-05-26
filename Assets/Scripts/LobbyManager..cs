using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    private Lobby currentLobby;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public async System.Threading.Tasks.Task<string> CreateLobby()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
        string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        var options = new CreateLobbyOptions
        {
            IsPrivate = true,
            Data = new Dictionary<string, DataObject>
            {
                { "RelayCode", new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
            }
        };

        currentLobby = await LobbyService.Instance.CreateLobbyAsync("CoCaNgua", 4, options);

        var relayServerData = new RelayServerData(allocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        if (!NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.StartHost();

        return currentLobby.LobbyCode;
    }

    public async System.Threading.Tasks.Task JoinLobby(string lobbyCode)
    {
        currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

        string relayCode = currentLobby.Data["RelayCode"].Value;
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

        var relayServerData = new RelayServerData(joinAllocation, "dtls");
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        if (!NetworkManager.Singleton.IsListening)
            NetworkManager.Singleton.StartClient();
    }

    public string GetLobbyCode() => currentLobby?.LobbyCode;
}