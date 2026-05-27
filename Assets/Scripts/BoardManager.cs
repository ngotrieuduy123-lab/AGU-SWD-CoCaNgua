using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Chuồng [playerIndex][pieceIndex]
    // Thứ tự: Cyan(0), Đỏ(1), Vàng(2), Xanh lá(3)
    public static readonly Vector3[][] StablePositions = new Vector3[][]
    {
        // Cyan - góc trên trái
        new Vector3[] {
            new Vector3(-7.12f, 3.53f, 0), new Vector3(-5.50f, 3.56f, 0),
            new Vector3(-7.40f, 2.62f, 0), new Vector3(-5.68f, 2.57f, 0)
        },
        // Đỏ - góc dưới trái
        new Vector3[] {
            new Vector3(-7.44f, -2.19f, 0), new Vector3(-5.44f, -2.15f, 0),
            new Vector3(-7.47f, -3.52f, 0), new Vector3(-5.57f, -3.59f, 0)
        },
        // Vàng - góc trên phải
        new Vector3[] {
            new Vector3(4.93f, 3.62f, 0), new Vector3(6.88f, 3.64f, 0),
            new Vector3(4.98f, 2.24f, 0), new Vector3(7.27f, 2.25f, 0)
        },
        // Xanh lá - góc dưới phải
        new Vector3[] {
            new Vector3(4.41f, -2.10f, 0), new Vector3(7.07f, -2.06f, 0),
            new Vector3(4.35f, -3.59f, 0), new Vector3(7.10f, -3.50f, 0)
        }
    };

    // Vòng ngoài 56 ô - bắt đầu từ xuất phát Cyan, chiều kim đồng hồ
    public static readonly Vector3[] BoardPath = new Vector3[]
    {
        new Vector3(-1.20f, 4.61f, 0),  // 0  - Xuất phát Cyan
        new Vector3(-1.33f, 3.88f, 0),  // 1
        new Vector3(-1.31f, 3.34f, 0),  // 2
        new Vector3(-1.31f, 2.64f, 0),  // 3
        new Vector3(-1.31f, 1.98f, 0),  // 4
        new Vector3(-1.31f, 1.35f, 0),  // 5
        new Vector3(-1.31f, 0.67f, 0),  // 6
        new Vector3(-2.55f, 0.69f, 0),  // 7
        new Vector3(-4.00f, 0.69f, 0),  // 8
        new Vector3(-5.42f, 0.71f, 0),  // 9
        new Vector3(-6.72f, 0.71f, 0),  // 10
        new Vector3(-7.82f, 0.74f, 0),  // 11
        new Vector3(-9.26f, 0.72f, 0),  // 12
        new Vector3(-9.26f, -0.05f, 0), // 13
        new Vector3(-9.26f, -0.66f, 0), // 14 - Xuất phát Đỏ
        new Vector3(-8.03f, -0.81f, 0), // 15
        new Vector3(-6.62f, -0.66f, 0), // 16
        new Vector3(-5.39f, -0.66f, 0), // 17
        new Vector3(-3.99f, -0.64f, 0), // 18
        new Vector3(-2.64f, -0.64f, 0), // 19
        new Vector3(-1.37f, -0.71f, 0), // 20
        new Vector3(-1.35f, -1.34f, 0), // 21
        new Vector3(-1.35f, -1.97f, 0), // 22
        new Vector3(-1.35f, -2.61f, 0), // 23
        new Vector3(-1.37f, -3.24f, 0), // 24
        new Vector3(-1.38f, -3.83f, 0), // 25
        new Vector3(-1.42f, -4.59f, 0), // 26
        new Vector3(-0.09f, -4.66f, 0), // 27
        new Vector3( 1.33f, -4.62f, 0), // 28 - Xuất phát Xanh lá
        new Vector3( 1.33f, -3.90f, 0), // 29
        new Vector3( 1.35f, -3.22f, 0), // 30
        new Vector3( 1.31f, -2.58f, 0), // 31
        new Vector3( 1.29f, -1.89f, 0), // 32
        new Vector3( 1.29f, -1.27f, 0), // 33
        new Vector3( 1.29f, -0.62f, 0), // 34
        new Vector3( 2.56f, -0.62f, 0), // 35
        new Vector3( 3.89f, -0.70f, 0), // 36
        new Vector3( 5.20f, -0.68f, 0), // 37
        new Vector3( 6.48f, -0.68f, 0), // 38
        new Vector3( 7.88f, -0.60f, 0), // 39
        new Vector3( 9.15f, -0.57f, 0), // 40
        new Vector3( 9.23f,  0.06f, 0), // 41
        new Vector3( 9.24f,  0.65f, 0), // 42 - Xuất phát Vàng
        new Vector3( 7.92f,  0.72f, 0), // 43
        new Vector3( 6.61f,  0.69f, 0), // 44
        new Vector3( 5.17f,  0.71f, 0), // 45
        new Vector3( 3.99f,  0.67f, 0), // 46
        new Vector3( 2.58f,  0.72f, 0), // 47
        new Vector3( 1.33f,  0.61f, 0), // 48
        new Vector3( 1.25f,  1.33f, 0), // 49
        new Vector3( 1.33f,  1.98f, 0), // 50
        new Vector3( 1.31f,  2.60f, 0), // 51
        new Vector3( 1.31f,  3.23f, 0), // 52
        new Vector3( 1.25f,  3.95f, 0), // 53
        new Vector3( 1.29f,  4.54f, 0), // 54
        new Vector3( 0.04f,  4.63f, 0), // 55
    };

    // Đường về đích [playerIndex][0-5] = ô 1 đến 6
    public static readonly Vector3[][] HomePath = new Vector3[][]
    {
        // Cyan - từ ô 1 xuống giữa
        new Vector3[] {
            new Vector3(-0.09f,  4.04f, 0),
            new Vector3(-0.06f,  3.29f, 0),
            new Vector3(-0.04f,  2.68f, 0),
            new Vector3(-0.04f,  2.01f, 0),
            new Vector3(-0.04f,  1.37f, 0),
            new Vector3( 0.00f,  0.76f, 0)
        },
        // Đỏ - từ ô 1 vào giữa từ trái
        new Vector3[] {
            new Vector3(-7.86f,  0.04f, 0),
            new Vector3(-6.68f,  0.06f, 0),
            new Vector3(-5.35f,  0.13f, 0),
            new Vector3(-4.13f, -0.05f, 0),
            new Vector3(-2.56f,  0.00f, 0),
            new Vector3(-1.37f, -0.05f, 0)
        },
        // Xanh lá - từ ô 1 lên giữa
        new Vector3[] {
            new Vector3(-0.06f, -3.79f, 0),
            new Vector3(-0.13f, -3.29f, 0),
            new Vector3(-0.04f, -2.59f, 0),
            new Vector3(-0.02f, -1.97f, 0),
            new Vector3( 0.00f, -1.30f, 0),
            new Vector3( 0.00f, -0.66f, 0)
        },
        // Vàng - từ ô 1 vào giữa từ phải
        new Vector3[] {
            new Vector3(7.86f, -0.07f, 0),
            new Vector3(6.57f,  0.02f, 0),
            new Vector3(5.20f,  0.06f, 0),
            new Vector3(4.06f,  0.04f, 0),
            new Vector3(2.47f,  0.04f, 0),
            new Vector3(1.42f,  0.02f, 0)
        }
    };

    // Ô xuất phát của từng màu trên vòng ngoài
    public static readonly int[] StartIndex = new int[] { 0, 14, 28, 42 };

    // Lấy vị trí chuồng
    public Vector3 GetStablePosition(int playerIndex, int pieceIndex)
        => StablePositions[playerIndex][pieceIndex];

    // Lấy vị trí vòng ngoài
    public Vector3 GetBoardPosition(int pathIndex)
        => BoardPath[pathIndex % 56];

    // Lấy vị trí đường về đích
    public Vector3 GetHomePosition(int playerIndex, int step)
        => HomePath[playerIndex][step]; // step 0-5
}