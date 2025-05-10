using UnityEngine;
using UnityEngine.UI;

public class RightClickRotator : MonoBehaviour
{
    [SerializeField]
    private float rotationAngle = 45f;

    [HideInInspector]
    public bool isTemporarilyHeld = false;

    private bool isSelected = false;

    public enum RotationAxis { X, Y, Z }
    public RotationAxis currentAxis = RotationAxis.Y;

    [Header("UI Setup")]
    public string switchAxisButtonName = "SwitchAxisButton"; // Hierarchy 上の名前
    public string axisTextName = "AxisText"; // 表示用 Text のオブジェクト名

    private Text axisDisplayText;

    void Start()
    {
        // UIボタンを名前で探してイベント登録
        GameObject btnObj = GameObject.Find(switchAxisButtonName);
        if (btnObj != null && btnObj.TryGetComponent<Button>(out Button switchButton))
        {
            switchButton.onClick.AddListener(SwitchAxis);
        }

        // Textを名前で探して取得
        GameObject textObj = GameObject.Find(axisTextName);
        if (textObj != null && textObj.TryGetComponent<Text>(out Text text))
        {
            axisDisplayText = text;
        }
    }

    void Update()
    {
        // 選択処理（左クリック）
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                isSelected = hit.transform == transform;
            }
            else
            {
                isSelected = false;
            }
        }

        // 回転処理（右クリック）
        if (Input.GetMouseButtonDown(1) && isSelected && !isTemporarilyHeld)
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
