using UnityEngine;

public class CanvasCameraChanger : MonoBehaviour
{
    public Canvas canvas;            // 対象のCanvas
    public Camera newCamera;        // 新しく設定したいカメラ

    void Start()
    {
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvas.worldCamera = newCamera;
        }
    }
}
