using UnityEngine;
using UnityEngine.UI;

public class RightClickRotator : MonoBehaviour
{
    [SerializeField]
    private float rotationAngle = 45f;

    [HideInInspector]
    public bool isTemporarilyHeld = false;

    public enum RotationAxis { X, Y, Z }
    public RotationAxis currentAxis = RotationAxis.Y;

    [Header("UI Auto-Setup")]
    public string switchAxisButtonName = "SwitchAxisButton";
    public string axisTextName = "AxisText";

    private Text axisDisplayText;
    private bool isMouseOver = false;

    void Start()
    {
        // UIボタンにイベント登録
        GameObject btnObj = GameObject.Find(switchAxisButtonName);
        if (btnObj != null && btnObj.TryGetComponent<Button>(out Button switchButton))
        {
            switchButton.onClick.AddListener(SwitchAxis);
        }

        // Text参照取得
        GameObject textObj = GameObject.Find(axisTextName);
        if (textObj != null && textObj.TryGetComponent<Text>(out Text text))
        {
            axisDisplayText = text;
        }
    }

    void Update()
    {
        // マウスがオブジェクトに当たっているか確認
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        isMouseOver = false;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                isMouseOver = true;
            }
        }

        // 右クリックで回転（触れていて・一時保持されていない時）
        if (Input.GetMouseButtonDown(1) && isMouseOver && !isTemporarilyHeld)
        {
            Rotate();
        }

        // UI表示更新
        if (axisDisplayText != null)
        {
            axisDisplayText.text = "Axis: " + currentAxis.ToString();
        }
    }

    private void Rotate()
    {
        Vector3 axis = GetAxisVector(currentAxis);
        transform.Rotate(axis, rotationAngle, Space.Self);
    }

    public void SwitchAxis()
    {
        currentAxis = (RotationAxis)(((int)currentAxis + 1) % 3);
    }

    private Vector3 GetAxisVector(RotationAxis axis)
    {
        switch (axis)
        {
            case RotationAxis.X: return Vector3.right;
            case RotationAxis.Y: return Vector3.up;
            case RotationAxis.Z: return Vector3.forward;
            default: return Vector3.zero;
        }
    }
}
