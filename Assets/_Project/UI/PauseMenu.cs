using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuPanel;
    private GameManager gameManager;
    private InputSystem inputActions;
    private bool isPaused = false;
    private void Awake()
    {
        gameManager = GameManager.Instance;
        inputActions = new InputSystem();
    }
    private void OnEnable()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        if (inputActions != null)
        {
            inputActions.UI.Enable();
        }
    }
    private void Start()
    {
        SetupButtons();
        if (inputActions != null)
        {
            inputActions.UI.Cancel.performed += OnCancelPerformed;
        }
    }
    private void OnDisable()
    {
        inputActions.UI.Disable();
    }
    private void SetupButtons()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
        }
        if (saveGameButton != null)
        {
            saveGameButton.onClick.AddListener(OnSaveGameClicked);
        }
        if (loadGameButton != null)
        {
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
        }
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClicked);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        }
    }
    private void OnCancelPerformed(InputAction.CallbackContext context)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu")
        {
            return;
        }
        TogglePause();
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (gameManager != null)
            {
                gameManager.PauseGame();
            }
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (gameManager != null)
            {
                gameManager.ResumeGame();
            }
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }
    }
    private void OnResumeClicked()
    {
        TogglePause();
    }
    private void OnSaveGameClicked()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
            Debug.Log("Игра сохранена из меню паузы");
        }
        else
        {
            Debug.LogError("SaveManager.Instance is null! Cannot save game.");
        }
    }
    private void OnLoadGameClicked()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.LoadGame();
        }
        TogglePause();
    }
    private void OnSettingsClicked()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowSettings(true);
        }
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }
    private void OnMainMenuClicked()
    {
        TogglePause();
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene("MainMenu", () =>
            {
                if (gameManager != null)
                {
                    gameManager.SetState(GameManager.GameState.Menu);
                }
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowMainMenu();
                }
            });
        }
        else
        {
            if (gameManager != null)
            {
                gameManager.SetState(GameManager.GameState.Menu);
            }
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowMainMenu();
            }
        }
    }
    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}
