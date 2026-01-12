using UnityEngine;
public class GravityPuzzle : BasePuzzle
{
    [Header("Gravity Puzzle Settings")]
    [SerializeField] private GameObject[] targetObjects;
    [SerializeField] private Transform[] targetPositions;
    [SerializeField] private float positionTolerance = 0.5f;
    [Header("Visual Feedback")]
    [SerializeField] private Material correctMaterial;
    [SerializeField] private Material incorrectMaterial;
    [SerializeField] private Renderer[] targetRenderers;
    private int correctObjectsCount = 0;
    private void Start()
    {
        if (targetObjects == null || targetPositions == null)
        {
            Debug.LogWarning($"GravityPuzzle {puzzleId}: Target objects or positions not set!");
            return;
        }
        if (targetObjects.Length != targetPositions.Length)
        {
            Debug.LogWarning($"GravityPuzzle {puzzleId}: Mismatch between target objects and positions count!");
        }
    }
    private void Update()
    {
        if (currentState != PuzzleState.InProgress)
            return;
        CheckPuzzleProgress();
    }
    private void CheckPuzzleProgress()
    {
        correctObjectsCount = 0;
        int totalObjects = Mathf.Min(targetObjects.Length, targetPositions.Length);
        for (int i = 0; i < totalObjects; i++)
        {
            if (targetObjects[i] == null || targetPositions[i] == null)
                continue;
            float distance = Vector3.Distance(targetObjects[i].transform.position, targetPositions[i].position);
            bool isCorrect = distance <= positionTolerance;
            if (isCorrect)
            {
                correctObjectsCount++;
            }
            UpdateVisualFeedback(i, isCorrect);
        }
        float progress = totalObjects > 0 ? (float)correctObjectsCount / totalObjects : 0f;
        UpdateProgress(progress);
        if (correctObjectsCount == totalObjects && totalObjects > 0)
        {
            SolvePuzzle();
        }
    }
    private void UpdateVisualFeedback(int index, bool isCorrect)
    {
        if (targetRenderers != null && index < targetRenderers.Length && targetRenderers[index] != null)
        {
            targetRenderers[index].material = isCorrect ? correctMaterial : incorrectMaterial;
        }
    }
    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        correctObjectsCount = 0;
        if (targetRenderers != null)
        {
            foreach (var renderer in targetRenderers)
            {
                if (renderer != null && incorrectMaterial != null)
                {
                    renderer.material = incorrectMaterial;
                }
            }
        }
    }
    protected override void OnPuzzleStateRestored()
    {
        base.OnPuzzleStateRestored();
        if (currentState == PuzzleState.Solved && targetRenderers != null && correctMaterial != null)
        {
            foreach (var renderer in targetRenderers)
            {
                if (renderer != null)
                {
                    renderer.material = correctMaterial;
                }
            }
            Debug.Log($"GravityPuzzle {puzzleId}: Visual state restored for solved puzzle");
        }
    }
}
