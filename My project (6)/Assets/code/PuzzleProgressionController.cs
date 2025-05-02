using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PuzzleProgressionController : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        [Header("このステップで出現するパーツ")]
        public List<GameObject> piecesToSpawn;

        [Header("このステップの完了に必要な合体数")]
        public int requiredCompletedCount = 1;

        [Header("このステップ完了時に非表示にするオブジェクト（完成品・見本など）")]
        public List<GameObject> objectsToHideOnStepComplete;
    }

    public static PuzzleProgressionController Instance;

    [Header("各ステップの設定")]
    public List<Step> spawnSteps;

    [Header("次へボタン（押すと次のステップへ）")]
    public Button nextButton;

    private int currentStepIndex = 0;
    private int currentCompletedCount = 0;
    private bool awaitingNext = false;

    void Awake()
    {
        // シングルトン化
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ステップが未設定なら警告
        if (spawnSteps == null || spawnSteps.Count == 0)
        {
            Debug.LogWarning("spawnSteps が設定されていません。ステップが1つ以上必要です。");
            return;
        }

        // 全パーツを非表示
        foreach (var step in spawnSteps)
        {
            foreach (var obj in step.piecesToSpawn)
            {
                if (obj != null) obj.SetActive(false);
            }

            foreach (var hideObj in step.objectsToHideOnStepComplete)
            {
                if (hideObj != null) hideObj.SetActive(true); // 念のため表示
            }
        }

        // 最初のステップ開始
        SpawnCurrentStep();

        // 「次へ」ボタン設定
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }
        else
        {
            Debug.LogWarning("Nextボタンがアサインされていません！");
        }
    }

    /// <summary>
    /// 現在のステップのパーツを表示
    /// </summary>
    private void SpawnCurrentStep()
    {
        if (currentStepIndex < 0 || currentStepIndex >= spawnSteps.Count)
        {
            Debug.LogWarning($"SpawnCurrentStep: 無効な currentStepIndex = {currentStepIndex}");
            return;
        }

        // 全パーツ非表示
        foreach (var step in spawnSteps)
        {
            foreach (var obj in step.piecesToSpawn)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        // 現在のステップのみ表示
        Step currentStep = spawnSteps[currentStepIndex];
        foreach (var obj in currentStep.piecesToSpawn)
        {
            if (obj != null) obj.SetActive(true);
        }

        currentCompletedCount = 0;
        awaitingNext = false;
        nextButton?.gameObject.SetActive(false);
    }

    /// <summary>
    /// パーツが完全に合体したときに呼ばれる
    /// </summary>
    public void ReportPieceFullyInserted()
    {
        if (awaitingNext) return;

        // 範囲外チェック
        if (currentStepIndex < 0 || currentStepIndex >= spawnSteps.Count)
        {
            Debug.LogWarning($"ReportPieceFullyInserted: currentStepIndex ({currentStepIndex}) が spawnSteps.Count ({spawnSteps.Count}) の範囲外です。");
            return;
        }

        currentCompletedCount++;
        Step currentStep = spawnSteps[currentStepIndex];

        if (currentCompletedCount >= currentStep.requiredCompletedCount)
        {
            awaitingNext = true;

            // 「次へ」ボタンを表示
            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 「次へ」ボタンが押されたときに次のステップへ進む
    /// </summary>
    private void OnNextButtonClicked()
    {
        if (currentStepIndex < 0 || currentStepIndex >= spawnSteps.Count)
        {
            Debug.LogWarning($"OnNextButtonClicked: currentStepIndex ({currentStepIndex}) が範囲外です。");
            return;
        }

        Step currentStep = spawnSteps[currentStepIndex];

        // 完了時に非表示にするオブジェクト
        foreach (var obj in currentStep.objectsToHideOnStepComplete)
        {
            if (obj != null) obj.SetActive(false);
        }

        nextButton?.gameObject.SetActive(false);
        currentStepIndex++;

        if (currentStepIndex < spawnSteps.Count)
        {
            SpawnCurrentStep();
        }
        else
        {
            Debug.Log("全ステップ完了！");
        }
    }
}
