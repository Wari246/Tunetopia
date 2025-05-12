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
        if (spawnSteps == null || spawnSteps.Count == 0)
        {
            Debug.LogWarning("spawnSteps が設定されていません。ステップが1つ以上必要です。");
            return;
        }

        foreach (var step in spawnSteps)
        {
            foreach (var obj in step.piecesToSpawn)
            {
                if (obj != null) obj.SetActive(false);
            }

            foreach (var hideObj in step.objectsToHideOnStepComplete)
            {
                if (hideObj != null) hideObj.SetActive(true);
            }
        }

        SpawnCurrentStep();

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

    private void SpawnCurrentStep()
    {
        if (currentStepIndex < 0 || currentStepIndex >= spawnSteps.Count)
        {
            Debug.LogWarning($"SpawnCurrentStep: 無効な currentStepIndex = {currentStepIndex}");
            return;
        }

        foreach (var step in spawnSteps)
        {
            foreach (var obj in step.piecesToSpawn)
            {
                if (obj != null) obj.SetActive(false);
            }
        }

        Step currentStep = spawnSteps[currentStepIndex];
        foreach (var obj in currentStep.piecesToSpawn)
        {
            if (obj != null) obj.SetActive(true);
        }

        currentCompletedCount = 0;
        awaitingNext = false;
        nextButton?.gameObject.SetActive(false);
    }

    public void ReportPieceFullyInserted()
    {
        if (awaitingNext) return;

        if (currentStepIndex < 0 || currentStepIndex >= spawnSteps.Count)
        {
            Debug.LogWarning($"ReportPieceFullyInserted: currentStepIndex ({currentStepIndex}) が spawnSteps.Count ({spawnSteps.Count}) の範囲外です。");
            return;
        }

        currentCompletedCount++;
        Step currentStep = spawnSteps[currentStepIndex];

        Debug.Log($"ステップ {currentStepIndex} の合体数: {currentCompletedCount} / {currentStep.requiredCompletedCount}");

        if (currentCompletedCount >= currentStep.requiredCompletedCount)
        {
            awaitingNext = true;

            if (nextButton != null)
            {
                nextButton.gameObject.SetActive(true);
            }
        }
    }

    private void OnNextButtonClicked()
    {
        if (currentStepIndex < 0 || currentStepIndex >= spawnSteps.Count)
        {
            Debug.LogWarning($"OnNextButtonClicked: currentStepIndex ({currentStepIndex}) が範囲外です。");
            return;
        }

        Step currentStep = spawnSteps[currentStepIndex];

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
