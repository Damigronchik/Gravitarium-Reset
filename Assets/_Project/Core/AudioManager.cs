using UnityEngine;
using System.Collections.Generic;
public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private int sfxPoolSize = 10;
    [Header("Music")]
    [SerializeField] private AudioClip[] backgroundMusicTracks;
    [SerializeField] private float musicVolume = 0.7f;
    [Header("SFX")]
    [SerializeField] private float sfxVolume = 1f;
    [Header("Footstep Sounds")]
    [SerializeField] private AudioClip metalFootstep;
    [SerializeField] private AudioClip concreteFootstep;
    [Header("Gameplay Sounds")]
    [SerializeField] private AudioClip gravityFlipSound;
    [SerializeField] private AudioClip terminalActivationSound;
    [SerializeField] private AudioClip itemCollectSound;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorLockedSound;
    [SerializeField] private AudioClip puzzleSolvedSound;
    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();
    private List<AudioSource> activeSFX = new List<AudioSource>();
    private int currentMusicTrackIndex = 0;
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
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
            InitializeAudioSources();
            LoadAudioSettings();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        SubscribeToEvents();
        PlayBackgroundMusic();
    }
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }
    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject sfxGO = new GameObject($"SFXSource_{i}");
            sfxGO.transform.SetParent(transform);
            AudioSource source = sfxGO.AddComponent<AudioSource>();
            source.volume = sfxVolume;
            source.spatialBlend = 1f;
            sfxPool.Enqueue(source);
        }
    }
    private void SubscribeToEvents()
    {
        EventBus.OnGravityFlipped += OnGravityFlipped;
        EventBus.OnTerminalActivated += OnTerminalActivated;
        EventBus.OnItemCollected += OnItemCollected;
        EventBus.OnPuzzleSolved += OnPuzzleSolved;
    }
    private void UnsubscribeFromEvents()
    {
        EventBus.OnGravityFlipped -= OnGravityFlipped;
        EventBus.OnTerminalActivated -= OnTerminalActivated;
        EventBus.OnItemCollected -= OnItemCollected;
        EventBus.OnPuzzleSolved -= OnPuzzleSolved;
    }
    private void OnGravityFlipped(Vector3 newGravity)
    {
        PlaySFX(gravityFlipSound, Vector3.zero);
    }
    private void OnTerminalActivated(GameObject terminal)
    {
        PlaySFX(terminalActivationSound, terminal.transform.position);
    }
    private void OnItemCollected(GameObject item)
    {
        PlaySFX(itemCollectSound, item.transform.position);
    }
    private void OnPuzzleSolved(GameObject puzzle)
    {
        PlaySFX(puzzleSolvedSound, puzzle.transform.position);
    }
    public void PlayBackgroundMusic(int trackIndex = -1)
    {
        if (backgroundMusicTracks == null || backgroundMusicTracks.Length == 0)
            return;
        if (trackIndex < 0)
        {
            trackIndex = currentMusicTrackIndex;
        }
        else
        {
            currentMusicTrackIndex = trackIndex;
        }
        if (trackIndex >= 0 && trackIndex < backgroundMusicTracks.Length && musicSource != null)
        {
            musicSource.clip = backgroundMusicTracks[trackIndex];
            musicSource.Play();
        }
    }
    public void PlaySFX(AudioClip clip, Vector3 position)
    {
        if (clip == null)
            return;
        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.transform.position = position;
            source.clip = clip;
            source.Play();
            StartCoroutine(ReturnSFXSourceToPool(source, clip.length));
        }
    }
    public void PlaySFX2D(AudioClip clip)
    {
        if (clip == null)
            return;
        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.spatialBlend = 0f;
            source.clip = clip;
            source.Play();
            StartCoroutine(ReturnSFXSourceToPool(source, clip.length));
        }
    }
    private AudioSource GetAvailableSFXSource()
    {
        if (sfxPool.Count > 0)
        {
            AudioSource source = sfxPool.Dequeue();
            activeSFX.Add(source);
            return source;
        }
        GameObject sfxGO = new GameObject("SFXSource_Temp");
        sfxGO.transform.SetParent(transform);
        AudioSource newSource = sfxGO.AddComponent<AudioSource>();
        newSource.volume = sfxVolume;
        activeSFX.Add(newSource);
        return newSource;
    }
    private System.Collections.IEnumerator ReturnSFXSourceToPool(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (source != null && activeSFX.Contains(source))
        {
            activeSFX.Remove(source);
            source.Stop();
            source.clip = null;
            source.spatialBlend = 1f;
            sfxPool.Enqueue(source);
        }
    }
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (var source in activeSFX)
        {
            if (source != null)
            {
                source.volume = sfxVolume;
            }
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    private void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
        }
    }
    public AudioClip GetFootstepClip(string surfaceType)
    {
        return surfaceType switch
        {
            "Metal" => metalFootstep,
            "Concrete" => concreteFootstep,
            _ => concreteFootstep
        };
    }
    public void PlayFootstepSFX(AudioClip clip, float pitchVariation = 0.2f)
    {
        if (clip == null)
            return;
        AudioSource source = GetAvailableSFXSource();
        if (source != null)
        {
            source.spatialBlend = 0f;
            source.clip = clip;
            float originalPitch = source.pitch;
            float basePitch = 1.0f;
            float randomPitch = Random.Range(basePitch - pitchVariation, basePitch + pitchVariation);
            source.pitch = Mathf.Clamp(randomPitch, 0.5f, 2.0f);
            source.Play();
            StartCoroutine(ReturnSFXSourceToPoolWithPitchReset(source, clip.length, originalPitch));
        }
    }
    private System.Collections.IEnumerator ReturnSFXSourceToPoolWithPitchReset(AudioSource source, float delay, float originalPitch)
    {
        yield return new WaitForSeconds(delay);
        if (source != null)
        {
            source.pitch = originalPitch;
            StartCoroutine(ReturnSFXSourceToPool(source, 0f));
        }
    }
    public AudioClip GetItemCollectSound()
    {
        return itemCollectSound;
    }
    public AudioClip GetDoorOpenSound()
    {
        return doorOpenSound;
    }
    public AudioClip GetDoorLockedSound()
    {
        return doorLockedSound;
    }
    public AudioClip GetPuzzleSolvedSound()
    {
        return puzzleSolvedSound;
    }
    public AudioClip GetGravityFlipSound()
    {
        return gravityFlipSound;
    }
    public AudioClip GetTerminalActivationSound()
    {
        return terminalActivationSound;
    }
}
