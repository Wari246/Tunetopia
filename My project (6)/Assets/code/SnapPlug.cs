using UnityEngine;

public class SnapPlug : MonoBehaviour
{
    public string acceptablePlugID;
    public string plugID;                // 例: "TabA"
    public Vector3 pushDirection = Vector3.forward; // 差し込む方向（ローカル軸）
    public float pushDistance = 0.02f;   // 押し込む距離

    private RightClickRotator rotator;

    void Start()
    {
        // 一度だけ RightClickRotator を取得
        rotator = GetComponent<RightClickRotator>();
    }

    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);

        foreach (var collider in colliders)
        {
            SnapSocket socket = collider.GetComponent<SnapSocket>();
            if (socket != null)
            {
                bool snapped = socket.TrySnap(this);
                if (snapped)
                {
                    // 仮止めされた場合は回転を無効化
                    if (rotator != null)
                    {
                        rotator.isTemporarilyHeld = true;
                    }
                }
            }
        }
    }

    // 仮止めを解除するときに呼び出す想定のメソッド（必要に応じて）
    public void ReleaseTemporaryHold()
    {
        if (rotator != null)
        {
            rotator.isTemporarilyHeld = false;
        }
    }
}
