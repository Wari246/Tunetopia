using UnityEngine;

public class SnapSocket : MonoBehaviour
{
    public string acceptablePlugID; // 許可されているPlugのID
    public float allowedAngleDifference = 10f; // 許容される回転角度の差

    private bool isFullySnapped = false; // 完全に接続された状態を管理

    // 完全に接続されたかどうかを確認するプロパティ
    public bool IsFullySnapped
    {
        get { return isFullySnapped; }
    }

    // 接続状態を設定するメソッド
    public void SetConnectionStatus(bool isConnected)
    {
        if (isFullySnapped != isConnected) // 接続状態が変わったときのみログを出力
        {
            isFullySnapped = isConnected;
            if (isFullySnapped)
            {
                Debug.Log(gameObject.name + " is fully snapped.");
            }
            else
            {
                Debug.Log(gameObject.name + " is not fully snapped.");
            }
        }
    }

    // SnapPlug（接続されるべきパーツ）との接続を検証するメソッド
    public bool TrySnap(SnapPlug plug)
    {
        // 接続IDが一致するか、回転角度が許容範囲内であるかを確認
        if (plug.acceptablePlugID == acceptablePlugID)
        {
            float angleDifference = Quaternion.Angle(plug.transform.rotation, transform.rotation);
            if (angleDifference <= allowedAngleDifference)
            {
                // 完全に接続された状態と判定
                SetConnectionStatus(true);
                return true; // 接続成功
            }
        }

        SetConnectionStatus(false);
        return false; // 接続失敗
    }
}
