using UnityEngine;

[CreateAssetMenu(fileName = "BoardSkin", menuName = "CoCaNgua/Board Skin")]
public class BoardSkin : ScriptableObject
{
    public Sprite boardSprite;         // Ảnh bàn cờ
    public Sprite[] pieceSprites;      // 4 sprites - 1 màu mỗi sprite
                                       // [0] Cyan, [1] Vàng, [2] Đỏ, [3] Xanh lá
}