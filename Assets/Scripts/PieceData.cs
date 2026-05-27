using Unity.Netcode;
using UnityEngine;

public class PieceData : NetworkBehaviour
{
    public NetworkVariable<int> PlayerIndex = new NetworkVariable<int>(
        -1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        PlayerIndex.OnValueChanged += (old, newVal) => StartCoroutine(ApplySpriteDelayed(newVal));

        if (PlayerIndex.Value >= 0)
            StartCoroutine(ApplySpriteDelayed(PlayerIndex.Value));
    }

    System.Collections.IEnumerator ApplySpriteDelayed(int index)
    {
        // Chờ SkinManager sẵn sàng
        yield return new WaitUntil(() => SkinManager.Instance != null);
        ApplySprite(index);
    }

    void ApplySprite(int index)
    {
        if (index < 0) return;
        if (SkinManager.Instance == null) return;
        Sprite s = SkinManager.Instance.GetPieceSprite(index);
        if (sr != null && s != null)
            sr.sprite = s;
    }

    public void SetPlayerIndex(int index)
    {
        if (!IsServer) return;
        PlayerIndex.Value = index;
        ApplySpriteClientRpc(index);
    }

    [ClientRpc]
    void ApplySpriteClientRpc(int index)
    {
        StartCoroutine(ApplySpriteDelayed(index));
    }
}