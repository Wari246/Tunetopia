using UnityEngine;

public class Mouse : MonoBehaviour
{
    [Header("スナップ設定")]
    public float snapRange = 0.5f;
    public bool allowTemporaryHoldOnWrongSocket = true;

    private Vector3 mOffset;
    private float mZCoord;

    private bool isHeldTemporarily = false;
    private bool isFullyInserted = false;

    private Transform snapTarget = null;
    private SnapSocket socketInfo = null;
    private SnapPlug plugInfo = null;
    private RightClickRotator rotator = null;

    void Start()
    {
        plugInfo = GetComponentInChildren<SnapPlug>();
        rotator = GetComponentInChildren<RightClickRotator>();
    }

    void OnMouseDown()
    {
        if (isHeldTemporarily && !isFullyInserted)
        {
            TryInsert();
            return;
        }

        if (isFullyInserted) return;

        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;
        mOffset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (isFullyInserted) return;

        transform.position = GetMouseWorldPos() + mOffset;
        CheckForTemporaryHold();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    /// <summary>
    /// 仮止めの判定処理
    /// </summary>
    private void CheckForTemporaryHold()
    {
        GameObject[] sockets = GameObject.FindGameObjectsWithTag("SnapSocket");

        foreach (GameObject s in sockets)
        {
            float distance = Vector3.Distance(transform.position, s.transform.position);
            if (distance < snapRange)
            {
                SnapSocket socket = s.GetComponent<SnapSocket>();
                if (socket == null || plugInfo == null) continue;
                if (socket.IsFullySnapped) continue;

                float angleDiff = Quaternion.Angle(transform.rotation, s.transform.rotation);
                bool rotationMatch = angleDiff <= socket.allowedAngleDifference;

                if (rotationMatch)
                {
                    bool idMatch = plugInfo.plugID == socket.acceptablePlugID;

                    if (idMatch || allowTemporaryHoldOnWrongSocket)
                    {
                        transform.position = s.transform.position;
                        transform.rotation = s.transform.rotation;

                        isHeldTemporarily = true;
                        snapTarget = s.transform;
                        socketInfo = socket;
                        return;
                    }
                }
            }
        }

        isHeldTemporarily = false;
        snapTarget = null;
        socketInfo = null;
    }

    /// <summary>
    /// 仮止め状態でクリックしたら完全に挿入する
    /// </summary>
    private void TryInsert()
    {
        if (socketInfo == null || plugInfo == null) return;

        bool idMatch = plugInfo.plugID == socketInfo.acceptablePlugID;

        if (idMatch)
        {
            FullyInsert();
        }
        else
        {
            Debug.Log("このソケットには合いません！");
        }
    }

    /// <summary>
    /// 完全にパーツを押し込む処理
    /// </summary>
    private void FullyInsert()
    {
        isFullyInserted = true;
        isHeldTemporarily = false;

        transform.position = snapTarget.position;
        transform.rotation = snapTarget.rotation;

        Vector3 pushDirWorld = transform.TransformDirection(plugInfo.pushDirection.normalized);
        transform.position += pushDirWorld * plugInfo.pushDistance;

        // SnapSocket 側に完全合体を通知
        socketInfo.SetConnectionStatus(true);

        // 回転スクリプトがあればロックする
        if (rotator != null)
        {
            rotator.isTemporarilyHeld = true;
        }

        // 親子化して固定
        transform.SetParent(snapTarget);

        // PuzzleProgressionController に通知して次のパーツを解放
        PuzzleProgressionController.Instance?.ReportPieceFullyInserted();


        // このスクリプトはもう不要
        this.enabled = false;
    }
}