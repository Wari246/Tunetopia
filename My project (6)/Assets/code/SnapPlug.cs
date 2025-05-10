using UnityEngine;
using System.Collections;

public class SnapPlug : MonoBehaviour
{
    public string plugID;
    public Vector3 pushDirection = Vector3.forward; // 押し込む方向
    public float pushDistance = 0.02f;   // 押し込む距離
    public float snapRange = 0.5f;        // スナップ範囲
    public AudioClip holdSound;           // 仮止めのサウンド
    public AudioClip insertSound;         // 完全に挿入されたサウンド
    public float insertDuration = 0.2f;   // 完全に挿入されるアニメーションの時間

    private SnapSocket targetSocket;
    private bool isHeldTemporarily = false;   // 仮止め状態かどうか
    private bool isFullyInserted = false;     // 完全に挿入されたかどうか
    private float lockTime = 0f;              // 仮止め位置のロック時間

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

        // 仮止めロック時間中は動かさない
        if (isHeldTemporarily && Time.time < lockTime) return;

        // 仮止め可能な位置の確認
        CheckSnapSockets();

        // 左クリックで押し込む処理
        if (isHeldTemporarily && Input.GetMouseButtonDown(0)) // 左クリック
        {
            StartCoroutine(InsertToSocket());
        }
    }

    private void CheckSnapSockets()
    {
        GameObject[] sockets = GameObject.FindGameObjectsWithTag("SnapSocket");

        foreach (var s in sockets)
        {
            SnapSocket socket = s.GetComponent<SnapSocket>();
            if (socket == null || socket.IsFullySnapped) continue;

            float distance = Vector3.Distance(transform.position, socket.transform.position);
            if (distance > snapRange) continue;

            float angleDiff = Quaternion.Angle(transform.rotation, socket.transform.rotation);
            if (angleDiff > socket.allowedAngleDifference) continue;

            // 仮止め
            parentTransform.position = socket.transform.position;
            parentTransform.rotation = socket.transform.rotation;
            targetSocket = socket;
            isHeldTemporarily = true;
            lockTime = Time.time + 0.5f; // 仮止めロック時間（0.5秒）

            if (holdSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(holdSound);
            }

            break;
        }
    }

    private IEnumerator InsertToSocket()
    {
        isFullyInserted = true;
        isHeldTemporarily = false;
        targetSocket.SetConnectionStatus(true);

        // マウススクリプト無効化
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
        {
            audioSource.PlayOneShot(insertSound);
        }
    }
}
