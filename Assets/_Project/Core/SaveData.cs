using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class NoteData
{
    public string noteId;
    public string noteTitle;
    public string noteText;
    public NoteData()
    {
        noteId = "";
        noteTitle = "";
        noteText = "";
    }
    public NoteData(string id, string title, string text)
    {
        noteId = id;
        noteTitle = title;
        noteText = text;
    }
}
[Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public bool isGravityFlipped;
    public float playerHealth;
    public float playerMaxHealth;
    public float playerEnergy;
    public float playerMaxEnergy;
    public List<string> collectedKeyCards = new List<string>();
    public List<string> collectedEnergyCores = new List<string>();
    public List<NoteData> collectedNotes = new List<NoteData>();
    public List<string> solvedPuzzleIds = new List<string>();
    public List<string> disabledHazardNames = new List<string>();
    public string currentLevel;
    public string saveDate;
    public float playTime;
    public SaveData()
    {
        playerPosition = Vector3.zero;
        playerRotation = Quaternion.identity;
        isGravityFlipped = false;
        playerHealth = 100f;
        playerMaxHealth = 100f;
        playerEnergy = 100f;
        playerMaxEnergy = 100f;
        currentLevel = "Level01_StationHub";
        saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        playTime = 0f;
    }
}
