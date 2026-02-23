using UnityEngine;

/// <summary>
/// Фигура на канвасе, которую можно перетаскивать прицелом (центр экрана).
/// При отпускании проверяется правильный слот (DragDropSlotUI).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DraggableShapeUI : MonoBehaviour
{
    [Header("Drag & Drop")]
    [SerializeField] private DragDropSlotUI correctSlot;

    private RectTransform rectTransform;
    private Vector2 startAnchoredPosition;
    private Vector2 startAnchorMin, startAnchorMax;
    private bool isPlaced;

    public DragDropSlotUI CorrectSlot => correctSlot;
    public bool IsPlaced => isPlaced;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startAnchoredPosition = rectTransform.anchoredPosition;
    }

    private void Start()
    {
        StoreStartState();
    }

    private void StoreStartState()
    {
        startAnchoredPosition = rectTransform.anchoredPosition;
        startAnchorMin = rectTransform.anchorMin;
        startAnchorMax = rectTransform.anchorMax;
    }

    /// <summary>
    /// Сохраняет текущую позицию как стартовую (после перемешивания головоломки).
    /// </summary>
    public void RefreshStartState()
    {
        StoreStartState();
    }

    public void Place()
    {
        isPlaced = true;
        if (correctSlot != null)
        {
            var slotRect = correctSlot.GetComponent<RectTransform>();
            if (slotRect != null)
            {
                rectTransform.anchoredPosition = slotRect.anchoredPosition;
                rectTransform.anchorMin = slotRect.anchorMin;
                rectTransform.anchorMax = slotRect.anchorMax;
            }
        }
    }

    public void ResetItem()
    {
        isPlaced = false;
        rectTransform.anchoredPosition = startAnchoredPosition;
        rectTransform.anchorMin = startAnchorMin;
        rectTransform.anchorMax = startAnchorMax;
    }

    public void SetAnchoredPosition(Vector2 pos)
    {
        rectTransform.anchoredPosition = pos;
    }

    public Vector2 GetAnchoredPosition()
    {
        return rectTransform.anchoredPosition;
    }

    public RectTransform RectTransform => rectTransform;
}
