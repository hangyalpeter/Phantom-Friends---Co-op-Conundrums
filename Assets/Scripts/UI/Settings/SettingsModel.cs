using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsModel : MonoBehaviour
{

    public static SettingsModel Instance { get; private set; }

    const float k_DefaultMasterVolume = 1f;
    const float k_DefaultSFXVolume = 1f;
    const float k_DefaultMusicVolume = 0f;

    [SerializeField] private AudioMixer m_MasterAudioMixer;

    [Header("Volume Settings")]
    [Tooltip("The master volume level (0 to 1)")]
    [SerializeField] private float m_MasterVolume = 0;

    [Tooltip("The sound effects volume level (0 to 1)")]
    [SerializeField] private float m_SoundEffectsVolume = 0;

    [Tooltip("The music volume level (0 to 1)")]
    [SerializeField] private float m_MusicVolume = 0;

    private Resolution[] resolutions;
    private int currentResolutionIndex;
    private List<string> options;
    public AudioMixer MasterAudioMixer { get => m_MasterAudioMixer; }

    public float MasterVolume { get => m_MasterVolume; set => m_MasterVolume = value; }
    public float SoundEffectsVolume { get => m_SoundEffectsVolume; set => m_SoundEffectsVolume = value; }
    public float MusicVolume { get => m_MusicVolume; set => m_MusicVolume = value; }

    const bool k_IsFullScreen = true;

    private bool m_IsFullScreen = k_IsFullScreen;
    public bool IsFullScreen { get => m_IsFullScreen; set => m_IsFullScreen = value; }

    public Resolution[] Resolutions => resolutions;
    public int CurrentResolutionIndex => currentResolutionIndex;
    public List<string> ResultionOptions => options;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        InitializeAudioSettings();
        InitializeResolutionSettings();

    }

    private void InitializeAudioSettings()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            m_MasterVolume = PlayerPrefs.GetFloat("masterVolume");
        }
        else
        {
            m_MasterVolume = k_DefaultMasterVolume;
            PlayerPrefs.SetFloat("masterVolume", m_MasterVolume);
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            m_SoundEffectsVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            m_SoundEffectsVolume = k_DefaultSFXVolume;
            PlayerPrefs.SetFloat("sfxVolume", m_SoundEffectsVolume);
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            m_MusicVolume = PlayerPrefs.GetFloat("musicVolume");
        }
        else
        {
            m_MusicVolume = k_DefaultMusicVolume;
            PlayerPrefs.SetFloat("musicVolume", m_MusicVolume);
        }
    }

    private void InitializeResolutionSettings()
    {
        resolutions = Screen.resolutions;
        options = new List<string>();
        currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

        }

        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        }
        else
        {
            PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
        }

    }

    private void OnEnable()
    {
        SettingsEvents.MasterVolumeChanged += SettingsEvents_MasterVolumeChanged;
        SettingsEvents.MusicVolumeChanged += SettingsEvents_MusicVolumeChanged;
        SettingsEvents.SoundEffectsVolumeChanged += SettingsEvents_SoundEffectsVolumeChanged;
        SettingsEvents.ResolutionChanged += SettingsEvents_ResolutionChanged;
    }

    private void OnDisable()
    {
        SettingsEvents.MasterVolumeChanged -= SettingsEvents_MasterVolumeChanged;
        SettingsEvents.MusicVolumeChanged -= SettingsEvents_MusicVolumeChanged;
        SettingsEvents.SoundEffectsVolumeChanged -= SettingsEvents_SoundEffectsVolumeChanged;
        SettingsEvents.ResolutionChanged -= SettingsEvents_ResolutionChanged;
    }

    private void SettingsEvents_ResolutionChanged(int index)
    {
        currentResolutionIndex = index;
    }

    private void SettingsEvents_MasterVolumeChanged(float volume)
    {
        m_MasterVolume = volume;
    }

    private void SettingsEvents_MusicVolumeChanged(float volume)
    {
        m_MusicVolume = volume;
    }

    private void SettingsEvents_SoundEffectsVolumeChanged(float volume)
    {
        m_SoundEffectsVolume = volume;
    }

}
