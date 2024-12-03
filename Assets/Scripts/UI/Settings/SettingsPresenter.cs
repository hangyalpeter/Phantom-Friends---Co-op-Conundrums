using UnityEngine;

public class SettingsPresenter : MonoBehaviour
{
    private void Start()
    {
        Initialize();
    }
    private void OnEnable()
    {
        SettingsEvents.ResolutionDropdownChanged += SettingsEvents_ResolutionDropdownChanged;
        SettingsEvents.MasterVolumeSliderChanged += SettingsEvents_MasterVolumeSliderChanged;
        SettingsEvents.MusicVolumeSliderChanged += SettingsEvents_MusicVolumeSliderChanged;
        SettingsEvents.SoundEffectsVolumeSliderChanged += SettingsEvents_SoundEffectsVolumeSliderChanged;
        SettingsEvents.FullScreenToggleChanged += SettingsEvents_FullScreenToggleChanged;
    }
    private void OnDisable()
    {
        SettingsEvents.ResolutionDropdownChanged -= SettingsEvents_ResolutionDropdownChanged;
        SettingsEvents.MasterVolumeSliderChanged -= SettingsEvents_MasterVolumeSliderChanged;
        SettingsEvents.MusicVolumeSliderChanged -= SettingsEvents_MusicVolumeSliderChanged;
        SettingsEvents.SoundEffectsVolumeSliderChanged -= SettingsEvents_SoundEffectsVolumeSliderChanged;
        SettingsEvents.FullScreenToggleChanged -= SettingsEvents_FullScreenToggleChanged;
    }
    private void Initialize()
    {
        InitializeResolution();
        InitializeAudio();
        InitializeFullScreen();

    }

    private void InitializeResolution()
    {
        SettingsEvents.ResolutionDropdownOptionsSet?.Invoke(SettingsModel.Instance.ResultionOptions.ToArray());
        SettingsEvents.ResolutionDropdownIndexSet?.Invoke(SettingsModel.Instance.CurrentResolutionIndex);
    }

    private void InitializeAudio()
    {
        float masterVolume = SettingsModel.Instance.MasterVolume * 100f;
        float musicVolume = SettingsModel.Instance.MusicVolume * 100f;
        float sfxVolume = SettingsModel.Instance.SoundEffectsVolume * 100f;

        SettingsEvents.MasterVolumeSliderSet?.Invoke(masterVolume);
        SettingsEvents.MusicVolumeSliderSet?.Invoke(musicVolume);
        SettingsEvents.SoundEffectsVolumeSliderSet?.Invoke(sfxVolume);
    }

    private void InitializeFullScreen()
    {
        SettingsEvents.FullScreenToggleSet?.Invoke(SettingsModel.Instance.IsFullScreen);
    }

    private void SettingsEvents_ResolutionDropdownChanged(int index)
    {
        SettingsEvents.ResolutionChanged?.Invoke(index);
    }

    private void SettingsEvents_MusicVolumeSliderChanged(float value)
    {
        float volume = value / 100f;
        SettingsEvents.MusicVolumeChanged?.Invoke(volume);
    }

    private void SettingsEvents_SoundEffectsVolumeSliderChanged(float value)
    {
        float volume = value / 100f;
        SettingsEvents.SoundEffectsVolumeChanged?.Invoke(volume);
    }

    private void SettingsEvents_MasterVolumeSliderChanged(float value)
    {
        float volume = value / 100f;
        SettingsEvents.MasterVolumeChanged?.Invoke(volume);
    }

    private void SettingsEvents_FullScreenToggleChanged(bool newValue)
    {
        SettingsEvents.FullScreenChanged?.Invoke(newValue);
    }
}
