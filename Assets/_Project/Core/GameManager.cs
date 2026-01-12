using UnityEngine;
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused
    }
    private GameState currentState = GameState.Menu;
    private static GameManager instance;
    private bool isStartingNewGame = false;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }
    public GameState CurrentState => currentState;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != null)
		{
            DontDestroyOnLoad(gameObject);
		}
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SetState(GameState.Menu);
        SubscribeToEvents();
    }
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    private void SubscribeToEvents()
    {
        EventBus.OnPlayerDeath += OnPlayerDeath;
    }
    private void UnsubscribeFromEvents()
    {
        EventBus.OnPlayerDeath -= OnPlayerDeath;
    }
    private void OnPlayerDeath()
    {
        var player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.InputEnabled = false;
        }
        HandlePlayerDeath();
    }
    private void HandlePlayerDeath()
    {
        Time.timeScale = 0f;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLoadingScreen();
            UIManager.Instance.ShowBG();
            UIManager.Instance.HideGameHUD();
        }
        else
        {
            Debug.LogWarning("GameManager: UIManager not found!");
        }
        if (SaveManager.Instance != null)
        {
            if (SaveManager.Instance.SaveExists())
            {
                StartCoroutine(LoadSaveAfterDelay());
            }
            else
            {
                Debug.LogWarning("GameManager: No save file found! Loading first level after death.");
                StartCoroutine(LoadFirstLevelAfterDelay());
            }
        }
        else
        {
            Debug.LogError("GameManager: SaveManager not found! Loading first level after death.");
            StartCoroutine(LoadFirstLevelAfterDelay());
        }
    }
    private System.Collections.IEnumerator LoadSaveAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
        yield return null;
        bool loadSuccess = SaveManager.Instance.LoadGame();
        if (!loadSuccess)
        {
            Debug.LogError("GameManager: Failed to load save after death! Loading first level.");
            yield return StartCoroutine(LoadFirstLevelAfterDelay());
        }
    }
    private System.Collections.IEnumerator LoadFirstLevelAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
        yield return null;
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("Level01_StationHub");
        }
        else
        {
            Debug.LogError("GameManager: SceneLoader not found! Cannot load level after death.");
        }
    }
    public void SetState(GameState newState)
    {
        if (currentState == newState) return;
        currentState = newState;
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                EventBus.InvokeGameResumed();
                break;
            case GameState.Paused:
                Time.timeScale = 0f;
                EventBus.InvokeGamePaused();
                break;
            case GameState.Menu:
                Time.timeScale = 1f;
                break;
        }
    }
    public void PauseGame()
    {
        if (currentState == GameState.Playing)
            SetState(GameState.Paused);
    }
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
            SetState(GameState.Playing);
    }
    public void StartNewGame()
    {
        isStartingNewGame = true;
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearInventory();
        }
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSave();
        }
        if (SceneLoader.Instance != null)
        {
            EventBus.OnLevelLoaded += OnNewGameLevelLoaded;
            SceneLoader.Instance.LoadScene("Level01_StationHub");
        }
        else
        {
            Debug.LogError("GameManager: SceneLoader not found! Cannot start new game.");
            isStartingNewGame = false;
        }
    }
    private void OnNewGameLevelLoaded(string levelName)
    {
        if (!isStartingNewGame)
        {
            return;
        }
        EventBus.OnLevelLoaded -= OnNewGameLevelLoaded;
        StartCoroutine(ResetGameStateAfterSceneLoad());
    }
    private System.Collections.IEnumerator ResetGameStateAfterSceneLoad()
    {
        yield return null;
        yield return null;
        yield return null;
        var puzzleManager = FindObjectOfType<PuzzleManager>();
        if (puzzleManager != null)
        {
            puzzleManager.ResetAllPuzzles();
        }
        else
        {
            var allPuzzles = FindObjectsOfType<BasePuzzle>();
            foreach (var puzzle in allPuzzles)
            {
                if (puzzle != null)
                {
                    puzzle.ResetPuzzle();
                }
            }
        }
        var hazardManager = FindObjectOfType<HazardManager>();
        if (hazardManager != null)
        {
            hazardManager.EnableAllEnergyDischarges();
        }
        isStartingNewGame = false;
    }
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
