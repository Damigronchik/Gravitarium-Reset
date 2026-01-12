using UnityEngine;
public class Terminal : MonoBehaviour
{
    [Header("Terminal Settings")]
    [SerializeField] private string terminalId = "terminal_001";
    [SerializeField] private bool isActivated = false;
    [Header("Puzzle Reference")]
    [SerializeField] private TerminalHackPuzzle puzzle;
    private void Start()
    {
        if (puzzle == null)
        {
            puzzle = GetComponent<TerminalHackPuzzle>();
        }
    }
    public void Interact()
    {
        EventBus.InvokeTerminalActivated(gameObject);
        if (puzzle != null)
        {
            puzzle.StartPuzzle();
        }
        else
        {
            Debug.LogWarning($"Terminal {terminalId} has no puzzle assigned!");
        }
    }
    public string TerminalId => terminalId;
    public bool IsActivated => isActivated;
    public void ResetTerminal()
    {
        isActivated = false;
    }
}
