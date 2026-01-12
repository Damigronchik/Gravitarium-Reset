using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
public class JournalUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private RectTransform notesContainer;
    [SerializeField] private GameObject noteEntryPrefab;
    [SerializeField] private TextMeshProUGUI noteTitleText;
    [SerializeField] private TextMeshProUGUI noteContentText;
    [SerializeField] private Button closeButton;
    private Dictionary<string, GameObject> noteEntryButtons = new Dictionary<string, GameObject>();
    private InputSystem inputActions;
    private void Awake()
    {
        inputActions = new InputSystem();
    }
    private void Start()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
        }
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseJournal);
        }
        SubscribeToEvents();
        RefreshJournal();
    }
    private void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Enable();
            inputActions.Player.Journal.performed += OnJournalPressed;
        }
    }
    private void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.Player.Journal.performed -= OnJournalPressed;
            inputActions.Player.Disable();
        }
    }
    private void OnJournalPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleJournal();
        }
    }
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    private void SubscribeToEvents()
    {
        EventBus.OnNoteCollected += OnNoteCollected;
    }
    private void UnsubscribeFromEvents()
    {
        EventBus.OnNoteCollected -= OnNoteCollected;
    }
    private void OnNoteCollected(GameObject note, string noteId)
    {
        RefreshJournal();
    }
    private void RefreshJournal()
    {
        if (notesContainer == null)
        {
            Debug.LogWarning("JournalUI: notesContainer is null!");
            return;
        }
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("JournalUI: InventoryManager.Instance is null!");
            return;
        }
        if (!notesContainer.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("JournalUI: notesContainer is not active in hierarchy!");
        }
        foreach (var entry in noteEntryButtons.Values)
        {
            if (entry != null)
            {
                Destroy(entry);
            }
        }
        noteEntryButtons.Clear();
        var allNotes = InventoryManager.Instance.GetAllNotes();
        Debug.Log($"JournalUI: Found {allNotes.Count} notes in inventory. NotesContainer active: {notesContainer.gameObject.activeInHierarchy}");
        foreach (var kvp in allNotes)
        {
            if (kvp.Value != null)
            {
                Debug.Log($"JournalUI: Adding note to journal: {kvp.Value.noteId} - {kvp.Value.noteTitle}");
                AddNoteToJournal(kvp.Value);
            }
        }
        if (notesContainer != null)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(notesContainer);
        }
    }
    private void AddNoteToJournal(NoteData noteData)
    {
        if (noteEntryPrefab == null)
        {
            Debug.LogWarning("JournalUI: noteEntryPrefab is null!");
            return;
        }
        if (notesContainer == null)
        {
            Debug.LogWarning("JournalUI: notesContainer is null!");
            return;
        }
        if (noteData == null)
        {
            Debug.LogWarning("JournalUI: noteData is null!");
            return;
        }
        if (noteEntryButtons.ContainsKey(noteData.noteId))
        {
            Debug.Log($"JournalUI: Note {noteData.noteId} already in journal, skipping");
            return;
        }
        GameObject entry = Instantiate(noteEntryPrefab);
        RectTransform entryRect = entry.GetComponent<RectTransform>();
        if (entryRect != null && notesContainer != null)
        {
            entryRect.SetParent(notesContainer, false);
            entryRect.localScale = Vector3.one;
            entry.SetActive(true);
            Debug.Log($"JournalUI: Entry RectTransform configured. Position: {entryRect.anchoredPosition}, Size: {entryRect.sizeDelta}, Active: {entry.activeSelf}");
        }
        else
        {
            if (entryRect == null)
        {
                Debug.LogWarning($"JournalUI: Entry prefab doesn't have RectTransform component!");
            }
            if (notesContainer == null)
            {
                Debug.LogWarning($"JournalUI: notesContainer is null!");
        }
    }
        Button entryButton = entry.GetComponent<Button>();
        TextMeshProUGUI entryTextTMP = entry.GetComponentInChildren<TextMeshProUGUI>();
        Text entryText = entryTextTMP == null ? entry.GetComponentInChildren<Text>() : null;
        if (entryTextTMP != null)
        {
            entryTextTMP.text = noteData.noteTitle;
            Debug.Log($"JournalUI: Set TextMeshProUGUI text to: {noteData.noteTitle}");
        }
        else if (entryText != null)
        {
            entryText.text = noteData.noteTitle;
            Debug.Log($"JournalUI: Set Text text to: {noteData.noteTitle}");
        }
        else
        {
            Debug.LogWarning($"JournalUI: No Text or TextMeshProUGUI found in note entry prefab!");
        }
        if (entryButton != null)
        {
            entryButton.onClick.AddListener(() => SelectNote(noteData));
        }
        else
        {
            Debug.LogWarning($"JournalUI: No Button component found in note entry prefab!");
        }
        noteEntryButtons[noteData.noteId] = entry;
        Debug.Log($"JournalUI: Successfully added note {noteData.noteId} to journal. Entry active: {entry.activeSelf}, Entry visible in hierarchy: {entry.activeInHierarchy}");
    }
    private void SelectNote(NoteData noteData)
        {
        if (noteTitleText != null)
            {
            noteTitleText.text = noteData.noteTitle;
            }
        if (noteContentText != null)
        {
            noteContentText.text = noteData.noteText;
        }
    }
    public void ToggleJournal()
    {
        if (journalPanel != null)
        {
            bool isActive = journalPanel.activeSelf;
            journalPanel.SetActive(!isActive);
            if (!isActive)
            {
                RefreshJournal();
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    private void CloseJournal()
    {
        if (journalPanel != null)
        {
            journalPanel.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
