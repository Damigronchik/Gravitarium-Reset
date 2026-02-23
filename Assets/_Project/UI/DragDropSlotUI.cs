using UnityEngine;

/// <summary>
/// Слот на канвасе, в который нужно перетащить соответствующую фигуру.
/// Вешается на RectTransform (Panel/Image). Прицел в центре экрана — проверка через ContainsScreenPoint.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class DragDropSlotUI : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// Проверяет, попадает ли точка экрана (например, центр — прицел) в слот.
    /// </summary>
    public bool ContainsScreenPoint(Vector2 screenPoint)
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint, cam);
    }
}
