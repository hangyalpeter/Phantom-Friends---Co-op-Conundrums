using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    private void OnEnable()
    {
        SettingsEvents.MusicVolumeChanged += SettingsEvents_OnMusicVolumeChanged;
        SettingsEvents.SoundEffectsVolumeChanged += SettingsEvents_OnSoundEffectsVolumeChanged;
        SettingsEvents.MasterVolumeChanged += SettingsEvents_OnMasterVolumeChanged;
        SettingsEvents.ResolutionChanged += SettingsEvents_OnResolutionChanged;
    }

    private void OnDisable()
    {
        SettingsEvents.MusicVolumeChanged -= SettingsEvents_OnMusicVolumeChanged;
        SettingsEvents.SoundEffectsVolumeChanged -= SettingsEvents_OnSoundEffectsVolumeChanged;
        SettingsEvents.MasterVolumeChanged -= SettingsEvents_OnMasterVolumeChanged;
        SettingsEvents.ResolutionChanged -= SettingsEvents_OnResolutionChanged;
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
    }

    private void SettingsEvents_OnSoundEffectsVolumeChanged(float volume)
    {
        SettingsModel.Instance.MasterAudioMixer.SetFloat("sfxVolume", DecibelConverter.ConvertLinearToDecibel(volume));
    }

    private void SettingsEvents_OnMusicVolumeChanged(float volume)
    {
        SettingsModel.Instance.MasterAudioMixer.SetFloat("musicVolume", DecibelConverter.ConvertLinearToDecibel(volume));
        Debug.Log("Music volume changed to " + volume);
    }
}
