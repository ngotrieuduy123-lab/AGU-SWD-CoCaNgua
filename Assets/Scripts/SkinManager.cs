using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public static SkinManager Instance;

    public BoardSkin currentSkin;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Sprite GetPieceSprite(int playerIndex)
    {
        return currentSkin.pieceSprites[playerIndex];
    }

    public Sprite GetBoardSprite()
    {
        return currentSkin.boardSprite;
    }
}