using UnityEngine;
using UnityEngine.UI;

public class RightClickRotator : MonoBehaviour
{
    [SerializeField]
    private float rotationAngle = 45f;

    [HideInInspector]
    public bool isTemporarilyHeld = false;

    public Button xAxisButton;
    public Button yAxisButton;
    public Button zAxisButton;
    public Text currentAxisLabel;

    private Vector3 currentAxis = Vector3.up;

    void Start()
    {
        // ボタンがあればリスナーを設定
        if (xAxisButton != null) xAxisButton.onClick.AddListener(() => SetRotationAxis(Vector3.right));
        if (yAxisButton != null) yAxisButton.onClick.AddListener(() => SetRotationAxis(Vector3.up));
        if (zAxisButton != null) zAxisButton.onClick.AddListener(() => SetRotationAxis(Vector3.forward));

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

    private void SetRotationAxis(Vector3 newAxis)
    {
        currentAxis = newAxis;
        UpdateAxisLabel();
    }

    private void UpdateAxisLabel()
    {
        if (currentAxisLabel == null) return;

        if (currentAxis == Vector3.right)
            currentAxisLabel.text = "X軸で回転中";
        else if (currentAxis == Vector3.up)
            currentAxisLabel.text = "Y軸で回転中";
        else if (currentAxis == Vector3.forward)
            currentAxisLabel.text = "Z軸で回転中";
    }
}
