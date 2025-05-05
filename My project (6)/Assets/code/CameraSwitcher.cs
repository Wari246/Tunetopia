using UnityEngine;
using UnityEngine.UI;

public class CameraSwitcher : MonoBehaviour
{
    public Camera[] cameras;         // 複数のカメラ
    public Canvas canvas;            // Screen Space - Camera の Canvas
    public Button switchButton;      // UIボタン

    private int currentIndex = 0;

    void Start()
    {
        if (switchButton != null)
        {
            switchButton.onClick.AddListener(SwitchCamera);
        }

        UpdateView();
    }

    public void SwitchCamera()
    {
        currentIndex = (currentIndex + 1) % cameras.Length;
        UpdateView();
    }

    private void UpdateView()
    {
        // すべてのカメラを非アクティブに
        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }

        // 現在のカメラだけ有効に
        Camera activeCamera = cameras[currentIndex];
        activeCamera.gameObject.SetActive(true);

        // Canvas を一度無効にしてから再設定 → 描画バグ対策
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            canvas.enabled = false; // 一旦切る
            canvas.worldCamera = activeCamera; // 新しいカメラを設定
            canvas.enabled = true;  // 再表示
        }
    }
}
