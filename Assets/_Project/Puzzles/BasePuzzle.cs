using UnityEngine;
public abstract class BasePuzzle : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] protected string puzzleId = "puzzle_001";
    [SerializeField] protected PuzzleState currentState = PuzzleState.InProgress;
    public enum PuzzleState
    {
        InProgress,
        Solved,
        Failed
    }
    public string PuzzleId => puzzleId;
    public PuzzleState CurrentState => currentState;
    public bool IsSolved => currentState == PuzzleState.Solved;
    public virtual void StartPuzzle()
    {
        if (currentState == PuzzleState.Solved)
            return;
        currentState = PuzzleState.InProgress;
        OnPuzzleStarted();
    }
    protected virtual void SolvePuzzle()
    {
        if (currentState == PuzzleState.Solved)
            return;
        currentState = PuzzleState.Solved;
        OnPuzzleSolved();
    }
    protected virtual void UpdateProgress(float progress)
    {
        EventBus.InvokePuzzleProgressed(gameObject, progress);
    }
    protected virtual void OnPuzzleStarted()
    {
        Debug.Log($"Puzzle {puzzleId} started");
    }
    protected virtual void OnPuzzleSolved()
    {
        EventBus.InvokePuzzleSolved(gameObject);
        Debug.Log($"Puzzle {puzzleId} solved!");
    }
    public virtual void ResetPuzzle()
    {
        currentState = PuzzleState.InProgress;
    }
    public virtual void RestorePuzzleState(PuzzleState state)
    {
        currentState = state;
        OnPuzzleStateRestored();
    }
    protected virtual void OnPuzzleStateRestored()
    {
    }
}
