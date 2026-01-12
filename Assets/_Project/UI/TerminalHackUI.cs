using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class TerminalHackUI : MonoBehaviour
{
    [Header("Puzzle Reference")]
    [SerializeField] private TerminalHackPuzzle puzzle;
    [Header("UI Elements")]
    [SerializeField] private Canvas terminalCanvas;
    [SerializeField] private RectTransform columnsContainer;
    [SerializeField] private RectTransform buttonsContainer;
    [Header("Prefabs")]
    [SerializeField] private GameObject columnPrefab;
    [SerializeField] private GameObject valueCellPrefab;
    [SerializeField] private GameObject arrowButtonPrefab;
    [Header("Settings")]
    [SerializeField] private float canvasDistance = 1.5f;
    [SerializeField] private int columnsCount = 4;
    [SerializeField] private int valuesPerColumn = 8;
    [SerializeField] private int visibleValues = 4;
    private List<ColumnData> columns = new List<ColumnData>();
    private List<ArrowButton> arrowButtons = new List<ArrowButton>();
    private bool isActive = false;
    private bool isClosing = false;
    private bool isSolved = false;
    private Camera playerCamera;
    private InputSystem inputActions;
    private bool isMouseHeld = false;
    private bool wasMouseDown = false;
    private class ColumnData
    {
        public List<int> values = new List<int>();
        public int visibleStartIndex = 0;
        public RectTransform columnTransform;
        public List<TextMeshProUGUI> visibleCells = new List<TextMeshProUGUI>();
    }
    private void Awake()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = FindObjectOfType<Camera>();
        }
        inputActions = new InputSystem();
    }
    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Enable();
            inputActions.Player.Attack.performed += OnMouseDown;
            inputActions.Player.Attack.canceled += OnMouseUp;
        }
    }
    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Attack.performed -= OnMouseDown;
            inputActions.Player.Attack.canceled -= OnMouseUp;
            inputActions.Player.Disable();
        }
    }
    private void OnMouseDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isMouseHeld = true;
            wasMouseDown = false;
        }
    }
    private void OnMouseUp(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            isMouseHeld = false;
            wasMouseDown = false;
        }
    }
    private void Update()
    {
        if (!isActive) return;
        UpdateCanvasRotation();
        HandleButtonClicks();
        CheckPuzzleComplete();
    }
    private void UpdateCanvasRotation()
    {
        if (terminalCanvas != null && playerCamera != null)
        {
            Vector3 directionToCamera = playerCamera.transform.position - terminalCanvas.transform.position;
            directionToCamera.y = 0;
            if (directionToCamera != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-directionToCamera);
                terminalCanvas.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            }
        }
    }
    public void Initialize(TerminalHackPuzzle puzzleRef)
    {
        puzzle = puzzleRef;
        CreateColumns();
        CreateArrowButtons();
    }
    public void OpenTerminal()
    {
        isClosing = false;
        isSolved = false;
        isActive = false;
        gameObject.SetActive(true);
        if (terminalCanvas != null)
        {
            terminalCanvas.gameObject.SetActive(true);
        }
        InitializePuzzle();
        isActive = true;
    }
    public void CloseTerminal()
    {
        if (isClosing) return;
        isClosing = true;
        isActive = false;
        if (terminalCanvas != null)
        {
            terminalCanvas.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
    private void InitializePuzzle()
    {
        foreach (ColumnData column in columns)
        {
            column.values.Clear();
            for (int i = 0; i < valuesPerColumn; i++)
            {
                column.values.Add(0);
            }
            int onePosition = Random.Range(0, valuesPerColumn);
            column.values[onePosition] = 1;
            column.visibleStartIndex = Mathf.Clamp(onePosition - visibleValues / 2, 0, valuesPerColumn - visibleValues);
            UpdateColumnDisplay(column);
        }
        if (AreAllOnesAligned())
        {
            if (columns.Count > 0)
            {
                ColumnData firstColumn = columns[0];
                int shiftAmount = Random.Range(1, 4);
                for (int i = 0; i < shiftAmount; i++)
                {
                    if (firstColumn.values.Count > 0)
                    {
                        int lastValue = firstColumn.values[firstColumn.values.Count - 1];
                        for (int j = firstColumn.values.Count - 1; j > 0; j--)
                        {
                            firstColumn.values[j] = firstColumn.values[j - 1];
                        }
                        firstColumn.values[0] = lastValue;
                    }
                }
                int onePosition = -1;
                for (int i = 0; i < firstColumn.values.Count; i++)
                {
                    if (firstColumn.values[i] == 1)
                    {
                        onePosition = i;
                        break;
                    }
                }
                if (onePosition >= 0)
                {
                    firstColumn.visibleStartIndex = Mathf.Clamp(onePosition - visibleValues / 2, 0, valuesPerColumn - visibleValues);
                }
                UpdateColumnDisplay(firstColumn);
            }
        }
    }
    private bool AreAllOnesAligned()
    {
        if (columns.Count == 0) return false;
        for (int row = 0; row < visibleValues; row++)
        {
            bool allAligned = true;
            for (int colIndex = 0; colIndex < columns.Count; colIndex++)
            {
                ColumnData column = columns[colIndex];
                int valueIndex = column.visibleStartIndex + row;
                if (valueIndex < 0 || valueIndex >= column.values.Count || column.values[valueIndex] != 1)
                {
                    allAligned = false;
                    break;
                }
            }
            if (allAligned)
            {
                return true;
            }
        }
        return false;
    }
    private void CreateColumns()
    {
        if (columnsContainer == null || columnPrefab == null || valueCellPrefab == null) return;
        columns.Clear();
        for (int colIndex = 0; colIndex < columnsCount; colIndex++)
        {
            GameObject columnObj = Instantiate(columnPrefab, columnsContainer);
            RectTransform columnRect = columnObj.GetComponent<RectTransform>();
            ColumnData column = new ColumnData
            {
                columnTransform = columnRect
            };
            for (int i = 0; i < visibleValues; i++)
            {
                GameObject cellObj = Instantiate(valueCellPrefab, columnRect);
                TextMeshProUGUI cellText = cellObj.GetComponentInChildren<TextMeshProUGUI>();
                if (cellText == null)
                {
                    cellText = cellObj.GetComponent<TextMeshProUGUI>();
                }
                if (cellText != null)
                {
                    column.visibleCells.Add(cellText);
                }
            }
            columns.Add(column);
        }
    }
    private void CreateArrowButtons()
    {
        if (buttonsContainer == null || arrowButtonPrefab == null) return;
        arrowButtons.Clear();
        for (int i = 0; i < columnsCount; i++)
        {
            GameObject buttonObj = Instantiate(arrowButtonPrefab, buttonsContainer);
            ArrowButton arrowButton = buttonObj.GetComponent<ArrowButton>();
            arrowButton.Initialize(i, this);
            if (terminalCanvas != null)
            {
                arrowButton.SetTerminalCanvas(terminalCanvas);
            }
            arrowButtons.Add(arrowButton);
        }
    }
    private float lastClickTime = 0f;
    private const float clickCooldown = 0.2f;
    private void HandleButtonClicks()
    {
        if (playerCamera == null) return;
        if (!isMouseHeld)
        {
            wasMouseDown = false;
            return;
        }
        if (wasMouseDown)
        {
            if (Time.time - lastClickTime < clickCooldown)
            {
                return;
            }
        }
        wasMouseDown = true;
        lastClickTime = Time.time;
        if (UnityEngine.EventSystems.EventSystem.current != null && terminalCanvas != null)
        {
            GraphicRaycaster raycaster = terminalCanvas.GetComponent<GraphicRaycaster>();
            UnityEngine.EventSystems.PointerEventData pointerData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            pointerData.position = new Vector2(Screen.width / 2f, Screen.height / 2f);
            List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
            raycaster.Raycast(pointerData, results);
            if (results.Count == 0)
            {
                UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointerData, results);
            }
            foreach (var result in results)
            {
                Button uiButton = result.gameObject.GetComponent<Button>();
                if (uiButton != null && uiButton.interactable)
                {
                    ArrowButton arrowButton = uiButton.GetComponentInParent<ArrowButton>();
                    if (arrowButton != null)
                    {
                        if (arrowButton.GetUpButton() != null && uiButton == arrowButton.GetUpButton())
                        {
                            arrowButton.OnUpArrowClicked();
                            return;
                        }
                        else if (arrowButton.GetDownButton() != null && uiButton == arrowButton.GetDownButton())
                        {
                            arrowButton.OnDownArrowClicked();
                            return;
                        }
                    }
                    else
                    {
                        uiButton.onClick.Invoke();
                        return;
                    }
                }
                ArrowButton button = result.gameObject.GetComponent<ArrowButton>();
                if (button == null)
                {
                    button = result.gameObject.GetComponentInParent<ArrowButton>();
                }
                if (button != null)
                {
                    RectTransform buttonRect = button.GetComponent<RectTransform>();
                    if (buttonRect != null)
                    {
                        Vector2 localPoint;
                        Camera eventCamera = result.module != null ? result.module.eventCamera : null;
                        if (eventCamera == null && terminalCanvas != null)
                        {
                            eventCamera = terminalCanvas.worldCamera;
                        }
                        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                            buttonRect, result.screenPosition, eventCamera, out localPoint))
                        {
                            float buttonHeight = buttonRect.rect.height;
                            float buttonCenterY = buttonRect.rect.center.y;
                            bool isUp = localPoint.y > buttonCenterY;
                            if (isUp)
                            {
                                button.OnUpArrowClicked();
                            }
                            else
                            {
                                button.OnDownArrowClicked();
                            }
                        }
                    }
                    return;
                }
            }
        }
    }
    public void OnArrowButtonClicked(int columnIndex, bool isUp)
    {
        if (columnIndex < 0 || columnIndex >= columns.Count) return;
        ColumnData column = columns[columnIndex];
        if (isUp)
        {
            if (column.values.Count > 0)
            {
                int firstValue = column.values[0];
                for (int i = 0; i < column.values.Count - 1; i++)
                {
                    column.values[i] = column.values[i + 1];
                }
                column.values[column.values.Count - 1] = firstValue;
            }
        }
        else
        {
            if (column.values.Count > 0)
            {
                int lastValue = column.values[column.values.Count - 1];
                for (int i = column.values.Count - 1; i > 0; i--)
                {
                    column.values[i] = column.values[i - 1];
                }
                column.values[0] = lastValue;
            }
        }
        UpdateColumnDisplay(column);
    }
    private void UpdateColumnDisplay(ColumnData column)
    {
        for (int i = 0; i < visibleValues && i < column.visibleCells.Count; i++)
        {
            int valueIndex = column.visibleStartIndex + i;
            if (valueIndex >= 0 && valueIndex < column.values.Count)
            {
                column.visibleCells[i].text = column.values[valueIndex].ToString();
            }
        }
    }
    private void CheckPuzzleComplete()
    {
        if (isSolved || isClosing || !isActive) return;
        for (int row = 0; row < visibleValues; row++)
        {
            bool allAligned = true;
            for (int colIndex = 0; colIndex < columns.Count; colIndex++)
            {
                ColumnData column = columns[colIndex];
                int valueIndex = column.visibleStartIndex + row;
                if (valueIndex < 0 || valueIndex >= column.values.Count || column.values[valueIndex] != 1)
                {
                    allAligned = false;
                    break;
                }
            }
            if (allAligned)
            {
                OnHackSuccessful();
                return;
            }
        }
    }
    private void OnHackSuccessful()
    {
        if (isSolved || isClosing) return;
        isSolved = true;
        if (puzzle != null)
        {
            puzzle.OnHackSuccessful();
        }
    }
}
