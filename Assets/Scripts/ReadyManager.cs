using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReadyManager : NetworkBehaviour
{
    public static ReadyManager Instance;

    private NetworkVariable<int> readyCount = new NetworkVariable<int>(0);
    private NetworkVariable<int> playerCount = new NetworkVariable<int>(1); // Host mặc định = 1

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnNetworkSpawn()
    {
        readyCount.OnValueChanged += (old, newVal) => LobbyUI.Instance.UpdateReadyStatus(newVal, playerCount.Value);
        playerCount.OnValueChanged += (old, newVal) => LobbyUI.Instance.UpdateReadyStatus(readyCount.Value, newVal);

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
            {
                if (id != NetworkManager.Singleton.LocalClientId)
                    playerCount.Value++;
            };

            NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
            {
                playerCount.Value--;
            };
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void PlayerReadyServerRpc()
    {
        readyCount.Value++;

        // Nếu host muốn tự ready luôn thì uncomment dòng dưới
        // if (readyCount.Value >= playerCount.Value) ...
    }

    public bool CanStart() => readyCount.Value >= playerCount.Value && playerCount.Value > 1;

    public void StartGame()
    {
        if (IsServer)
            NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
}