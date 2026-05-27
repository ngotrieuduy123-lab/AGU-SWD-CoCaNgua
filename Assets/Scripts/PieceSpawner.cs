using Unity.Netcode;
using UnityEngine;

public class PieceSpawner : NetworkBehaviour
{
    public static PieceSpawner Instance;

    [Header("Prefab")]
    public GameObject piecePrefab;

    [Header("Skin")]
    public SkinManager skinManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            SpawnAllPieces();
    }

    void SpawnAllPieces()
    {
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        for (int p = 0; p < clients.Count; p++)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector3 pos = BoardManager.Instance.GetStablePosition(p, i);
                GameObject piece = Instantiate(piecePrefab, pos, Quaternion.identity);

                var netObj = piece.GetComponent<NetworkObject>();
                netObj.SpawnWithOwnership(clients[p].ClientId); // Spawn trước

                // Set index SAU khi spawn
                piece.GetComponent<PieceData>().SetPlayerIndex(p);
            }
        }
    }
}