using UnityEngine;

public class MouseZoom : MonoBehaviour
{
    [Header("ズーム設定")]
    public float zoomSpeed = 5f;
    public float minZoom = 2f;
    public float maxZoom = 20f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            ZoomAtCursor(scroll);
        }
    }

    void ZoomAtCursor(float scrollDelta)
    {
        // マウスカーソル位置のワールド座標（ズーム前）
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldBefore = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, cam.nearClipPlane));

        // ズーム処理
        if (cam.orthographic)
        {
            cam.orthographicSize -= scrollDelta * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
        else
        {
            cam.fieldOfView -= scrollDelta * zoomSpeed * 10f;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);
        }

        // マウスカーソル位置のワールド座標（ズーム後）
        Vector3 mouseWorldAfter = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, cam.nearClipPlane));

        // ズームによって変化した分だけカメラを移動して補正
        Vector3 difference = mouseWorldBefore - mouseWorldAfter;
        cam.transform.position += difference;
    }
}
