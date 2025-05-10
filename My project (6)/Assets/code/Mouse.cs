using UnityEngine;

public class Mouse : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    public bool isDraggable = true;

    void OnMouseDown()
    {
        if (!isDraggable) return;

        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        mOffset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (!isDraggable) return;

        transform.position = GetMouseWorldPos() + mOffset;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
