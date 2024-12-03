using System;

public static class SettingsEvents
{
    // Presenter -> View
    public static Action<string[]> ResolutionDropdownOptionsSet;
    public static Action<int> ResolutionDropdownIndexSet;

    public static Action<float> MasterVolumeSliderSet;
    public static Action<float> MusicVolumeSliderSet;
    public static Action<float> SoundEffectsVolumeSliderSet;
    public static Action<bool> FullScreenToggleSet;


    //Presenter -> Model
    public static Action<float> MasterVolumeChanged;
    public static Action<float> MusicVolumeChanged;
    public static Action<float> SoundEffectsVolumeChanged;

    public static Action<int> ResolutionChanged;
    public static Action<bool> FullScreenChanged;

    //View -> Presenter
    public static Action<float> MasterVolumeSliderChanged;
    public static Action<float> MusicVolumeSliderChanged;
    public static Action<float> SoundEffectsVolumeSliderChanged;

    public static Action<int> ResolutionDropdownChanged;
    public static Action<bool> FullScreenToggleChanged;

}
