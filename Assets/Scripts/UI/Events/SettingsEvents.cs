using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsEvents
{
    // Presenter -> View
    public static Action<string[]> ResolutionDropdownOptionsSet;
    public static Action<int> ResolutionDropdownIndexSet;

    public static Action<float> MasterVolumeSliderSet;
    public static Action<float> MusicVolumeSliderSet;
    public static Action<float> SoundEffectsVolumeSliderSet;


    //Presenter -> Model
    public static Action<float> MasterVolumeChanged;
    public static Action<float> MusicVolumeChanged;
    public static Action<float> SoundEffectsVolumeChanged;

    public static Action<int> ResolutionChanged;

    //View -> Presenter
    public static Action<float> MasterVolumeSliderChanged;
    public static Action<float> MusicVolumeSliderChanged;
    public static Action<float> SoundEffectsVolumeSliderChanged;

    public static Action<int> ResolutionDropdownChanged;

    //View -> Model
    public static Action<bool> FullScreenChanged;
}
