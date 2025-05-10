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
        if (isFullyInserted) return; // 完全に挿入されていたら何もしない

        if (isHeldTemporarily && Time.time < lockTime) return;

        if (!isHeldTemporarily)
        {
            CheckSnapSockets();
        }

        if (isHeldTemporarily && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(InsertToSocket());
        }

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

            // 仮止めとして一瞬固定（位置・回転）
            SnapToSocketPosition();

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

        // パーツが完全に挿入されたら動かないようにする
        parentTransform.GetComponent<Collider>().enabled = false; // 動かないようにするためにコライダーを無効化

        var mouse = parentTransform.GetComponent<Mouse>();
        if (mouse != null) mouse.enabled = false;

        Vector3 startPos = parentTransform.position;
        Quaternion startRot = parentTransform.rotation;

        // ワールド空間での押し込み方向
        Vector3 pushDirWorld = transform.TransformDirection(pushDirection.normalized);
        Vector3 endPos = startPos + pushDirWorld * pushDistance;
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
