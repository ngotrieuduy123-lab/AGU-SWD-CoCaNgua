using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PieceSpawner : NetworkBehaviour
{
    public static PieceSpawner Instance;

    [Header("Prefab")]
    public GameObject piecePrefab;

    [Header("Skin")]
    public SkinManager skinManager;

    private readonly HashSet<ulong> clientsReadyForGameplay = new HashSet<ulong>();
    private bool hasSpawned;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            MarkClientReady(NetworkManager.Singleton.LocalClientId);
            StartCoroutine(SpawnAfterTimeoutIfNeeded());
            return;
        }

        GameplayReadyServerRpc();
    }

    public override void OnNetworkDespawn()
    {
        if (Instance == this)
            Instance = null;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (Instance == this)
            Instance = null;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void GameplayReadyServerRpc(RpcParams rpcParams = default)
    {
        MarkClientReady(rpcParams.Receive.SenderClientId);
    }

    void MarkClientReady(ulong clientId)
    {
        if (!IsServer) return;

        clientsReadyForGameplay.Add(clientId);
        TrySpawnAllPieces("clients-ready");
    }

    IEnumerator SpawnAfterTimeoutIfNeeded()
    {
        yield return new WaitForSeconds(5f);

        if (hasSpawned) yield break;

        Debug.LogWarning(
            $"Waiting for gameplay clients before spawning pieces. Ready clients: {clientsReadyForGameplay.Count}/{NetworkManager.Singleton.ConnectedClientsList.Count}");
    }

    void TrySpawnAllPieces(string reason)
    {
        if (hasSpawned) return;

        int connectedClientCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        if (clientsReadyForGameplay.Count < connectedClientCount)
            return;

        hasSpawned = true;
        SpawnAllPieces(reason);
    }

    void SpawnAllPieces(string reason)
    {
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        Debug.Log($"Spawning pieces ({reason}). Client count: {clients.Count}");

        for (int p = 0; p < clients.Count; p++)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 pos = BoardManager.Instance.GetStablePosition(p, i);
                GameObject piece = Instantiate(piecePrefab, pos, Quaternion.identity);

                var netObj = piece.GetComponent<NetworkObject>();
                netObj.Spawn();

                var pieceData = piece.GetComponent<PieceData>();
                pieceData.SetPlayerIndex(p);
            }
        }
    }
}
