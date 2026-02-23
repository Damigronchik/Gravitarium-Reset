using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Головоломка «перетащи фигуру на место» на канвасе. Наследует BasePuzzle.
/// Прицел в центре экрана: луч из (Screen.width/2, Screen.height/2), ЛКМ (Attack) — перетаскивание и дроп.
/// </summary>
public class DragDropPuzzleUI : BasePuzzle
{
    [Header("Drag & Drop UI")]
    [SerializeField] private Canvas puzzleCanvas;
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private RectTransform shuffleArea;
    [SerializeField] private DraggableShapeUI[] shapes;
    [SerializeField] private DragDropSlotUI[] slots;

    private bool isActive;
    private bool isClosing;
    private DraggableShapeUI currentDragged;
    private bool isMouseHeld;
    private InputSystem inputActions;
    private Camera eventCamera;
    private int placedCount;

    private static Vector2 ScreenCenter => new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

    private void Awake()
    {
        inputActions = new InputSystem();
        if (puzzleCanvas != null)
        {
            eventCamera = puzzleCanvas.renderMode != RenderMode.ScreenSpaceOverlay ? puzzleCanvas.worldCamera : null;
            if (raycaster == null)
                raycaster = puzzleCanvas.GetComponent<GraphicRaycaster>();
        }
    }

    private void Start()
    {
        if (shapes == null || shapes.Length == 0)
            Debug.LogWarning($"DragDropPuzzleUI {puzzleId}: No shapes assigned!");
        gameObject.SetActive(false);
        if (puzzleCanvas != null)
            puzzleCanvas.gameObject.SetActive(false);
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
            isMouseHeld = true;
    }

    private void OnMouseUp(InputAction.CallbackContext context)
    {
        if (context.canceled)
            isMouseHeld = false;
    }

    private void Update()
    {
        if (!isActive || isClosing) return;

        if (isMouseHeld && currentDragged != null)
            FollowCrosshair();
        else if (isMouseHeld && currentDragged == null)
            TryStartDrag();
        else if (!isMouseHeld && currentDragged != null)
            TryDrop();
    }

    public override void StartPuzzle()
    {
        base.StartPuzzle();
        OpenPuzzle();
    }

    public void OpenPuzzle()
    {
        isClosing = false;
        isActive = false;
        gameObject.SetActive(true);
        if (puzzleCanvas != null)
            puzzleCanvas.gameObject.SetActive(true);
        currentDragged = null;
        ShufflePositions();
        var itemDragSystem = FindObjectOfType<ItemDragSystem>();
        if (itemDragSystem != null)
            itemDragSystem.SetEnabled(false);
        isActive = true;
    }

    public void ClosePuzzle()
    {
        if (isClosing) return;
        isClosing = true;
        isActive = false;
        currentDragged = null;
        var itemDragSystem = FindObjectOfType<ItemDragSystem>();
        if (itemDragSystem != null)
            itemDragSystem.SetEnabled(true);
        if (puzzleCanvas != null)
            puzzleCanvas.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void ShufflePositions()
    {
        RectTransform area = shuffleArea;
        if (area == null && shapes != null && shapes.Length > 0 && shapes[0] != null)
            area = shapes[0].RectTransform.parent as RectTransform;
        if (area == null) return;
        Rect rect = area.rect;
        float margin = 20f;
        float minX = rect.xMin + margin;
        float maxX = rect.xMax - margin;
        float minY = rect.yMin + margin;
        float maxY = rect.yMax - margin;
        if (shapes != null)
        {
            foreach (var shape in shapes)
            {
                if (shape == null || shape.RectTransform == null) continue;
                Vector2 pos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                shape.RectTransform.anchoredPosition = pos;
                shape.RefreshStartState();
            }
        }
        if (slots != null)
        {
            foreach (var slot in slots)
            {
                if (slot == null) continue;
                var rt = slot.GetComponent<RectTransform>();
                if (rt != null)
                    rt.anchoredPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            }
        }
    }

    private void TryStartDrag()
    {
        DraggableShapeUI hit = RaycastAtCenter<DraggableShapeUI>();
        if (hit != null && !hit.IsPlaced)
            currentDragged = hit;
    }

    private void FollowCrosshair()
    {
        if (currentDragged == null || currentDragged.RectTransform == null) return;
        RectTransform rect = currentDragged.RectTransform;
        RectTransform parent = rect.parent as RectTransform;
        if (parent == null) return;
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, ScreenCenter, eventCamera, out localPoint))
        {
            rect.localPosition = new Vector3(localPoint.x, localPoint.y, rect.localPosition.z);
        }
    }

    private void TryDrop()
    {
        if (currentDragged == null) return;
        bool overCorrect = currentDragged.CorrectSlot != null && currentDragged.CorrectSlot.ContainsScreenPoint(ScreenCenter);
        if (overCorrect)
        {
            currentDragged.Place();
            OnShapePlaced();
        }
        else
        {
            currentDragged.ResetItem();
        }
        currentDragged = null;
    }

    private void OnShapePlaced()
    {
        if (shapes == null) return;
        placedCount = 0;
        foreach (var shape in shapes)
        {
            if (shape != null && shape.IsPlaced)
                placedCount++;
        }
        int total = shapes.Length;
        float progress = total > 0 ? (float)placedCount / total : 0f;
        UpdateProgress(progress);
        if (placedCount == total && total > 0)
        {
            SolvePuzzle();
            ClosePuzzle();
        }
    }

    public override void ResetPuzzle()
    {
        base.ResetPuzzle();
        ClosePuzzle();
        if (shapes != null)
        {
            foreach (var shape in shapes)
            {
                if (shape != null)
                    shape.ResetItem();
            }
        }
        placedCount = 0;
    }

    protected override void OnPuzzleStateRestored()
    {
        base.OnPuzzleStateRestored();
        if (currentState == PuzzleState.Solved && shapes != null)
        {
            foreach (var shape in shapes)
            {
                if (shape != null && !shape.IsPlaced && shape.CorrectSlot != null)
                    shape.Place();
            }
        }
    }

    private T RaycastAtCenter<T>() where T : Component
    {
        if (raycaster == null || EventSystem.current == null) return null;
        var pointerData = new PointerEventData(EventSystem.current) { position = ScreenCenter };
        var results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);
        if (results.Count == 0)
            EventSystem.current.RaycastAll(pointerData, results);
        foreach (var result in results)
        {
            var c = result.gameObject.GetComponent<T>();
            if (c != null) return c;
            c = result.gameObject.GetComponentInParent<T>();
            if (c != null) return c;
        }
        return null;
    }
}
