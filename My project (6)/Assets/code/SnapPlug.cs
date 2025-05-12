using UnityEngine;
using System.Collections;

public class SnapPlug : MonoBehaviour
{
    public string plugID;
    public Vector3 pushDirection = Vector3.forward;
    public float pushDistance = 0.02f; // fallback距離
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
    private Mouse mouseScript;

    void Start()
    {
        audioSource = GetComponentInParent<AudioSource>();
        parentTransform = transform.parent;
        mouseScript = parentTransform.GetComponent<Mouse>();
    }

    void Update()
    {
        if (isFullyInserted) return;

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
            else
            {
                Vector3 plugToParent = transform.position - parentTransform.position;
                Vector3 targetParentPos = targetSocket.transform.position - plugToParent;

                parentTransform.position = Vector3.Lerp(parentTransform.position, targetParentPos, Time.deltaTime * 30f);
                parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, targetSocket.transform.rotation, Time.deltaTime * 30f);
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
        Vector3 plugToParent = transform.position - parentTransform.position;
        Vector3 targetParentPos = targetSocket.transform.position - plugToParent;

        parentTransform.position = targetParentPos;
        parentTransform.rotation = targetSocket.transform.rotation;
        hasSnappedToPosition = true;
    }

    IEnumerator InsertToSocket()
    {
        if (targetSocket == null || targetSocket.socketID != plugID)
        {
            Debug.Log("プラグIDとソケットIDが一致しません。挿入できません。");
            yield break;
        }

        isFullyInserted = true;
        isHeldTemporarily = false;
        targetSocket.SetConnectionStatus(true);

        Collider col = parentTransform.GetComponent<Collider>();
        if (col != null) col.enabled = false;
        if (mouseScript != null) mouseScript.enabled = false;

        Vector3 startPos = parentTransform.position;
        Quaternion startRot = parentTransform.rotation;

        Vector3 pushDirWorld = transform.TransformDirection(pushDirection.normalized);

        float finalPushDistance = pushDistance;

        if (targetSocket.insertionEndPoint != null)
        {
            Vector3 plugTip = transform.position;
            Vector3 endPoint = targetSocket.insertionEndPoint.position;

            float plugZLength = transform.lossyScale.z * 0.5f; // 必要に応じて調整
            finalPushDistance = Vector3.Distance(plugTip, endPoint) + plugZLength;
        }

        Vector3 endPos = startPos + pushDirWorld * finalPushDistance;
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
        if (PuzzleProgressionController.Instance != null)
        {
            PuzzleProgressionController.Instance.ReportPieceFullyInserted();
        }
    }
}
