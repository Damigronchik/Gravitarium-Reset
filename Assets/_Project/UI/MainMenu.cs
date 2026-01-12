using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    private GameManager gameManager;
    private UIManager uiManager;
    private void Awake()
    {
        gameManager = GameManager.Instance;
        uiManager = UIManager.Instance;
    }
	private void OnEnable ()
    {
        SetupButtons();
        UpdateLoadButtonState();
    }
    private void SetupButtons()
    {
        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(OnStartGameClicked);
        }
        if (loadGameButton != null)
        {
            loadGameButton.onClick.RemoveAllListeners();
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
        }
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveAllListeners();
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(OnQuitClicked);
        }
    }
    public void UpdateLoadButtonState()
    {
        if (loadGameButton != null && SaveManager.Instance != null)
        {
            bool saveExists = SaveManager.Instance.SaveExists();
            loadGameButton.interactable = saveExists;
            Debug.Log($"MainMenu: Load button state updated - Save exists: {saveExists}");
        }
    }
    private void OnStartGameClicked()
    {
        if (gameManager != null)
        {
            gameManager.StartNewGame();
        }
    }
    private void OnLoadGameClicked()
    {
        if (loadGameButton != null)
        {
            loadGameButton.interactable = false;
        }
        if (SaveManager.Instance != null)
        {
            bool loadStarted = SaveManager.Instance.LoadGame();
            if (!loadStarted)
            {
                UpdateLoadButtonState();
            }
        }
        else
        {
            Debug.LogError("MainMenu: SaveManager.Instance is null! Cannot load game.");
            UpdateLoadButtonState();
        }
    }
    private void OnSettingsClicked()
    {
        if (uiManager != null)
        {
            uiManager.ShowSettings();
        }
    }
    private void OnQuitClicked()
    {
        if (gameManager != null)
        {
            gameManager.QuitGame();
        }
    }
}
