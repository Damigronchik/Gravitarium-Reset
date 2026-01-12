using UnityEngine;
public class NarrativeNote : MonoBehaviour
{
    [Header("Note Settings")]
    [SerializeField] private string noteId = "note_001";
    [SerializeField] private string noteTitle = "Записка";
    [SerializeField] [TextArea(5, 10)] private string noteText = "Текст записки...";
    [Header("Visual")]
    [SerializeField] private GameObject visualObject;
    [SerializeField] private float rotationSpeed = 45f;
    [Header("Visual Effects (Deprecated)")]
    [Tooltip("Используется ObjectPoolManager для эффектов. Оставьте пустым.")]
    [SerializeField] private ParticleSystem collectEffect;
    private bool isCollected = false;
    private void Start()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.HasNote(noteId))
        {
            isCollected = true;
            if (visualObject != null)
            {
                visualObject.SetActive(false);
            }
            gameObject.SetActive(false);
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            return;
        }
    }
    private void Update()
    {
        if (!isCollected && visualObject != null)
        {
            visualObject.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
    public void Collect()
    {
        if (isCollected)
            return;
        isCollected = true;
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.SpawnFromPool("ItemCollect", transform.position, Quaternion.identity);
        }
        else if (collectEffect != null)
        {
            collectEffect.Play();
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GetItemCollectSound(), transform.position);
        }
        if (visualObject != null)
        {
            visualObject.SetActive(false);
        }
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddNote(noteId, noteTitle, noteText);
        }
        else
        {
            Debug.LogError($"NarrativeNote: InventoryManager.Instance is null! Cannot add note {noteId}");
        }
        EventBus.InvokeNoteCollected(gameObject, noteId);
        EventBus.InvokeItemCollected(gameObject);
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    public string NoteId => noteId;
    public string NoteTitle => noteTitle;
    public string NoteText => noteText;
    public bool IsCollected => isCollected;
}
