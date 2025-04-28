using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance { get; private set; }

    public PuzzlePiece[] puzzlePieces;
    public GameObject[] nextParts; // 次に出現するパーツ
    public Vector3[] targetPositions; // 各パズルピースのターゲット位置
    public GameObject newPartPrefab; // 新しいパーツのプレハブ
    public Transform spawnPoint; // 新しいパーツの出現位置

    public GameObject[] otherObjects; // 関係ないオブジェクト
    public GameObject[] newOtherObjects; // 新しく登場するオブジェクト

    private bool hasShownNextParts = false;
    private bool hasClicked = false;

    public float moveDistance = 5f; // 上昇距離
    public float fadeSpeed = 1.0f; // フェードイン・フェードアウト速度
    public float newPartMoveSpeed = 1.0f; // 新しいパズルピースの上昇速度
    public float otherObjectsMoveSpeed = 2.0f; // 既存の関係ないオブジェクトの上昇速度
    public float newOtherObjectsMoveSpeed = 1.5f; // 新しく登場する関係ないオブジェクトの上昇速度
    public float maxHeightOtherObjects = 10f; // 既存オブジェクトの最大高さ
    public float maxHeightNewOtherObjects = 8f; // 新しく登場するオブジェクトの最大高さ

    private int currentPartIndex = 0; // 現在表示しているパズルのインデックス

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        puzzlePieces = FindObjectsOfType<PuzzlePiece>();

        ValidatePuzzle();
        if (puzzlePieces.Length == 0)
        {
            Debug.LogError("PuzzlePieces が見つかりません！");
            return;
        }
    }

    void Update()
    {
        if (!hasShownNextParts && CheckPuzzleCompletion())
        {
            ShowNextParts();
            hasShownNextParts = true;
        }

        if (hasShownNextParts && !hasClicked && Input.GetMouseButtonDown(0))
        {
            HideNextParts();
            SpawnNewPart();
            hasClicked = true;

            // すべてのパズルピースを縮小して削除
            foreach (var puzzlePiece in puzzlePieces)
            {
                StartCoroutine(ShrinkAndDestroy(puzzlePiece.gameObject));
            }

            // その他のオブジェクトを処理
            StartCoroutine(HandleOtherObjects());
        }
    }

    void ValidatePuzzle()
    {
        if (puzzlePieces.Length != targetPositions.Length)
        {
            Debug.LogError("puzzlePieces と targetPositions の数が一致しません！");
        }
    }

    bool CheckPuzzleCompletion()
    {
        int completedCount = 0;
        float tolerance = 3f;

        for (int i = 0; i < puzzlePieces.Length; i++)
        {
            if (Vector3.Distance(puzzlePieces[i].transform.position, targetPositions[i]) <= tolerance)
            {
                completedCount++;
            }
        }

        return completedCount == puzzlePieces.Length;
    }

    void ShowNextParts()
    {
        if (currentPartIndex < nextParts.Length)
        {
            nextParts[currentPartIndex].SetActive(true);
        }
    }

    void HideNextParts()
    {
        if (currentPartIndex < nextParts.Length)
        {
            nextParts[currentPartIndex].SetActive(false);
        }
    }

    void SpawnNewPart()
    {
        if (newPartPrefab != null && spawnPoint != null)
        {
            Vector3 startPosition = spawnPoint.position + Vector3.down * 3f;
            GameObject newPart = Instantiate(newPartPrefab, startPosition, Quaternion.identity);
            StartCoroutine(MoveNewPartUpwards(newPart));
        }
    }

    IEnumerator MoveNewPartUpwards(GameObject newPart)
    {
        Vector3 targetPosition = spawnPoint.position;
        while (newPart.transform.position.y < targetPosition.y)
        {
            newPart.transform.position += Vector3.up * newPartMoveSpeed * Time.deltaTime;
            yield return null;
        }

        newPart.transform.position = targetPosition;

        // 新しいパーツが完了した後、次のパーツを準備
        currentPartIndex++;
        hasShownNextParts = false;
        hasClicked = false;

        // 次のパーツがあれば表示する
        if (currentPartIndex < nextParts.Length)
        {
            ShowNextParts();
        }
    }

    IEnumerator ShrinkAndDestroy(GameObject targetObject)
    {
        if (targetObject == null) yield break;

        Vector3 initialScale = targetObject.transform.localScale;
        Vector3 targetScale = Vector3.zero;
        float shrinkDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < shrinkDuration)
        {
            targetObject.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / shrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(targetObject);
    }

    IEnumerator HandleOtherObjects()
    {
        float fadeDuration = 1.0f;

        for (int i = 0; i < otherObjects.Length; i++)
        {
            GameObject obj = otherObjects[i];
            Vector3 startPos = obj.transform.position;

            yield return MoveAndFadeObject(obj, startPos, otherObjectsMoveSpeed, fadeDuration, maxHeightOtherObjects);
            Destroy(obj);

            if (i < newOtherObjects.Length)
            {
                GameObject newObj = newOtherObjects[i];
                newObj.transform.position = startPos - Vector3.up * moveDistance;
                newObj.SetActive(true);

                yield return MoveAndFadeObject(newObj, startPos, newOtherObjectsMoveSpeed, fadeDuration, maxHeightNewOtherObjects);
            }
        }
    }

    IEnumerator MoveAndFadeObject(GameObject obj, Vector3 startPos, float moveSpeed, float fadeDuration, float maxHeight)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            if (obj.transform.position.y + moveSpeed * Time.deltaTime >= maxHeight)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, maxHeight, obj.transform.position.z);
                break;
            }

            obj.transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                renderer.material.color = color;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

