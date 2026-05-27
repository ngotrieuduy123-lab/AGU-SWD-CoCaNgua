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
        PlayerIndex.OnValueChanged += OnPlayerIndexChanged;
        ApplySpriteWhenReady(PlayerIndex.Value);
    }

    public override void OnNetworkDespawn()
    {
        PlayerIndex.OnValueChanged -= OnPlayerIndexChanged;
    }

    void OnPlayerIndexChanged(int oldValue, int newValue)
    {
        ApplySpriteWhenReady(newValue);
    }

    public void SetPlayerIndex(int index)
    {
        if (!IsServer) return;

        PlayerIndex.Value = index;
        ApplySpriteWhenReady(index);
    }

    void ApplySpriteWhenReady(int index)
    {
        if (index < 0) return;
        StartCoroutine(ApplySpriteDelayed(index));
    }

    System.Collections.IEnumerator ApplySpriteDelayed(int index)
    {
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
}
