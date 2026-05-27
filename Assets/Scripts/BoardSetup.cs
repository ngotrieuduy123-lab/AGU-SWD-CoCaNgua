using UnityEngine;
using UnityEngine.InputSystem;

public class BoardSetup : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;
            Debug.Log($"World Position: ({worldPos.x:F2}, {worldPos.y:F2})");
        }
    }
}