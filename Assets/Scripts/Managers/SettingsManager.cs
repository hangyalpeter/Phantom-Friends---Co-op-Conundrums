using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private void OnEnable()
    {
        SettingsEvents.MusicVolumeChanged += SettingsEvents_OnMusicVolumeChanged;
        SettingsEvents.SoundEffectsVolumeChanged += SettingsEvents_OnSoundEffectsVolumeChanged;
        SettingsEvents.MasterVolumeChanged += SettingsEvents_OnMasterVolumeChanged;
        SettingsEvents.ResolutionChanged += SettingsEvents_OnResolutionChanged;
        SettingsEvents.FullScreenChanged += SettingsEvents_OnFullScreenChanged;
    }

    private void OnDisable()
    {
        SettingsEvents.MusicVolumeChanged -= SettingsEvents_OnMusicVolumeChanged;
        SettingsEvents.SoundEffectsVolumeChanged -= SettingsEvents_OnSoundEffectsVolumeChanged;
        SettingsEvents.MasterVolumeChanged -= SettingsEvents_OnMasterVolumeChanged;
        SettingsEvents.ResolutionChanged -= SettingsEvents_OnResolutionChanged;
        SettingsEvents.FullScreenChanged -= SettingsEvents_OnFullScreenChanged;
    }

    private void SettingsEvents_OnFullScreenChanged(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    private void SettingsEvents_OnResolutionChanged(int index)
    {
        Resolution resolution = SettingsModel.Instance.Resolutions[index];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ResolutionIndex", index);
    }

    private void SettingsEvents_OnMasterVolumeChanged(float volume)
    {
        SettingsModel.Instance.MasterAudioMixer.SetFloat("masterVolume", DecibelConverter.ConvertLinearToDecibel(volume));
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    private void SettingsEvents_OnSoundEffectsVolumeChanged(float volume)
    {
        SettingsModel.Instance.MasterAudioMixer.SetFloat("sfxVolume", DecibelConverter.ConvertLinearToDecibel(volume));
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    private void SettingsEvents_OnMusicVolumeChanged(float volume)
    {
        SettingsModel.Instance.MasterAudioMixer.SetFloat("musicVolume", DecibelConverter.ConvertLinearToDecibel(volume));
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
}
