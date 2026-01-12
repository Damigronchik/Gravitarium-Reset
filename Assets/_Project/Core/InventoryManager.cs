using UnityEngine;
using System.Collections.Generic;
public class InventoryManager : MonoBehaviour
{
    private HashSet<string> collectedKeyCards = new HashSet<string>();
    private HashSet<string> collectedEnergyCores = new HashSet<string>();
    private Dictionary<string, NoteData> collectedNotes = new Dictionary<string, NoteData>();
    private static InventoryManager instance;
    public static InventoryManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InventoryManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("InventoryManager");
                    instance = go.AddComponent<InventoryManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void AddKeyCard(string keyCardId)
    {
        if (!string.IsNullOrEmpty(keyCardId))
        {
            collectedKeyCards.Add(keyCardId);
            Debug.Log($"Key card {keyCardId} added to inventory");
        }
    }
    public bool HasKeyCard(string keyCardId)
    {
        if (string.IsNullOrEmpty(keyCardId))
            return false;
        return collectedKeyCards.Contains(keyCardId);
    }
    public void RemoveKeyCard(string keyCardId)
    {
        if (!string.IsNullOrEmpty(keyCardId))
        {
            collectedKeyCards.Remove(keyCardId);
        }
    }
    public void AddEnergyCore(string coreId)
    {
        if (!string.IsNullOrEmpty(coreId))
        {
            collectedEnergyCores.Add(coreId);
            Debug.Log($"Energy core {coreId} added to inventory");
        }
    }
    public bool HasEnergyCore(string coreId)
    {
        if (string.IsNullOrEmpty(coreId))
            return false;
        return collectedEnergyCores.Contains(coreId);
    }
    public void RemoveEnergyCore(string coreId)
    {
        if (!string.IsNullOrEmpty(coreId))
        {
            collectedEnergyCores.Remove(coreId);
        }
    }
    public int GetEnergyCoreCount()
    {
        return collectedEnergyCores.Count;
    }
    public HashSet<string> GetCollectedEnergyCores()
    {
        return new HashSet<string>(collectedEnergyCores);
    }
    public void ClearInventory()
    {
        collectedKeyCards.Clear();
        collectedEnergyCores.Clear();
        collectedNotes.Clear();
    }
    public HashSet<string> GetCollectedKeyCards()
    {
        return new HashSet<string>(collectedKeyCards);
    }
    public void AddNote(string noteId, string noteTitle, string noteText)
    {
        if (!string.IsNullOrEmpty(noteId))
        {
            collectedNotes[noteId] = new NoteData(noteId, noteTitle, noteText);
            Debug.Log($"Note {noteId} added to inventory");
        }
    }
    public bool HasNote(string noteId)
    {
        if (string.IsNullOrEmpty(noteId))
            return false;
        return collectedNotes.ContainsKey(noteId);
    }
    public Dictionary<string, NoteData> GetAllNotes()
    {
        return new Dictionary<string, NoteData>(collectedNotes);
    }
}
