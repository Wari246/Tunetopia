using UnityEngine;

public class PuzzlePieceScaler : MonoBehaviour
{
    public Transform partsArea;  // パーツエリアのTransformをドラッグアンドドロップで指定
    private Vector3 originalScale;

    void Start()
    {
        // 元のサイズを保存
        originalScale = transform.localScale;
    }

    void Update()
    {
        // パーツエリア内にパズルがある場合
        if (IsInPartsArea())
        {
            // パーツのサイズを0.8倍に変更
            transform.localScale = originalScale * 0.5f;
        }
        else
        {
            // パズルがエリア外にある場合は元のサイズに戻す
            transform.localScale = originalScale;
        }
    }

    // パーツがパーツエリア内にあるかどうかを判定するメソッド
    bool IsInPartsArea()
    {
        // ここでパーツがパーツエリア内にあるかどうかを判定
        // パーツエリアの範囲に入っているかを簡単に判定する例（必要に応じて調整）
        return partsArea != null && partsArea.GetComponent<Collider>().bounds.Contains(transform.position);
    }
}
