using UnityEngine;

public class RightClickRotator : MonoBehaviour
{
    [SerializeField]
    private float rotationAngle = 45f;

    [SerializeField]
    private Vector3 rotationAxis = Vector3.up;

    [HideInInspector]
    public bool isTemporarilyHeld = false; // 完全接続されたら true に

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
        transform.Rotate(rotationAxis, rotationAngle, Space.Self);
    }
}
