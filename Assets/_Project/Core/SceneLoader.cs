using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class SceneLoader : MonoBehaviour
{
    private AsyncOperation currentLoadingOperation;
    private static SceneLoader instance;
    public static SceneLoader Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SceneLoader>();
                if (instance == null)
                {
                    GameObject go = new GameObject("SceneLoader");
                    instance = go.AddComponent<SceneLoader>();
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
    public void LoadScene(string sceneName)
    {
        LoadScene(sceneName, null);
    }
    public void LoadScene(string sceneName, System.Action onComplete)
    {
        StartCoroutine(LoadSceneAsync(sceneName, onComplete));
    }
    private IEnumerator LoadSceneAsync(string sceneName, System.Action onComplete = null)
    {
        EventBus.InvokeLoadingStarted(sceneName);
        EventBus.InvokeLevelStarted(sceneName);
        currentLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
        currentLoadingOperation.allowSceneActivation = false;
        while (currentLoadingOperation.progress < 0.9f)
            yield return null;
        currentLoadingOperation.allowSceneActivation = true;
        while (!currentLoadingOperation.isDone)
            yield return null;
        currentLoadingOperation = null;
        EventBus.InvokeLevelLoaded(sceneName);
        if (onComplete != null)
        {
            yield return null;
            onComplete.Invoke();
        }
    }
    public float GetLoadingProgress()
    {
        return currentLoadingOperation != null ? Mathf.Clamp01(currentLoadingOperation.progress / 0.9f) : 0f;
    }
    public bool IsLoading => currentLoadingOperation != null;
}
