using UnityEngine;
public class PuzzleGameController : MonoBehaviour
{
    public PuzzleCompletionChecker puzzleChecker;
    public PartSpawner partSpawner;

    void Start()
    {
        // パズルが完成した際の処理を追加
        puzzleChecker.OnPuzzleCompleted += OnPuzzleComplete;
    }

    void OnPuzzleComplete()
    {
        Debug.Log("パズルが完成しました！");
        partSpawner.SpawnNextPart();  // 新しいパーツをスポーン
    }
}
