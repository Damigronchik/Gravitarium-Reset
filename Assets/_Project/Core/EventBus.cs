using System;
using UnityEngine;
using UnityEngine.Events;
public static class EventBus
{
    public static event Action<Vector3> OnGravityFlipped;
    public static void InvokeGravityFlipped(Vector3 newGravity) => OnGravityFlipped?.Invoke(newGravity);
    public static event Action<GameObject> OnItemCollected;
    public static void InvokeItemCollected(GameObject item) => OnItemCollected?.Invoke(item);
    public static event Action<GameObject> OnEnergyCoreCollected;
    public static void InvokeEnergyCoreCollected(GameObject energyCore) => OnEnergyCoreCollected?.Invoke(energyCore);
    public static event Action<GameObject, string> OnKeyCardCollected;
    public static void InvokeKeyCardCollected(GameObject keyCard, string keyCardId) => OnKeyCardCollected?.Invoke(keyCard, keyCardId);
    public static event Action<GameObject> OnPuzzleSolved;
    public static void InvokePuzzleSolved(GameObject puzzle) => OnPuzzleSolved?.Invoke(puzzle);
    public static event Action<GameObject, float> OnPuzzleProgressed;
    public static void InvokePuzzleProgressed(GameObject puzzle, float progress) => OnPuzzleProgressed?.Invoke(puzzle, progress);
    public static event Action<string> OnLevelCompleted;
    public static void InvokeLevelCompleted(string levelName) => OnLevelCompleted?.Invoke(levelName);
    public static event Action<string> OnLoadingStarted;
    public static void InvokeLoadingStarted(string sceneName) => OnLoadingStarted?.Invoke(sceneName);
    public static event Action<string> OnLevelStarted;
    public static void InvokeLevelStarted(string levelName) => OnLevelStarted?.Invoke(levelName);
    public static event Action<string> OnLevelLoaded;
    public static void InvokeLevelLoaded(string levelName) => OnLevelLoaded?.Invoke(levelName);
    public static event Action<Vector3> OnPlayerMoved;
    public static void InvokePlayerMoved(Vector3 position) => OnPlayerMoved?.Invoke(position);
    public static event Action OnPlayerJumped;
    public static void InvokePlayerJumped() => OnPlayerJumped?.Invoke();
    public static event Action OnPlayerDeath;
    public static void InvokePlayerDeath() => OnPlayerDeath?.Invoke();
    public static event Action<GameObject> OnTerminalActivated;
    public static void InvokeTerminalActivated(GameObject terminal) => OnTerminalActivated?.Invoke(terminal);
    public static event Action<GameObject, string> OnNoteCollected;
    public static void InvokeNoteCollected(GameObject note, string noteId) => OnNoteCollected?.Invoke(note, noteId);
    public static event Action OnGamePaused;
    public static void InvokeGamePaused() => OnGamePaused?.Invoke();
    public static event Action OnGameResumed;
    public static void InvokeGameResumed() => OnGameResumed?.Invoke();
    public static void ClearAllEvents()
    {
        OnGravityFlipped = null;
        OnItemCollected = null;
        OnEnergyCoreCollected = null;
        OnKeyCardCollected = null;
        OnPuzzleSolved = null;
        OnPuzzleProgressed = null;
        OnLevelCompleted = null;
        OnLoadingStarted = null;
        OnLevelStarted = null;
        OnLevelLoaded = null;
        OnPlayerMoved = null;
        OnPlayerJumped = null;
        OnPlayerDeath = null;
        OnTerminalActivated = null;
        OnNoteCollected = null;
        OnGamePaused = null;
        OnGameResumed = null;
    }
}
