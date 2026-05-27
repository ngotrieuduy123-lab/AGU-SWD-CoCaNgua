using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyManager : NetworkBehaviour
{
    public static ReadyManager Instance;

    private readonly HashSet<ulong> readyClients = new HashSet<ulong>();
    private NetworkVariable<int> readyCount = new NetworkVariable<int>(0);
    private NetworkVariable<int> playerCount = new NetworkVariable<int>(1);

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        readyCount.OnValueChanged += OnReadyCountChanged;
        playerCount.OnValueChanged += OnPlayerCountChanged;

        if (IsServer)
        {
            SyncPlayerCount();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        UpdateLobbyReadyStatus();
    }

    public override void OnNetworkDespawn()
    {
        readyCount.OnValueChanged -= OnReadyCountChanged;
        playerCount.OnValueChanged -= OnPlayerCountChanged;

        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }

        if (Instance == this)
            Instance = null;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (Instance == this)
            Instance = null;
    }

    void OnReadyCountChanged(int oldValue, int newValue)
    {
        UpdateLobbyReadyStatus();
    }

    void OnPlayerCountChanged(int oldValue, int newValue)
    {
        UpdateLobbyReadyStatus();
    }

    void UpdateLobbyReadyStatus()
    {
        if (LobbyUI.Instance == null) return;
        LobbyUI.Instance.UpdateReadyStatus(readyCount.Value, playerCount.Value);
    }

    void OnClientConnected(ulong clientId)
    {
        SyncPlayerCount();
    }

    void OnClientDisconnected(ulong clientId)
    {
        if (readyClients.Remove(clientId))
            readyCount.Value = readyClients.Count;

        SyncPlayerCount();
    }

    void SyncPlayerCount()
    {
        playerCount.Value = NetworkManager.Singleton.ConnectedClientsList.Count;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlayerReadyServerRpc(RpcParams rpcParams = default)
    {
        ulong senderClientId = rpcParams.Receive.SenderClientId;

        if (!readyClients.Add(senderClientId))
            return;

        readyCount.Value = readyClients.Count;
    }

    public bool CanStart()
    {
        return IsServer && readyCount.Value >= playerCount.Value && playerCount.Value > 1;
    }

    public void StartGame()
    {
        if (!CanStart()) return;
        NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
}
