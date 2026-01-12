using UnityEngine;
public class LevelManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string[] levelNames = { "Level01_StationHub", "Level02_ReactorCore" };
    private int currentLevelIndex = 0;
    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<LevelManager>();
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    private void Start()
    {
        EventBus.OnLevelCompleted += LoadNextLevel;
    }
    private void OnDestroy()
    {
        EventBus.OnLevelCompleted -= LoadNextLevel;
    }
    public void LoadNextLevel(string levelName = "")
    {
        if (currentLevelIndex < levelNames.Length - 1)
        {
            currentLevelIndex++;
            SceneLoader.Instance.LoadScene(levelNames[currentLevelIndex]);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }
    public void LoadLevel(string levelName)
    {
        for (int i = 0; i < levelNames.Length; i++)
        {
            if (levelNames[i] == levelName)
            {
                currentLevelIndex = i;
                SceneLoader.Instance.LoadScene(levelName);
                return;
            }
        }
        Debug.LogWarning($"Level {levelName} not found!");
    }
    public string CurrentLevelName => currentLevelIndex >= 0 && currentLevelIndex < levelNames.Length ? levelNames[currentLevelIndex] : "";
}
