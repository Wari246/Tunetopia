using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour
{
    public List<Transform> snapPoints; // 仮止めする目標位置リスト
    public List<Transform> finalPositions; // 完全にはめ込む最終位置リスト
    public List<PuzzlePiece> correctPieces; // 各スナップポイントに対応する正解のピース

    public float snapThreshold = 0.5f; // 仮止めの許容範囲
    public float snapAdjustSpeed = 0.1f; // 仮止め調整のスピード
    public float snapVibrationIntensity = 0.05f; // 仮止め時の振動の強さ
    public float snapVibrationDuration = 0.2f; // 仮止め時の振動時間

    private bool isSnapped = false;
    private bool isPushed = false;
    private Transform currentSnapPoint;
    private Transform currentFinalPosition;
    private PuzzlePiece correctPiece;
    private Transform rootPiece;
    private bool isDragging = false;
    private Vector3 offset;

    private static PuzzlePiece draggedPiece = null; // 現在ドラッグされているピース（クラス変数）

    void Start()
    {
        rootPiece = transform; // 初期状態では自分自身が親
    }

    void Update()
    {
        // ドラッグしているピースのみ仮止めの処理を行う
        if (!isSnapped && draggedPiece == this)
        {
            FindClosestSnapPoint(); // 一番近いスナップポイントを探す
        }

        if (isSnapped && !isPushed && Input.GetMouseButtonDown(0))
        {
            TryPushIntoPlace(); // 正しい位置なら押し込む
        }

        if (isDragging)
        {
            DragPiece(); // ドラッグ処理
        }
    }

    /// <summary>
    /// 最も近いスナップポイントを探し、位置を調整
    /// </summary>
    void FindClosestSnapPoint()
    {
        if (draggedPiece != this) return; // ドラッグしていないピースは処理しない

        float minDistance = Mathf.Infinity;
        Transform bestSnapPoint = null;
        Transform bestFinalPosition = null;
        PuzzlePiece bestCorrectPiece = null;

        for (int i = 0; i < snapPoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, snapPoints[i].position);
            if (distance < minDistance && distance <= snapThreshold)
            {
                minDistance = distance;
                bestSnapPoint = snapPoints[i];
                bestFinalPosition = finalPositions[i]; // 対応する最終位置を取得
                bestCorrectPiece = correctPieces[i]; // 対応する正解のピース
            }
        }

        if (bestSnapPoint != null)
        {
            transform.position = bestSnapPoint.position;
            isSnapped = true;
            currentSnapPoint = bestSnapPoint;
            currentFinalPosition = bestFinalPosition;
            correctPiece = bestCorrectPiece;

            // 仮止めの感を出すために微調整
            StartCoroutine(SnapAdjustment());
        }
    }

    /// <summary>
    /// 仮止めの位置調整と振動を行う
    /// </summary>
    IEnumerator SnapAdjustment()
    {
        Vector3 originalPosition = transform.position;

        // 仮止めされた位置で微調整し、少し振動を加える
        float elapsedTime = 0f;
        while (elapsedTime < snapVibrationDuration)
        {
            transform.position = originalPosition + new Vector3(
                Mathf.Sin(elapsedTime * 10f) * snapVibrationIntensity, // 振動のX軸
                Mathf.Cos(elapsedTime * 10f) * snapVibrationIntensity, // 振動のY軸
                0
            );
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 微調整を終了
        while (transform.position != currentSnapPoint.position)
        {
            transform.position = Vector3.Lerp(transform.position, currentSnapPoint.position, snapAdjustSpeed);
            yield return null;
        }
    }

    /// <summary>
    /// 正しい位置に押し込む
    /// </summary>
    void TryPushIntoPlace()
    {
        if (draggedPiece != this) return; // ドラッグしていないピースは押し込まない

        if (correctPiece == this) // 正しいピースなら押し込む
        {
            PushIntoPlace(); // アニメーションを削除し、直接位置を更新
        }
        else
        {
            Debug.Log("このピースは正しくありません！");
        }
    }

    /// <summary>
    /// 完全に押し込む（アニメーションなし）
    /// </summary>
    void PushIntoPlace()
    {
        isPushed = true;
        transform.position = currentFinalPosition.position; // 直接最終位置に移動
        MergeWithConnectedPiece();
    }

    /// <summary>
    /// 組み合わせが完了したら親を統一
    /// </summary>
    void MergeWithConnectedPiece()
    {
        Transform newRoot = currentSnapPoint.parent; // 既にあるピースの親を取得

        if (newRoot != null && newRoot != rootPiece)
        {
            // すべての子オブジェクトを新しい親の下に移動
            foreach (Transform child in transform)
            {
                child.SetParent(newRoot);
            }
            transform.SetParent(newRoot);
            rootPiece = newRoot; // ルートを更新
        }
    }

    /// <summary>
    /// ドラッグ処理を開始
    /// </summary>
    void OnMouseDown()
    {
        if (draggedPiece != null) return; // 他のピースがドラッグされていれば動かさない
        draggedPiece = this; // 現在のピースをドラッグ対象に
        StartDragging();
    }

    /// <summary>
    /// ドラッグ処理を終了
    /// </summary>
    void OnMouseUp()
    {
        if (draggedPiece == this)
        {
            draggedPiece = null; // ドラッグが終了したらリセット
        }
        isDragging = false;
    }

    /// <summary>
    /// ドラッグの開始
    /// </summary>
    void StartDragging()
    {
        isDragging = true;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(rootPiece.position).z;
        offset = rootPiece.position - Camera.main.ScreenToWorldPoint(mousePosition);
    }

    /// <summary>
    /// ドラッグ中のピースをマウスに合わせて移動
    /// </summary>
    void DragPiece()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(rootPiece.position).z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition) + offset;
        rootPiece.position = worldPosition;
    }
}
