using UnityEngine;
using System.Collections.Generic;
public class KeyPuzzle : BasePuzzle
{
    [Header("Key Puzzle Settings")]
    [SerializeField] private int requiredEnergyCores = 3;
    [SerializeField] private Transform[] keySlots;
    [SerializeField] private bool requiresSpecificOrder = false;
    [Header("Visual Feedback")]
    [SerializeField] private Material emptySlotMaterial;
    [SerializeField] private Material filledSlotMaterial;
    [SerializeField] private Renderer[] slotRenderers;
    private List<EnergyCore> placedCores = new List<EnergyCore>();
    private int currentSlotIndex = 0;
    private void Start()
    {
        InitializeSlots();
        EventBus.OnEnergyCoreCollected += OnEnergyCoreCollected;
    }
    private void OnDestroy()
    {
        EventBus.OnEnergyCoreCollected -= OnEnergyCoreCollected;
    }
    private void InitializeSlots()
    {
        if (slotRenderers != null)
        {
            foreach (var renderer in slotRenderers)
            {
                if (renderer != null && emptySlotMaterial != null)
                {
                    renderer.material = emptySlotMaterial;
                }
            }
        }
    }
    private void OnEnergyCoreCollected(GameObject energyCore)
    {
        if (currentState != PuzzleState.InProgress)
            return;
        var core = energyCore.GetComponent<EnergyCore>();
        if (core == null)
            return;
        if (placedCores.Count < requiredEnergyCores)
        {
            PlaceKey(core);
        }
    }
    private void PlaceKey(EnergyCore core)
    {
        if (keySlots == null || keySlots.Length == 0)
        {
            Debug.LogWarning($"KeyPuzzle {puzzleId}: No key slots defined!");
            return;
        }
        int slotIndex = requiresSpecificOrder ? currentSlotIndex : placedCores.Count;
        if (slotIndex >= keySlots.Length)
            return;
        core.transform.position = keySlots[slotIndex].position;
        core.transform.rotation = keySlots[slotIndex].rotation;
        placedCores.Add(core);
        UpdateSlotVisual(slotIndex, true);
        float progress = (float)placedCores.Count / requiredEnergyCores;
        UpdateProgress(progress);
        if (placedCores.Count >= requiredEnergyCores)
        {
            SolvePuzzle();
        }
        currentSlotIndex++;
    }
    private void UpdateSlotVisual(int slotIndex, bool isFilled)
    {
        if (slotRenderers != null && slotIndex < slotRenderers.Length && slotRenderers[slotIndex] != null)
        {
            slotRenderers[slotIndex].material = isFilled ? filledSlotMaterial : emptySlotMaterial;
        }
    }
    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        placedCores.Clear();
        currentSlotIndex = 0;
        InitializeSlots();
    }
    protected override void OnPuzzleStateRestored()
    {
        base.OnPuzzleStateRestored();
        if (currentState == PuzzleState.Solved)
        {
            if (slotRenderers != null && filledSlotMaterial != null)
            {
                for (int i = 0; i < slotRenderers.Length && i < requiredEnergyCores; i++)
                {
                    if (slotRenderers[i] != null)
                    {
                        slotRenderers[i].material = filledSlotMaterial;
                    }
                }
            }
            Debug.Log($"KeyPuzzle {puzzleId}: Visual state restored for solved puzzle");
        }
    }
}
