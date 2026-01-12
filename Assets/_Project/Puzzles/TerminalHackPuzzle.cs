using UnityEngine;
public class TerminalHackPuzzle : BasePuzzle
{
    [Header("Terminal Hack Settings")]
    [SerializeField] private TerminalHackUI terminalHackUI;
    private bool isActive = false;
    private void Start()
    {
        if (terminalHackUI != null)
        {
            terminalHackUI.Initialize(this);
            terminalHackUI.gameObject.SetActive(false);
        }
    }
    public override void StartPuzzle()
    {
        base.StartPuzzle();
        isActive = true;
        if (terminalHackUI != null)
        {
            terminalHackUI.OpenTerminal();
        }
    }
    public void OnHackSuccessful()
    {
        SolvePuzzle();
        StartCoroutine(CloseUIAfterDelay(1.5f));
    }
    private System.Collections.IEnumerator CloseUIAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseUI();
    }
    public void OnHackCancelled()
    {
        if (!isActive) return;
        CloseUI();
    }
    private void CloseUI()
    {
        if (!isActive) return;
        isActive = false;
        if (terminalHackUI != null)
        {
            terminalHackUI.CloseTerminal();
        }
    }
    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        isActive = false;
        CloseUI();
    }
    protected override void OnPuzzleStateRestored()
    {
        base.OnPuzzleStateRestored();
        if (currentState == PuzzleState.Solved)
        {
            isActive = false;
            if (terminalHackUI != null)
            {
                terminalHackUI.gameObject.SetActive(false);
            }
            Debug.Log($"TerminalHackPuzzle {puzzleId}: Visual state restored for solved puzzle");
        }
    }
}
