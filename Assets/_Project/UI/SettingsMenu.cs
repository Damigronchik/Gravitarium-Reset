using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [Header("Buttons")]
    [SerializeField] private Button backButton;
    private AudioManager audioManager;
    private UIManager uiManager;
    private bool openedFromPauseMenu = false;
    private void Awake()
    {
        audioManager = AudioManager.Instance;
        uiManager = UIManager.Instance;
    }
    private void Start()
    {
        SetupUI();
        LoadSettings();
    }
    private void SetupUI()
    {
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.minValue = 0f;
            musicVolumeSlider.maxValue = 1f;
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.minValue = 0f;
            sfxVolumeSlider.maxValue = 1f;
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            string[] qualityNames = QualitySettings.names;
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(qualityNames));
            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }
    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = musicVolume;
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = sfxVolume;
        }
        if (qualityDropdown != null)
        {
            qualityDropdown.value = qualityLevel;
        }
        ApplySettings();
    }
    private void OnMusicVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetMusicVolume(value);
        }
        UpdateMusicVolumeText(value);
    }
    private void OnSFXVolumeChanged(float value)
    {
        if (audioManager != null)
        {
            audioManager.SetSFXVolume(value);
        }
        UpdateSFXVolumeText(value);
    }
    private void OnQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
        PlayerPrefs.Save();
    }
    private void UpdateMusicVolumeText(float value)
    {
        if (musicVolumeText != null)
        {
            musicVolumeText.text = $"Музыка: {Mathf.RoundToInt(value * 100)}%";
        }
    }
    private void UpdateSFXVolumeText(float value)
    {
        if (sfxVolumeText != null)
        {
            sfxVolumeText.text = $"Звуки: {Mathf.RoundToInt(value * 100)}%";
        }
    }
    private void ApplySettings()
    {
        if (audioManager != null)
        {
            if (musicVolumeSlider != null)
            {
                audioManager.SetMusicVolume(musicVolumeSlider.value);
            }
            if (sfxVolumeSlider != null)
            {
                audioManager.SetSFXVolume(sfxVolumeSlider.value);
            }
        }
        if (qualityDropdown != null)
        {
            QualitySettings.SetQualityLevel(qualityDropdown.value);
        }
    }
    public void SetOpenedFromPauseMenu(bool fromPause)
    {
        openedFromPauseMenu = fromPause;
    }
    private void OnBackClicked()
    {
        if (openedFromPauseMenu)
        {
            if (uiManager != null)
            {
                uiManager.ShowPauseMenu();
            }
            openedFromPauseMenu = false;
        }
        else
        {
            if (uiManager != null)
            {
                uiManager.ShowMainMenu();
            }
        }
    }
}
