using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Mouse : MonoBehaviour
{
    [Header("スナップ設定")]
    public float snapRange = 0.5f;
    public bool allowTemporaryHoldOnWrongSocket = true;

    [Header("効果音")]
    public AudioClip holdSound;
    public AudioClip insertSound;

    [Header("アニメーション設定")]
    public float insertAnimationDuration = 0.2f;

    private Vector3 mOffset;
    private float mZCoord;

    private bool isHeldTemporarily = false;
    private bool isFullyInserted = false;

    private Transform snapTarget = null;
    private SnapSocket socketInfo = null;
    private SnapPlug plugInfo = null;
    private RightClickRotator rotator = null;
    private AudioSource audioSource;

    void Start()
    {
        plugInfo = GetComponentInChildren<SnapPlug>();
        rotator = GetComponentInChildren<RightClickRotator>();
        audioSource = GetComponent<AudioSource>();
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
                        if (!isHeldTemporarily && holdSound != null && audioSource != null)
                        {
                            audioSource.PlayOneShot(holdSound);
                        }

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

    private void FullyInsert()
    {
        isFullyInserted = true;
        isHeldTemporarily = false;
        StartCoroutine(AnimateInsert());
    }

    private IEnumerator AnimateInsert()
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 pushDirWorld = transform.TransformDirection(plugInfo.pushDirection.normalized);
        Vector3 endPos = snapTarget.position + pushDirWorld * plugInfo.pushDistance;
        Quaternion endRot = snapTarget.rotation;

        float elapsed = 0f;
        while (elapsed < insertAnimationDuration)
        {
            float t = elapsed / insertAnimationDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        transform.rotation = endRot;

        socketInfo.SetConnectionStatus(true);

        if (rotator != null)
        {
            rotator.isTemporarilyHeld = true;
        }

        transform.SetParent(snapTarget);

        PuzzleProgressionController.Instance?.ReportPieceFullyInserted();

        if (insertSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(insertSound);
        }

        this.enabled = false;
    }
}
