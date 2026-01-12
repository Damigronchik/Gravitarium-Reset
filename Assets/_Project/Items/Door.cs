using UnityEngine;
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private string doorId = "door_001";
    [SerializeField] private int requiredEnergyCores = 0;
    [SerializeField] private bool requiresKeyCard = true;
    [SerializeField] private string requiredKeyCardId = "";
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
        if (requiresPuzzle && requiredPuzzle != null)
        {
            var puzzle = requiredPuzzle.GetComponent<BasePuzzle>();
            if (puzzle == null || !puzzle.IsSolved)
            {
                PlayLockedSound();
                return;
            }
        }
        if (requiresKeyCard && !string.IsNullOrEmpty(requiredKeyCardId))
        {
            if (InventoryManager.Instance == null || !InventoryManager.Instance.HasKeyCard(requiredKeyCardId))
            {
                PlayLockedSound();
                return;
            }
        }
        if (currentEnergyCores < requiredEnergyCores)
        {
            PlayLockedSound();
            return;
        }
        Open();
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
