using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ArrowButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Visual")]
    [SerializeField] private Button upArrowButton;
    [SerializeField] private Button downArrowButton;
    private int columnIndex;
    private TerminalHackUI terminalUI;
    private bool isHighlighted = false;
    public void Initialize(int columnIdx, TerminalHackUI ui)
    {
        columnIndex = columnIdx;
        terminalUI = ui;
        if (upArrowButton != null)
        {
            upArrowButton.onClick.AddListener(OnUpArrowClicked);
        }
        if (downArrowButton != null)
        {
            downArrowButton.onClick.AddListener(OnDownArrowClicked);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, null, out localPoint);
            bool isUp = localPoint.y > 0;
            if (terminalUI != null)
            {
                terminalUI.OnArrowButtonClicked(columnIndex, isUp);
            }
        }
    }
    public void OnUpArrowClicked()
    {
        if (terminalUI != null)
        {
            Debug.Log($"Up arrow clicked for column {columnIndex}");
            terminalUI.OnArrowButtonClicked(columnIndex, true);
        }
    }
    public void OnDownArrowClicked()
    {
        if (terminalUI != null)
        {
            Debug.Log($"Down arrow clicked for column {columnIndex}");
            terminalUI.OnArrowButtonClicked(columnIndex, false);
        }
    }
    public void HandleClick(Vector2 screenPosition)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null && terminalUI != null)
        {
            Vector2 localPoint;
            Camera eventCamera = null;
            if (terminalCanvas != null)
            {
                eventCamera = terminalCanvas.worldCamera;
            }
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, screenPosition, eventCamera, out localPoint))
            {
                bool isUp = localPoint.y > 0;
                terminalUI.OnArrowButtonClicked(columnIndex, isUp);
            }
        }
    }
    private Canvas terminalCanvas;
    public void SetTerminalCanvas(Canvas canvas)
    {
        terminalCanvas = canvas;
    }
    public Button GetUpButton()
    {
        return upArrowButton;
    }
    public Button GetDownButton()
    {
        return downArrowButton;
    }
}
