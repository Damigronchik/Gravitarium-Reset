using UnityEngine;
using System.IO;
using System.Collections.Generic;
public class SaveManager : MonoBehaviour
{
    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "savegame.json";
    private string saveDirectory;
    private SaveData currentSaveData;
    private float sessionStartTime;
    private bool isRestoringPlayer = false;
    private static SaveManager instance;
    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveManager>();
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
            InitializeSaveDirectory();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SubscribeToEvents();
        sessionStartTime = Time.time;
    }
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    private void InitializeSaveDirectory()
    {
        saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }
    }
    private void SubscribeToEvents()
    {
    }
    private void UnsubscribeFromEvents()
    {
    }
    public void SaveGame()
    {
        SaveData saveData = CollectSaveData();
        string filePath = GetSaveFilePath();
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(filePath, json);
            currentSaveData = saveData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }
    public bool LoadGame()
    {
        string filePath = GetSaveFilePath();
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Save file not found: {filePath}");
            return false;
        }
        try
        {
            string json = File.ReadAllText(filePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);
            if (saveDirectory == null || string.IsNullOrEmpty(saveDirectory))
            {
                InitializeSaveDirectory();
            }
            StartCoroutine(LoadGameCoroutine(saveData));
            currentSaveData = saveData;
            sessionStartTime = Time.time;
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return false;
        }
    }
    private System.Collections.IEnumerator LoadGameCoroutine(SaveData saveData)
    {
        yield return null;
        ApplySaveData(saveData);
    }
    private SaveData CollectSaveData()
    {
        SaveData saveData = new SaveData();
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            saveData.playerPosition = player.transform.position;
            saveData.playerRotation = player.transform.rotation;
            var gravitySystem = player.GetComponent<GravitySystem>();
            if (gravitySystem != null)
            {
                saveData.isGravityFlipped = gravitySystem.IsGravityFlipped;
            }
            var stats = player.GetComponent<PlayerStatsComponent>();
            if (stats != null)
            {
                saveData.playerHealth = stats.CurrentHealth;
                saveData.playerMaxHealth = stats.MaxHealth;
                saveData.playerEnergy = stats.CurrentEnergy;
                saveData.playerMaxEnergy = stats.MaxEnergy;
            }
        }
        if (InventoryManager.Instance != null)
        {
            var keyCards = InventoryManager.Instance.GetCollectedKeyCards();
            saveData.collectedKeyCards = new List<string>(keyCards);
            var energyCores = InventoryManager.Instance.GetCollectedEnergyCores();
            saveData.collectedEnergyCores = new List<string>(energyCores);
            var notes = InventoryManager.Instance.GetAllNotes();
            saveData.collectedNotes = new List<NoteData>(notes.Values);
        }
        saveData.currentLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        var puzzleManager = FindObjectOfType<PuzzleManager>();
        if (puzzleManager != null)
        {
            saveData.solvedPuzzleIds.Clear();
            var allPuzzles = FindObjectsOfType<BasePuzzle>();
            foreach (var puzzle in allPuzzles)
            {
                if (puzzle != null && puzzle.IsSolved)
                {
                    saveData.solvedPuzzleIds.Add(puzzle.PuzzleId);
                }
            }
        }
        saveData.disabledHazardNames.Clear();
        var allEnergyDischarges = FindObjectsOfType<EnergyDischarge>(true);
        foreach (var discharge in allEnergyDischarges)
        {
            if (discharge != null && discharge.gameObject != null && !discharge.gameObject.activeSelf)
            {
                string path = GetGameObjectPath(discharge.gameObject);
                saveData.disabledHazardNames.Add(path);
            }
        }
        saveData.saveDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        if (currentSaveData != null)
        {
            saveData.playTime = currentSaveData.playTime + (Time.time - sessionStartTime);
        }
        else
        {
            saveData.playTime = Time.time - sessionStartTime;
        }
        return saveData;
    }
    private void ApplySaveData(SaveData saveData)
    {
        if (string.IsNullOrEmpty(saveDirectory))
        {
            InitializeSaveDirectory();
        }
        if (!string.IsNullOrEmpty(saveData.currentLevel))
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (currentScene != saveData.currentLevel)
            {
                var sceneLoader = FindObjectOfType<SceneLoader>();
                if (sceneLoader != null)
                {
                    // Синхронизируем LevelManager перед загрузкой сцены
                    if (LevelManager.Instance != null)
                    {
                        LevelManager.Instance.SetLevelIndexFromLevelName(saveData.currentLevel);
                    }
                    sceneLoader.LoadScene(saveData.currentLevel, () => StartCoroutine(ApplySaveDataAfterSceneLoadDelayed(saveData)));
                    return;
                }
                else
                {
                    Debug.LogError("SaveManager: SceneLoader not found! Cannot load scene.");
                }
            }
            else
            {
                // Синхронизируем LevelManager, если мы уже на правильной сцене
                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.SetLevelIndexFromLevelName(saveData.currentLevel);
                }
            }
        }
        StartCoroutine(ApplySaveDataAfterSceneLoadDelayed(saveData));
    }
    private System.Collections.IEnumerator ApplySaveDataAfterSceneLoadDelayed(SaveData saveData)
    {
        yield return null;
        yield return null;
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().isLoaded)
        {
            Debug.LogWarning("SaveManager: Scene is not fully loaded yet. Waiting...");
            yield return new WaitForSeconds(0.5f);
        }
        ApplySaveDataAfterSceneLoad(saveData);
    }
    private void ApplySaveDataAfterSceneLoad(SaveData saveData)
    {
        StartCoroutine(RestorePlayerAfterDelay(saveData));
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearInventory();
            foreach (string keyCardId in saveData.collectedKeyCards)
            {
                InventoryManager.Instance.AddKeyCard(keyCardId);
            }
            foreach (string coreId in saveData.collectedEnergyCores)
            {
                InventoryManager.Instance.AddEnergyCore(coreId);
            }
            foreach (NoteData noteData in saveData.collectedNotes)
            {
                if (noteData != null)
                {
                    InventoryManager.Instance.AddNote(
                        noteData.noteId,
                        noteData.noteTitle,
                        noteData.noteText
                    );
                }
            }
        }
        // Скрыть энергоядра, которые уже собраны
        StartCoroutine(HideCollectedEnergyCoresAfterDelay(saveData));
        StartCoroutine(RestorePuzzlesAfterDelay(saveData));
    }
    private System.Collections.IEnumerator HideCollectedEnergyCoresAfterDelay(SaveData saveData)
    {
        yield return null;
        yield return null;
        if (saveData.collectedEnergyCores != null && saveData.collectedEnergyCores.Count > 0)
        {
            var allEnergyCores = FindObjectsOfType<EnergyCore>(true);
            foreach (var energyCore in allEnergyCores)
            {
                if (energyCore != null && saveData.collectedEnergyCores.Contains(energyCore.CoreId))
                {
                    energyCore.gameObject.SetActive(false);
                    var col = energyCore.GetComponent<Collider>();
                    if (col != null)
                    {
                        col.enabled = false;
                    }
                }
            }
        }
    }
    private System.Collections.IEnumerator RestorePlayerAfterDelay(SaveData saveData)
    {
        isRestoringPlayer = true;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        PlayerController player = null;
        int attempts = 0;
        while (player == null && attempts < 10)
        {
            player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                yield return new WaitForSeconds(0.1f);
                attempts++;
            }
        }
        if (player != null)
        {
            var rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                bool wasKinematic = rb.isKinematic;
                bool wasSleeping = rb.IsSleeping();
                rb.isKinematic = true;
                player.InputEnabled = false;
                player.transform.position = saveData.playerPosition;
                player.transform.rotation = saveData.playerRotation;
                var playerControllerType = player.GetType();
                var currentRotationYField = playerControllerType.GetField("currentRotationY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (currentRotationYField != null)
                {
                    float savedRotationY = saveData.playerRotation.eulerAngles.y;
                    currentRotationYField.SetValue(player, savedRotationY);
                }
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.position = saveData.playerPosition;
                rb.rotation = saveData.playerRotation;
                rb.MovePosition(saveData.playerPosition);
                rb.MoveRotation(saveData.playerRotation);
                rb.isKinematic = wasKinematic;
                if (wasSleeping)
                {
                    rb.WakeUp();
                }
                yield return null;
                yield return null;
                Debug.Log($"SaveManager: Player position restored to {saveData.playerPosition}, rotation: {saveData.playerRotation.eulerAngles} (Rigidbody method)");
            }
            else
            {
                player.transform.position = saveData.playerPosition;
                player.transform.rotation = saveData.playerRotation;
                Debug.Log($"SaveManager: Player position restored to {saveData.playerPosition}, rotation: {saveData.playerRotation.eulerAngles} (Transform method)");
            }
            yield return null;
            if (rb != null && Vector3.Distance(player.transform.position, saveData.playerPosition) > 0.1f)
            {
                Debug.LogWarning($"SaveManager: Player position changed before gravity restore! Restoring position again...");
                bool wasKinematic = rb.isKinematic;
                rb.isKinematic = true;
                player.transform.position = saveData.playerPosition;
                player.transform.rotation = saveData.playerRotation;
                rb.MovePosition(saveData.playerPosition);
                rb.MoveRotation(saveData.playerRotation);
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = wasKinematic;
                yield return null;
            }
            var gravitySystem = player.GetComponent<GravitySystem>();
            if (gravitySystem != null)
            {
                if (gravitySystem.IsGravityFlipped != saveData.isGravityFlipped)
                {
                    gravitySystem.FlipGravity();
                }
            }
            yield return null;
            if (rb != null && Vector3.Distance(player.transform.position, saveData.playerPosition) > 0.1f)
            {
                Debug.LogWarning($"SaveManager: Player position changed after gravity restore! Restoring position again...");
                bool wasKinematic = rb.isKinematic;
                rb.isKinematic = true;
                player.transform.position = saveData.playerPosition;
                player.transform.rotation = saveData.playerRotation;
                rb.MovePosition(saveData.playerPosition);
                rb.MoveRotation(saveData.playerRotation);
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = wasKinematic;
                yield return null;
            }
            var stats = player.GetComponent<PlayerStatsComponent>();
            if (stats != null)
            {
                stats.CurrentHealth = saveData.playerHealth;
                stats.CurrentEnergy = saveData.playerEnergy;
            }
            if (rb == null)
            {
                var playerControllerType = player.GetType();
                var currentRotationYField = playerControllerType.GetField("currentRotationY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (currentRotationYField != null)
                {
                    float savedRotationY = saveData.playerRotation.eulerAngles.y;
                    currentRotationYField.SetValue(player, savedRotationY);
                }
                player.InputEnabled = false;
            }
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }
            float distance = Vector3.Distance(player.transform.position, saveData.playerPosition);
            float rotationDiff = Quaternion.Angle(player.transform.rotation, saveData.playerRotation);
            if (distance > 0.1f || rotationDiff > 5f)
            {
                Debug.LogWarning($"SaveManager: Player position/rotation changed after restore! Distance: {distance}, Rotation diff: {rotationDiff}°, Expected pos: {saveData.playerPosition}, Actual pos: {player.transform.position}. Restoring again...");
                if (rb != null)
                {
                    bool wasKinematic = rb.isKinematic;
                    rb.isKinematic = true;
                    player.transform.position = saveData.playerPosition;
                    player.transform.rotation = saveData.playerRotation;
                    rb.MovePosition(saveData.playerPosition);
                    rb.MoveRotation(saveData.playerRotation);
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = wasKinematic;
                    rb.WakeUp();
                }
                else
                {
                    player.transform.position = saveData.playerPosition;
                    player.transform.rotation = saveData.playerRotation;
                }
                for (int i = 0; i < 3; i++)
                {
                    yield return null;
                }
                distance = Vector3.Distance(player.transform.position, saveData.playerPosition);
                rotationDiff = Quaternion.Angle(player.transform.rotation, saveData.playerRotation);
                if (distance > 0.1f || rotationDiff > 5f)
                {
                    Debug.LogError($"SaveManager: Failed to restore player position/rotation! Final distance: {distance}, Rotation diff: {rotationDiff}°, Expected pos: {saveData.playerPosition}, Actual pos: {player.transform.position}, Expected rot: {saveData.playerRotation.eulerAngles}, Actual rot: {player.transform.rotation.eulerAngles}");
                    if (rb != null)
                    {
                        rb.isKinematic = true;
                        player.transform.position = saveData.playerPosition;
                        player.transform.rotation = saveData.playerRotation;
                        rb.position = saveData.playerPosition;
                        rb.rotation = saveData.playerRotation;
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                        var playerControllerType = player.GetType();
                        var currentRotationYField = playerControllerType.GetField("currentRotationY", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (currentRotationYField != null)
                        {
                            float savedRotationY = saveData.playerRotation.eulerAngles.y;
                            currentRotationYField.SetValue(player, savedRotationY);
                        }
                        yield return null;
                        yield return null;
                        rb.isKinematic = false;
                        yield return null;
                        distance = Vector3.Distance(player.transform.position, saveData.playerPosition);
                        rotationDiff = Quaternion.Angle(player.transform.rotation, saveData.playerRotation);
                        if (distance <= 0.1f && rotationDiff <= 5f)
                        {
                        }
                    }
                }
            }
            player.InputEnabled = true;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetState(GameManager.GameState.Playing);
            }
        }
        else
        {
            Debug.LogWarning("SaveManager: Player not found after multiple attempts. Position not restored.");
        }
        isRestoringPlayer = false;
    }
    public bool IsRestoringPlayer => isRestoringPlayer;
    private System.Collections.IEnumerator RestorePuzzlesAfterDelay(SaveData saveData)
    {
        yield return null;
        yield return null;
        yield return null;
        PuzzleManager puzzleManager = null;
        int attempts = 0;
        while (puzzleManager == null && attempts < 20)
        {
            puzzleManager = FindObjectOfType<PuzzleManager>();
            if (puzzleManager == null)
            {
                yield return new WaitForSeconds(0.1f);
                attempts++;
            }
        }
        if (puzzleManager == null)
        {
            Debug.LogWarning("SaveManager: PuzzleManager not found after multiple attempts. Puzzles may not be restored correctly.");
        }
        yield return null;
        yield return null;
        if (saveData.solvedPuzzleIds != null && saveData.solvedPuzzleIds.Count > 0)
        {
            var allPuzzles = FindObjectsOfType<BasePuzzle>();
            int restoredCount = 0;
            foreach (var puzzle in allPuzzles)
            {
                if (puzzle != null && saveData.solvedPuzzleIds.Contains(puzzle.PuzzleId))
                {
                    puzzle.RestorePuzzleState(BasePuzzle.PuzzleState.Solved);
                    restoredCount++;
                }
            }
            yield return null;
            if (puzzleManager != null)
            {
                puzzleManager.RecalculateSolvedCount();
            }
        }
        if (saveData.disabledHazardNames != null && saveData.disabledHazardNames.Count > 0)
        {
            yield return null;
            yield return null;
            yield return new WaitForSeconds(0.1f);
            var allEnergyDischarges = FindObjectsOfType<EnergyDischarge>(true);
            foreach (var discharge in allEnergyDischarges)
            {
                if (discharge != null && discharge.gameObject != null)
                {
                    string path = GetGameObjectPath(discharge.gameObject);
                    bool shouldDisable = false;
                    if (saveData.disabledHazardNames.Contains(path))
                    {
                        shouldDisable = true;
                    }
                    else if (saveData.disabledHazardNames.Contains(discharge.gameObject.name))
                    {
                        shouldDisable = true;
                    }
                    if (shouldDisable)
                    {
                        discharge.gameObject.SetActive(false);
                    }
                }
            }
            yield return null;
            yield return null;
            foreach (var savedPath in saveData.disabledHazardNames)
            {
                var obj = FindGameObjectByPath(savedPath);
                if (obj != null)
                {
                    if (obj.activeSelf)
                    {
                        Debug.LogWarning($"SaveManager: Hazard {savedPath} is still active! Disabling again...");
                        obj.SetActive(false);
                    }
                }
            }
        }
    }
    private string GetGameObjectPath(GameObject obj)
    {
        if (obj == null)
            return "";
        string path = obj.name;
        Transform current = obj.transform.parent;
        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }
        return path;
    }
    private GameObject FindGameObjectByPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;
        string[] parts = path.Split('/');
        if (parts.Length == 0)
            return null;
        GameObject root = GameObject.Find(parts[0]);
        if (root == null)
        {
            var allObjects = FindObjectsOfType<GameObject>(true);
            foreach (var obj in allObjects)
            {
                if (obj.name == parts[0] && GetGameObjectPath(obj) == path)
                {
                    return obj;
                }
            }
            return null;
        }
        Transform current = root.transform;
        for (int i = 1; i < parts.Length; i++)
        {
            current = current.Find(parts[i]);
            if (current == null)
            {
                foreach (Transform child in root.transform.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == parts[i] && GetGameObjectPath(child.gameObject) == path)
                    {
                        return child.gameObject;
                    }
                }
                return null;
            }
        }
        return current.gameObject;
    }
    private string GetSaveFilePath()
    {
        if (string.IsNullOrEmpty(saveDirectory))
        {
            InitializeSaveDirectory();
        }
        if (string.IsNullOrEmpty(saveFileName))
        {
            saveFileName = "savegame.json";
            Debug.LogWarning("SaveManager: saveFileName was empty, using default 'savegame.json'");
        }
        return Path.Combine(saveDirectory, saveFileName);
    }
    public bool SaveExists()
    {
        string filePath = GetSaveFilePath();
        return File.Exists(filePath);
    }
    public void DeleteSave()
    {
        string filePath = GetSaveFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
