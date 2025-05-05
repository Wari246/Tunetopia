using UnityEngine;
using UnityEngine.UI;

public class MoveModeToggler : MonoBehaviour
{
    public GameObject gizmoHandles;         // Gizmoの親（XYZハンドル）
    public Button moveModeButton;           // UIのMoveボタン

    private bool isMoveMode = false;

    void Start()
    {
        // ボタンクリック時に ToggleMoveMode() を呼ぶように登録
        moveModeButton.onClick.AddListener(ToggleMoveMode);

        // 最初はハンドル非表示
        gizmoHandles.SetActive(false);
    }

    public void ToggleMoveMode()
    {
        isMoveMode = !isMoveMode;
        gizmoHandles.SetActive(isMoveMode);
    }
}
