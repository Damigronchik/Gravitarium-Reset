using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class ItemDragSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image crosshairImage;
    [Header("Settings")]
    [SerializeField] private float dragDistance = 10f;
    [SerializeField] private float maxDragDistance = 5f;
    [SerializeField] private LayerMask draggableLayer = 1;
    [Header("Particles")]
    [SerializeField] private ParticleSystem dragParticles;
    private Camera playerCamera;
    private DraggableItem3D currentDraggedItem = null;
    private bool isDragging = false;
    private RaycastHit hit;
    private InputSystem inputActions;
    private bool isMouseHeld = false;
    private void Awake()
    {
        inputActions = new InputSystem();
    }
    private void Start()
    {
        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            playerCamera = FindObjectOfType<Camera>();
        }
        if (crosshairImage != null)
        {
            crosshairImage.gameObject.SetActive(true);
        }
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.performed += OnMouseDown;
        inputActions.Player.Attack.canceled += OnMouseUp;
    }
    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnMouseDown;
        inputActions.Player.Attack.canceled -= OnMouseUp;
        inputActions.Player.Disable();
    }
    private void Update()
    {
        if (isMouseHeld && isDragging)
        {
            UpdateDrag();
        }
        UpdateParticles();
    }
    private void OnMouseDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isMouseHeld = true;
            StartDrag();
        }
    }
    private void OnMouseUp(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            isMouseHeld = false;
            EndDrag();
        }
    }
    private void StartDrag()
    {
        if (playerCamera == null) return;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out hit, maxDragDistance, draggableLayer))
        {
            DraggableItem3D item = hit.collider.GetComponent<DraggableItem3D>();
            if (item != null && !item.IsPlaced)
            {
                currentDraggedItem = item;
                isDragging = true;
                item.StartDrag();
            }
        }
    }
    private void UpdateDrag()
    {
        if (currentDraggedItem == null || playerCamera == null) return;
        Vector3 targetPosition = playerCamera.transform.position + 
                                 playerCamera.transform.forward * dragDistance;
        currentDraggedItem.UpdateDrag(targetPosition);
    }
    private void EndDrag()
    {
        if (currentDraggedItem != null)
        {
            currentDraggedItem.EndDrag();
            currentDraggedItem = null;
        }
        isDragging = false;
    }
    private void UpdateParticles()
    {
        if (dragParticles == null) return;
        bool shouldPlay = isDragging && currentDraggedItem != null;
        if (shouldPlay)
        {
            if (!dragParticles.isPlaying)
            {
                dragParticles.Play();
            }
            if (currentDraggedItem != null)
            {
                dragParticles.transform.position = currentDraggedItem.transform.position;
            }
        }
        else
        {
            if (dragParticles.isPlaying)
            {
                dragParticles.Stop();
            }
        }
    }
    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (crosshairImage != null)
        {
            crosshairImage.gameObject.SetActive(enabled);
        }
        if (!enabled && isDragging)
        {
            EndDrag();
        }
    }
}
