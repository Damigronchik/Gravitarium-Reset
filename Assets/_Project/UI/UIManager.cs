using UnityEngine;
public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject loadingScreenPanel;
    [SerializeField] private GameObject gameHUDPanel;
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private GameObject bgPanel;
    [Header("UI Components")]
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private SettingsMenu settingsMenu;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private GameHUD gameHUD;
    [SerializeField] private JournalUI journalUI;
    private GameObject currentActivePanel;
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }
    private void Awake()
    {
		DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SubscribeToEvents();
        ShowMainMenu();
    }
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    private void SubscribeToEvents()
    {
        EventBus.OnLevelStarted += OnLevelStarted;
        EventBus.OnLevelLoaded += OnLevelLoaded;
        EventBus.OnLoadingStarted += OnLoadingStarted;
        EventBus.OnGamePaused += OnGamePaused;
        EventBus.OnGameResumed += OnGameResumed;
    }
    private void UnsubscribeFromEvents()
    {
        EventBus.OnLevelStarted -= OnLevelStarted;
        EventBus.OnLevelLoaded -= OnLevelLoaded;
        EventBus.OnLoadingStarted -= OnLoadingStarted;
        EventBus.OnGamePaused -= OnGamePaused;
        EventBus.OnGameResumed -= OnGameResumed;
    }
    private void OnLoadingStarted(string levelName)
	{
		ShowBG();
		HideGameHUD();
	}
    private void OnLevelStarted(string levelName)
    {
		HideAllPanels();
        ShowLoadingScreen();
    }
    private void OnLevelLoaded(string levelName)
    {
        HideAllPanels();
        HideBG();
		ShowGameHUD();
    }
    private void OnGamePaused()
    {
        ShowPauseMenu();
    }
    private void OnGameResumed()
    {
        HidePauseMenu();
    }
    public void ShowMainMenu()
    {
        HideAllPanels();
        ShowBG();
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
            currentActivePanel = mainMenuPanel;
        }
        if (mainMenu != null)
        {
            mainMenu.UpdateLoadButtonState();
        }
    }
    public void ShowSettings()
    {
        ShowSettings(false);
    }
    public void ShowSettings(bool fromPauseMenu)
    {
        HideAllPanels();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            currentActivePanel = settingsPanel;
            if (settingsMenu != null)
            {
                settingsMenu.SetOpenedFromPauseMenu(fromPauseMenu);
            }
        }
    }
    public void ShowPauseMenu()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
    }
    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    public void ShowLoadingScreen()
    {
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
        }
    }
    public void HideLoadingScreen()
    {
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(false);
        }
    }
    public void ShowGameHUD()
    {
        if (gameHUDPanel != null)
        {
            gameHUDPanel.SetActive(true);
        }
    }
    public void HideGameHUD()
    {
        if (gameHUDPanel != null)
        {
            gameHUDPanel.SetActive(false);
        }
    }
    public void ToggleJournal()
    {
        if (journalPanel != null)
        {
            bool isActive = journalPanel.activeSelf;
            journalPanel.SetActive(!isActive);
        }
    }
    public void HideBG()
    {
        if (bgPanel != null)
        {
            bgPanel.SetActive(false);
        }
    }
    public void ShowBG()
    {
        if (bgPanel != null)
        {
            bgPanel.SetActive(true);
        }
    }
    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (loadingScreenPanel != null) loadingScreenPanel.SetActive(false);
        if (gameHUDPanel != null) gameHUDPanel.SetActive(false);
        if (journalPanel != null) journalPanel.SetActive(false);
    }
}
