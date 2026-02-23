using UnityEngine;
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private string doorId = "door_001";
    [SerializeField] private int requiredEnergyCores = 0;
    [SerializeField] private bool requiresKeyCard = true;
    [SerializeField] private string requiredKeyCardId = "";
    [Tooltip("Если включено: дверь откроется только когда эта головоломка решена (отдельно от головоломки на ключе).")]
    [SerializeField] private bool requiresPuzzle = false;
    [SerializeField] private GameObject requiredPuzzle;
    [Header("Animation")]
    [SerializeField] private bool useAnimation = true;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private float openSpeed = 2f;
    private bool isOpen = false;
    private bool isUnlocked = false;
    private int currentEnergyCores = 0;
    private bool waitingForPuzzle = false;
    private GameObject pendingPuzzleObject = null;

    private void OnDisable()
    {
        EventBus.OnPuzzleSolved -= OnPuzzleSolved;
    }

    private void Start()
    {
        if (useAnimation)
        {
            closedPosition = transform.position;
            openPosition = closedPosition + Vector3.up * 3f;
        }
    }
    private void Update()
    {
        if (isOpen && useAnimation)
        {
            transform.position = Vector3.Lerp(transform.position, openPosition, Time.deltaTime * openSpeed);
        }
    }
    public void TryOpen()
    {
        if (isOpen)
            return;
        // 1. Сначала головоломка двери (если есть и не решена)
        if (requiresPuzzle && requiredPuzzle != null)
        {
            var puzzle = requiredPuzzle.GetComponent<BasePuzzle>();
            if (puzzle != null && !puzzle.IsSolved)
            {
                pendingPuzzleObject = requiredPuzzle;
                waitingForPuzzle = true;
                EventBus.OnPuzzleSolved += OnPuzzleSolved;
                puzzle.StartPuzzle();
                return;
            }
        }
        // 2. Ключ
        bool hasKey = InventoryManager.Instance != null && !string.IsNullOrEmpty(requiredKeyCardId) && InventoryManager.Instance.HasKeyCard(requiredKeyCardId);
        if (requiresKeyCard && !string.IsNullOrEmpty(requiredKeyCardId) && !hasKey)
        {
            PlayLockedSound();
            return;
        }
        // 3. Головоломка ключа (если у ключа задана и не решена)
        GameObject keyPuzzle = hasKey && InventoryManager.Instance != null ? InventoryManager.Instance.GetPuzzleForKey(requiredKeyCardId) : null;
        if (keyPuzzle != null)
        {
            var keyPuzzleComp = keyPuzzle.GetComponent<BasePuzzle>();
            if (keyPuzzleComp != null && !keyPuzzleComp.IsSolved)
            {
                pendingPuzzleObject = keyPuzzle;
                waitingForPuzzle = true;
                EventBus.OnPuzzleSolved += OnPuzzleSolved;
                keyPuzzleComp.StartPuzzle();
                return;
            }
        }
        // 4. Энергоядра
        if (currentEnergyCores < requiredEnergyCores)
        {
            PlayLockedSound();
            return;
        }
        Open();
    }

    private void OnPuzzleSolved(GameObject solvedPuzzleObject)
    {
        if (solvedPuzzleObject != pendingPuzzleObject || !waitingForPuzzle)
            return;
        waitingForPuzzle = false;
        pendingPuzzleObject = null;
        EventBus.OnPuzzleSolved -= OnPuzzleSolved;
        TryOpen();
    }
    public void Open()
    {
        if (isOpen)
            return;
        isOpen = true;
        isUnlocked = true;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GetDoorOpenSound(), transform.position);
        }
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
    public void AddEnergyCore()
    {
        currentEnergyCores++;
        if (currentEnergyCores >= requiredEnergyCores)
        {
            isUnlocked = true;
        }
    }
    private void PlayLockedSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GetDoorLockedSound(), transform.position);
        }
    }
    public string DoorId => doorId;
    public bool IsOpen => isOpen;
    public bool IsUnlocked => isUnlocked;
}
