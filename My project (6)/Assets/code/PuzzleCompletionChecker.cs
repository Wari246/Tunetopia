using UnityEngine;

public class PuzzleCompletionChecker : MonoBehaviour
{
    public delegate void PuzzleCompletedEvent();
    public event PuzzleCompletedEvent OnPuzzleCompleted;

    private bool isCompleted = false;

    void Update()
    {
        // パズルが完成していない場合に確認を行う
        if (!isCompleted && IsPuzzleComplete())
        {
            isCompleted = true;
            OnPuzzleCompleted?.Invoke();  // パズルが完成したときにイベントを発火
        }
    }

    // パズルが完成したかどうかを確認
    public bool IsPuzzleComplete()
    {
        // すべての SnapSocket を取得して、すべてが完全に接続されているかを確認
        SnapSocket[] sockets = GetComponentsInChildren<SnapSocket>(true); // 孫オブジェクトも含める
        bool allPartsInPlace = true;

        // すべての SnapSocket が完全に接続されているか確認
        foreach (var socket in sockets)
        {
            if (!socket.IsFullySnapped)  // 1つでも完全に接続されていない場合
            {
                allPartsInPlace = false;
                break;
            }
        }

        return allPartsInPlace;  // すべてが完全に接続されていればパズルが完成
    }
}
