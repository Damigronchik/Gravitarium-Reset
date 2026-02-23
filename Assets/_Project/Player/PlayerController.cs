using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GravitySystem))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private float airControlMultiplier = 0.3f;
    [Header("Mouse Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float verticalLookLimit = 80f;
    [SerializeField] private Transform cameraTransform;
    [Header("Item Drag System")]
    [SerializeField] private ItemDragSystem itemDragSystem;
    [Header("Interaction Settings")]
    [SerializeField] private float maxInteractionDistance = 5f;
	[SerializeField] private Rigidbody _rigidbody;
	[SerializeField] private Help help;
    private GravitySystem gravitySystem;
    private InputSystem inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalRotation;
    private bool isSprinting;
    private bool isGrounded;
    private Vector3 moveDirection;
    private float currentRotationY = 0f;
    private bool inputEnabled = true;
    public bool IsGrounded => isGrounded;
    public Vector3 Velocity => _rigidbody.linearVelocity;
    public bool InputEnabled 
    { 
        get => inputEnabled; 
        set => inputEnabled = value; 
    }

    private void Awake()
    {
        gravitySystem = GetComponent<GravitySystem>();
        inputActions = new InputSystem();
        _rigidbody.freezeRotation = true;
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        currentRotationY = transform.eulerAngles.y;
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
            else
            {
                cameraTransform = GetComponentInChildren<Camera>()?.transform;
            }
        }
        if (cameraTransform != null)
        {
            verticalRotation = cameraTransform.localEulerAngles.x;
            if (verticalRotation > 180f) verticalRotation -= 360f;
        }
        if (itemDragSystem == null)
        {
            itemDragSystem = FindObjectOfType<ItemDragSystem>();
        }
    }
    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;
        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;
        inputActions.Player.FlipGravity.performed += OnFlipGravity;
        inputActions.Player.Help.performed += OnUseHeap;
        inputActions.Player.Interact.performed += OnInteract;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;
        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;
        inputActions.Player.FlipGravity.performed -= OnFlipGravity;
        inputActions.Player.Help.performed -= OnUseHeap;
        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void Update()
    {
        if (!inputEnabled) return;
        if (Time.timeScale == 0f || (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Paused))
        {
            return;
        }
        CheckGrounded();
        HandleMouseLook();
        HandleMovement();
        CheckInteractiveObjects();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        Quaternion targetRotation = Quaternion.Euler(0, currentRotationY, 0);
        _rigidbody.MoveRotation(targetRotation);
    }
    private void CheckGrounded()
    {
        Vector3 checkDirection = gravitySystem.IsGravityFlipped ? Vector3.up : Vector3.down;
        isGrounded = Physics.Raycast(transform.position, checkDirection, groundCheckDistance + 0.1f, groundLayer);
    }
    private void HandleMouseLook()
    {
        if (gravitySystem.IsFlipping ||
            Time.timeScale == 0f || 
            (GameManager.Instance != null && GameManager.Instance.CurrentState == GameManager.GameState.Paused) ||
            lookInput.magnitude < 0.01f) 
        {
            return;
        }
        bool flipped = gravitySystem.IsGravityFlipped;
        float mouseX = lookInput.x * mouseSensitivity * (flipped ? -1f : 1f);
        float mouseY = lookInput.y * mouseSensitivity;
        currentRotationY += mouseX;
        if (cameraTransform != null)
        {
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
            float pitch = flipped ? -verticalRotation : verticalRotation;
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
    private void HandleMovement()
    {
        if (moveInput.magnitude < 0.1f)
        {
            moveDirection = Vector3.zero;
            return;
        }
        float invertX = gravitySystem.IsGravityFlipped ? -1f : 1f;
        Vector2 effectiveInput = new Vector2(moveInput.x * invertX, moveInput.y);
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        moveDirection = (forward * effectiveInput.y + right * effectiveInput.x).normalized;
        EventBus.InvokePlayerMoved(transform.position);
    }
    private void ApplyMovement()
    {
        float speed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        Vector3 currentVelocity = _rigidbody.linearVelocity;
        Vector3 targetHorizontalVelocity = Vector3.zero;
        if (moveDirection.magnitude > 0.1f)
        {
            targetHorizontalVelocity = moveDirection * speed;
        }
        float controlMultiplier = isGrounded ? 1f : airControlMultiplier;
        Vector3 newVelocity = currentVelocity;
        if (isGrounded)
        {
            newVelocity.x = targetHorizontalVelocity.x;
            newVelocity.z = targetHorizontalVelocity.z;
        }
        else
        {
            newVelocity.x = Mathf.Lerp(currentVelocity.x, targetHorizontalVelocity.x, controlMultiplier);
            newVelocity.z = Mathf.Lerp(currentVelocity.z, targetHorizontalVelocity.z, controlMultiplier);
        }
        _rigidbody.linearVelocity = newVelocity;
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    private void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.performed;
    }
    private void OnFlipGravity(InputAction.CallbackContext context)
    {
        gravitySystem.FlipGravity();
    }
	private void OnUseHeap(InputAction.CallbackContext context)
	{
		help.ToggleHelp();
	}
	private void CheckInteractiveObjects()
    {
        if (cameraTransform == null) return;
        RaycastHit hit;
        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;
        GameHUD gameHUD = FindObjectOfType<GameHUD>();
        if (gameHUD == null) return;
        if (Physics.Raycast(origin, direction, out hit, maxInteractionDistance))
        {
            string hintText = null;
            var terminal = hit.collider.GetComponent<Terminal>();
            if (terminal != null)
            {
                hintText = "Нажмите E для взаимодействия с терминалом";
            }
            else
            {
                var door = hit.collider.GetComponent<Door>();
                if (door != null)
                {
                    if (door.IsOpen)
                    {
                        hintText = null;
                    }
                    else
                    {
                        hintText = "Нажмите E для открытия двери";
                    }
                }
                else
                {
                    var energyCore = hit.collider.GetComponent<EnergyCore>();
                    if (energyCore != null && !energyCore.IsCollected)
                    {
                        hintText = "Нажмите E для сбора энергетического ядра";
                    }
                    else
                    {
                        var keyCard = hit.collider.GetComponent<KeyCard>();
                        if (keyCard != null && !keyCard.IsCollected)
                        {
                            hintText = "Нажмите E для сбора ключ-карты";
                        }
                        else
                        {
                            var narrativeNote = hit.collider.GetComponent<NarrativeNote>();
                            if (narrativeNote != null && !narrativeNote.IsCollected)
                            {
                                hintText = "Нажмите E для сбора записки";
                            }
                            else
                            {
                                var draggableItem = hit.collider.GetComponent<DraggableItem3D>();
                                if (draggableItem != null && !draggableItem.IsDragging && !draggableItem.IsPlaced)
                                {
                                    hintText = "Зажмите ЛКМ чтобы перетянуть объект";
                                }
                            }
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(hintText))
            {
                gameHUD.ShowInteractionHint(hintText);
            }
            else
            {
                gameHUD.HideInteractionHint();
            }
        }
        else
        {
            gameHUD.HideInteractionHint();
        }
    }
    private void OnInteract(InputAction.CallbackContext context)
    {
        if (cameraTransform == null) return;
        RaycastHit hit;
        Vector3 origin = cameraTransform.position;
        Vector3 direction = cameraTransform.forward;
        if (Physics.Raycast(origin, direction, out hit, maxInteractionDistance))
        {
            var terminal = hit.collider.GetComponent<Terminal>();
            if (terminal != null) { terminal.Interact(); return; }
            var door = hit.collider.GetComponent<Door>();
            if (door != null) { door.TryOpen(); return; }
            var energyCore = hit.collider.GetComponent<EnergyCore>();
            if (energyCore != null) { energyCore.Collect(); return; }
            var keyCard = hit.collider.GetComponent<KeyCard>();
            if (keyCard != null) { keyCard.Collect(); return; }
            var narrativeNote = hit.collider.GetComponent<NarrativeNote>();
            if (narrativeNote != null) { narrativeNote.Collect(); return; }
        }
    }
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}
