using UnityEngine;

public class AxisMover : MonoBehaviour
{
    public enum MoveAxis { X, Y, Z }
    public MoveAxis moveAxis = MoveAxis.X;

    private bool dragging = false;
    private Vector3 startMousePos;
    private Vector3 startObjPos;

    void OnMouseDown()
    {
        dragging = true;
        startMousePos = Input.mousePosition;
        startObjPos = transform.parent.position; // 動かすのは親オブジェクト
    }

    void OnMouseDrag()
    {
        if (!dragging) return;

        Vector3 mouseDelta = Input.mousePosition - startMousePos;
        Vector3 worldAxis = moveAxis switch
        {
            MoveAxis.X => transform.right,
            MoveAxis.Y => transform.up,
            MoveAxis.Z => transform.forward,
            _ => Vector3.zero
        };

        float delta = Vector3.Dot(Camera.main.transform.right.normalized, worldAxis) * mouseDelta.x / 100f;
        transform.parent.position = startObjPos + worldAxis * delta;
    }

    void OnMouseUp()
    {
        dragging = false;
    }
}
