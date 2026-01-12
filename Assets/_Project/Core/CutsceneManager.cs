using UnityEngine;
using UnityEngine.Playables;
public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscenes")]
    [SerializeField] private PlayableDirector[] cutscenes;
    [Header("Settings")]
    [SerializeField] private bool pauseGameDuringCutscene = true;
    private PlayableDirector currentCutscene = null;
    private bool isPlayingCutscene = false;
    private static CutsceneManager instance;
    public static CutsceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CutsceneManager>();
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
    private void Start()
    {
        foreach (var cutscene in cutscenes)
        {
            if (cutscene != null)
            {
                cutscene.stopped += OnCutsceneStopped;
            }
        }
    }
    public void PlayCutscene(int index)
    {
        if (index < 0 || index >= cutscenes.Length)
        {
            Debug.LogWarning($"Cutscene index {index} is out of range!");
            return;
        }
        PlayCutscene(cutscenes[index]);
    }
    public void PlayCutscene(PlayableDirector cutscene)
    {
        if (cutscene == null)
        {
            Debug.LogWarning("Cutscene is null!");
            return;
        }
        if (isPlayingCutscene)
        {
            Debug.LogWarning("Another cutscene is already playing!");
            return;
        }
        currentCutscene = cutscene;
        isPlayingCutscene = true;
        if (pauseGameDuringCutscene)
        {
            Time.timeScale = 0f;
        }
        EventBus.InvokeGamePaused();
        cutscene.Play();
    }
    private void OnCutsceneStopped(PlayableDirector director)
    {
        if (director == currentCutscene)
        {
            isPlayingCutscene = false;
            currentCutscene = null;
            if (pauseGameDuringCutscene)
            {
                Time.timeScale = 1f;
            }
            EventBus.InvokeGameResumed();
        }
    }
    public void SkipCutscene()
    {
        if (currentCutscene != null && isPlayingCutscene)
        {
            currentCutscene.Stop();
        }
    }
    public bool IsPlayingCutscene => isPlayingCutscene;
}
