using UnityEngine;
using System.Collections;

public class SnapPlug : MonoBehaviour
{
    public string plugID;
    public Vector3 pushDirection = Vector3.forward;
    public float pushDistance = 0.02f;
    public float snapRange = 0.5f;
    public float autoSnapThreshold = 0.3f;
    public AudioClip holdSound;
    public AudioClip insertSound;
    public float insertDuration = 0.2f;

    private SnapSocket targetSocket;
    private bool isHeldTemporarily = false;
    private bool isFullyInserted = false;
    private bool hasSnappedToPosition = false;
    private float lockTime = 0f;

    private AudioSource audioSource;
    private Transform parentTransform;

    void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
        parentTransform = transform.parent;
    }

    void Update()
    {
        if (isFullyInserted) return;

        if (isHeldTemporarily && Time.time < lockTime) return;

        // まだ仮止めされてない → スナップ位置をチェック
        if (!isHeldTemporarily)
        {
            CheckSnapSockets();
        }

        // 仮止め後、左クリックで完全に挿入
        if (isHeldTemporarily && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(InsertToSocket());
        }

        // スナップ位置にまだ移動していない場合、範囲内で強制的に移動
        if (isHeldTemporarily && !hasSnappedToPosition && targetSocket != null)
        {
            float distance = Vector3.Distance(transform.position, targetSocket.transform.position);
            if (distance <= autoSnapThreshold)
            {
                SnapToSocketPosition();
            }
        }
    }

    void CheckSnapSockets()
    {
        GameObject[] sockets = GameObject.FindGameObjectsWithTag("SnapSocket");
        SnapSocket closestSocket = null;
        float closestDistance = snapRange;

        foreach (var s in sockets)
        {
            SnapSocket socket = s.GetComponent<SnapSocket>();
            if (socket == null || socket.IsFullySnapped) continue;

            float dist = Vector3.Distance(transform.position, socket.transform.position);
            if (dist > closestDistance) continue;

            float angle = Quaternion.Angle(transform.rotation, socket.transform.rotation);
            if (angle > socket.allowedAngleDifference) continue;

            closestSocket = socket;
            closestDistance = dist;
        }

        if (closestSocket != null)
        {
            targetSocket = closestSocket;
            isHeldTemporarily = true;
            lockTime = Time.time + 0.5f;

            if (holdSound != null && audioSource != null)
                audioSource.PlayOneShot(holdSound);
        }
    }

    void SnapToSocketPosition()
    {
        parentTransform.position = targetSocket.transform.position;
        parentTransform.rotation = targetSocket.transform.rotation;
        hasSnappedToPosition = true;
    }

    IEnumerator InsertToSocket()
    {
        isFullyInserted = true;
        isHeldTemporarily = false;
        targetSocket.SetConnectionStatus(true);

        var mouse = parentTransform.GetComponent<Mouse>();
        if (mouse != null) mouse.enabled = false;

        Vector3 startPos = parentTransform.position;
        Quaternion startRot = parentTransform.rotation;
        Vector3 pushDirWorld = transform.TransformDirection(pushDirection.normalized);
        Vector3 endPos = targetSocket.transform.position + pushDirWorld * pushDistance;
        Quaternion endRot = targetSocket.transform.rotation;

        float elapsed = 0f;
        while (elapsed < insertDuration)
        {
            float t = elapsed / insertDuration;
            parentTransform.position = Vector3.Lerp(startPos, endPos, t);
            parentTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        parentTransform.position = endPos;
        parentTransform.rotation = endRot;
        parentTransform.SetParent(targetSocket.transform);

        if (insertSound != null && audioSource != null)
            audioSource.PlayOneShot(insertSound);
    }
}
