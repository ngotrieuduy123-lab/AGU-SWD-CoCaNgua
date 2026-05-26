using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    async void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log("Đăng nhập thành công: " + AuthenticationService.Instance.PlayerId);
    }
}