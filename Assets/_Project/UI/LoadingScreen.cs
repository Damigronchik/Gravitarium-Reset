using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LoadingScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    private SceneLoader sceneLoader;
    private void Awake()
    {
        sceneLoader = SceneLoader.Instance;
    }
    private void Update()
    {
        UpdateProgress();
    }
    private void UpdateProgress()
    {
        if (sceneLoader == null)
            return;
        float progress = sceneLoader.GetLoadingProgress();
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
        if (progressText != null)
        {
            progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
        }
    }
}
