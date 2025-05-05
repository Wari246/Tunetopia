using UnityEngine;
using UnityEngine.UI; // ← これを忘れずに！

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras;         // 4つのカメラをInspectorで設定
    public Button switchButton;      // UIボタンをInspectorで設定

    private int currentIndex = 0;

    void Start()
    {
        UpdateCamera();

        if (switchButton != null)
        {
            switchButton.onClick.AddListener(SwitchCamera);
        }
    }

    public void SwitchCamera()
    {
        currentIndex = (currentIndex + 1) % cameras.Length;
        UpdateCamera();
    }

    private void UpdateCamera()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(i == currentIndex);
        }
    }
}
