using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class PuzzleHazardLink
{
    [Tooltip("ID головоломки (PuzzleId), при решении которой отключаются разряды")]
    public string puzzleId;
    [Tooltip("Список энергетических разрядов, которые отключатся при решении этой головоломки")]
    public List<EnergyDischarge> energyDischargesToDisable = new List<EnergyDischarge>();
}
public class HazardManager : MonoBehaviour
{
    [Header("Puzzle-Hazard Links")]
    [Tooltip("Связи между головоломками и энергетическими разрядами. При решении головоломки отключаются связанные с ней разряды.")]
    [SerializeField] private List<PuzzleHazardLink> puzzleHazardLinks = new List<PuzzleHazardLink>();
    [Header("All Energy Discharges (Auto-filled)")]
    [Tooltip("Все энергетические разряды на сцене (автоматически заполняется при старте)")]
    [SerializeField] private List<EnergyDischarge> allEnergyDischarges = new List<EnergyDischarge>();
    private Dictionary<string, PuzzleHazardLink> puzzleLinksDictionary = new Dictionary<string, PuzzleHazardLink>();
    private void Start()
    {
        allEnergyDischarges.AddRange(FindObjectsOfType<EnergyDischarge>());
        InitializePuzzleLinks();
        SubscribeToEvents();
    }
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    private void InitializePuzzleLinks()
    {
        puzzleLinksDictionary.Clear();
        foreach (var link in puzzleHazardLinks)
        {
            if (!string.IsNullOrEmpty(link.puzzleId) && !puzzleLinksDictionary.ContainsKey(link.puzzleId))
            {
                puzzleLinksDictionary[link.puzzleId] = link;
            }
        }
    }
    private void SubscribeToEvents()
    {
        EventBus.OnPuzzleSolved += OnPuzzleSolved;
    }
    private void UnsubscribeFromEvents()
    {
        EventBus.OnPuzzleSolved -= OnPuzzleSolved;
    }
    private void OnPuzzleSolved(GameObject puzzle)
    {
        var puzzleComponent = puzzle.GetComponent<BasePuzzle>();
        if (puzzleComponent != null)
        {
            string puzzleId = puzzleComponent.PuzzleId;
            if (puzzleLinksDictionary.TryGetValue(puzzleId, out PuzzleHazardLink link))
            {
                foreach (var discharge in link.energyDischargesToDisable)
                {
                    if (discharge != null && discharge.gameObject != null)
                    {
                        discharge.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    public void DisableAllEnergyDischarges()
    {
        foreach (var discharge in allEnergyDischarges)
        {
            if (discharge != null && discharge.gameObject != null)
            {
                discharge.gameObject.SetActive(false);
            }
        }
    }
    public void EnableAllEnergyDischarges()
    {
        foreach (var discharge in allEnergyDischarges)
        {
            if (discharge != null && discharge.gameObject != null)
            {
                discharge.gameObject.SetActive(true);
            }
        }
    }
    public void RegisterEnergyDischarge(EnergyDischarge discharge)
    {
        if (discharge != null && !allEnergyDischarges.Contains(discharge))
        {
            allEnergyDischarges.Add(discharge);
        }
    }
    public void AddPuzzleHazardLink(string puzzleId, List<EnergyDischarge> discharges)
    {
        if (string.IsNullOrEmpty(puzzleId) || discharges == null)
            return;
        var existingLink = puzzleHazardLinks.Find(link => link.puzzleId == puzzleId);
        if (existingLink != null)
        {
            foreach (var discharge in discharges)
            {
                if (discharge != null && !existingLink.energyDischargesToDisable.Contains(discharge))
                {
                    existingLink.energyDischargesToDisable.Add(discharge);
                }
            }
        }
        else
        {
            var newLink = new PuzzleHazardLink
            {
                puzzleId = puzzleId,
                energyDischargesToDisable = new List<EnergyDischarge>(discharges)
            };
            puzzleHazardLinks.Add(newLink);
            puzzleLinksDictionary[puzzleId] = newLink;
        }
    }
}
