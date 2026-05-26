using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance;

    [Header("Panels")]
    public GameObject panelMenu;      // Panel chọn Host/Join
    public GameObject panelLobby;     // Panel phòng chờ

    [Header("Menu UI")]
    public Button btnHost;
    public Button btnJoin;
    public TMP_InputField inputCode;  // Nhập code để join

    [Header("Lobby UI")]
    public TMP_Text txtLobbyCode;     // Hiển thị code cho host
    public TMP_Text txtReadyStatus;   // "Sẵn sàng: 1/2"
    public Button btnReady;
    public Button btnStart;           // Chỉ host thấy
    public TMP_Text txtStatus;

    private bool hasReady = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        panelMenu.SetActive(true);
        panelLobby.SetActive(false);

        btnHost.onClick.AddListener(OnHostClicked);
        btnJoin.onClick.AddListener(OnJoinClicked);
        btnReady.onClick.AddListener(OnReadyClicked);
        btnStart.onClick.AddListener(OnStartClicked);
    }

    async void OnHostClicked()
    {
        txtStatus.text = "Đang tạo phòng...";
        try
        {
            Debug.Log("Bắt đầu tạo phòng...");
            Debug.Log("LobbyManager: " + LobbyManager.Instance);
            Debug.Log("NetworkManager: " + NetworkManager.Singleton);

            string code = await LobbyManager.Instance.CreateLobby();

            Debug.Log("Tạo phòng thành công, code: " + code);

            panelMenu.SetActive(false);
            panelLobby.SetActive(true);
            txtLobbyCode.text = "Code phòng: " + code;
            btnStart.gameObject.SetActive(true);
            btnStart.interactable = false;
            UpdateReadyStatus(0, 1);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi tạo phòng: " + e.Message);
            Debug.LogError("Stack trace: " + e.StackTrace);
            txtStatus.text = "Lỗi: " + e.Message;
        }
    }

    async void OnJoinClicked()
    {
        string code = inputCode.text.Trim().ToUpper();
        if (string.IsNullOrEmpty(code)) return;

        txtStatus.text = "Đang vào phòng...";
        try
        {
            await LobbyManager.Instance.JoinLobby(code);
            panelMenu.SetActive(false);
            panelLobby.SetActive(true);
            txtLobbyCode.text = "Đã vào phòng!";
            btnStart.gameObject.SetActive(false); // Client không thấy nút Start
            txtStatus.text = "Bấm Sẵn sàng khi bạn đã chuẩn bị!";
        }
        catch (System.Exception e)
        {
            txtStatus.text = "Lỗi: " + e.Message;
        }
    }

    void OnReadyClicked()
    {
        if (hasReady) return;
        hasReady = true;
        btnReady.interactable = false;
        ReadyManager.Instance.PlayerReadyServerRpc();
        txtStatus.text = "Đã sẵn sàng, chờ host bắt đầu...";
    }

    void OnStartClicked()
    {
        if (ReadyManager.Instance.CanStart())
            ReadyManager.Instance.StartGame();
    }

    public void UpdateReadyStatus(int ready, int total)
    {
        txtReadyStatus.text = $"Sẵn sàng: {ready}/{total}";

        // Chỉ cập nhật nút Start nếu là host
        if (btnStart.gameObject.activeSelf)
            btnStart.interactable = ReadyManager.Instance.CanStart();
    }

}