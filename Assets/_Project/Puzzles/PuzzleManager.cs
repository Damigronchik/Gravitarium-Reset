using UnityEngine;
using System.Collections.Generic;
public class PuzzleManager : MonoBehaviour
{
    [Header("Puzzle Management")]
    [SerializeField] private List<BasePuzzle> puzzles = new List<BasePuzzle>();
    private Dictionary<string, BasePuzzle> puzzleDictionary = new Dictionary<string, BasePuzzle>();
    private int solvedPuzzlesCount = 0;
    public int TotalPuzzles => puzzles.Count;
    public int SolvedPuzzles => solvedPuzzlesCount;
    public float Progress => puzzles.Count > 0 ? (float)solvedPuzzlesCount / puzzles.Count : 0f;
    private void Awake()
    {
        InitializePuzzles();
    }
    private void Start()
    {
        EventBus.OnPuzzleSolved += OnPuzzleSolved;
    }
    private void OnDestroy()
    {
        EventBus.OnPuzzleSolved -= OnPuzzleSolved;
    }
    private void InitializePuzzles()
    {
        if (puzzles.Count == 0)
        {
            puzzles.AddRange(FindObjectsOfType<BasePuzzle>());
        }
        foreach (var puzzle in puzzles)
        {
            if (puzzle != null && !puzzleDictionary.ContainsKey(puzzle.PuzzleId))
            {
                puzzleDictionary[puzzle.PuzzleId] = puzzle;
            }
        }
    }
    private void OnPuzzleSolved(GameObject puzzleObject)
    {
        var puzzle = puzzleObject.GetComponent<BasePuzzle>();
        if (puzzle != null && puzzleDictionary.ContainsValue(puzzle))
        {
            solvedPuzzlesCount++;
        }
    }
    public void RegisterPuzzle(BasePuzzle puzzle)
    {
        if (puzzle != null && !puzzles.Contains(puzzle))
        {
            puzzles.Add(puzzle);
            if (!puzzleDictionary.ContainsKey(puzzle.PuzzleId))
            {
                puzzleDictionary[puzzle.PuzzleId] = puzzle;
            }
        }
    }
    public BasePuzzle GetPuzzle(string puzzleId)
    {
        return puzzleDictionary.TryGetValue(puzzleId, out var puzzle) ? puzzle : null;
    }
    public void ResetAllPuzzles()
    {
        foreach (var puzzle in puzzles)
        {
            if (puzzle != null)
            {
                puzzle.ResetPuzzle();
            }
        }
        solvedPuzzlesCount = 0;
    }
    public void RecalculateSolvedCount()
    {
        solvedPuzzlesCount = 0;
        foreach (var puzzle in puzzles)
        {
            if (puzzle != null && puzzle.IsSolved)
            {
                solvedPuzzlesCount++;
            }
        }
    }
}
