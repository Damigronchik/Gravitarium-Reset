using UnityEngine;
using System.Collections;
public class DraggableItem3D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float dragDistance = 2f;
    [SerializeField] private LayerMask dropZoneLayer = 1;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isDragging = false;
    private bool isPlaced = false;
    public bool IsDragging => isDragging;
    public bool IsPlaced => isPlaced;
    private void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }
    public void StartDrag()
    {
        if (isPlaced) return;
        isDragging = true;
    }
    public void UpdateDrag(Vector3 targetPosition)
    {
        if (!isDragging) return;
        transform.position = targetPosition;
    }
    public void EndDrag()
    {
        if (!isDragging) return;
        isDragging = false;
    }
    public void Place(Vector3 position)
    {
        isPlaced = true;
        transform.position = position;
    }
    public void ResetItem()
    {
        isPlaced = false;
        isDragging = false;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
}
