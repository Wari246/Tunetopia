using UnityEngine;
using UnityEngine.UI;

public class RightClickRotator : MonoBehaviour
{
    [SerializeField]
    private float rotationAngle = 45f;

    [HideInInspector]
    public bool isTemporarilyHeld = false;

    public Button toggleRotationAxisButton;
    public Text currentAxisLabel;

    private Vector3 currentAxis = Vector3.up; // 初期状態：Y軸（横回転）

    void Start()
    {
        // ボタンがあれば切り替え機能を追加
        if (toggleRotationAxisButton != null)
            toggleRotationAxisButton.onClick.AddListener(ToggleRotationAxis);
        else
            Debug.LogWarning("toggleRotationAxisButton が設定されていません。");

        UpdateAxisLabel();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isTemporarilyHeld)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform == transform)
                {
                    RotateObject();
                }
            }
        }
    }

    private void RotateObject()
    {
        transform.Rotate(currentAxis, rotationAngle, Space.Self);
    }

    private void ToggleRotationAxis()
    {
        if (currentAxis == Vector3.up)
            currentAxis = Vector3.right; // 横 → 縦
        else
            currentAxis = Vector3.up;    // 縦 → 横

        UpdateAxisLabel();
    }

    private void UpdateAxisLabel()
    {
        if (currentAxisLabel == null) return;

        if (currentAxis == Vector3.right)
            currentAxisLabel.text = "X軸（縦回転）";
        else if (currentAxis == Vector3.up)
            currentAxisLabel.text = "Y軸（横回転）";
    }
}
