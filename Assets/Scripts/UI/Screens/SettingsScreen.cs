using UnityEngine.UIElements;

public class SettingsScreen : UIScreen
{
    Button m_BackButton;

    private Slider m_MasterVolumeSlider;
    private Slider m_MusicVolumeSlider;
    private Slider m_SFXVolumeSlider;

    private DropdownField m_ResolutionDropdown;
    private Toggle m_fullScreenToggle;

    public SettingsScreen(VisualElement parentElement) : base(parentElement)
    {
        SetVisualElements();
        SubscribeToEvents();
        RegisterCallbacks();
    }

    public override void Disable()
    {
        base.Disable();
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        SettingsEvents.ResolutionDropdownOptionsSet += ResolutionDropdownOptionsSetHandler;
        SettingsEvents.ResolutionDropdownIndexSet += ResolutionDropdownIndexSetHandler;

        SettingsEvents.MasterVolumeSliderSet += MasterVolumeSliderSetHandler;
        SettingsEvents.MusicVolumeSliderSet += MusicVolumeSliderSetHandler;
        SettingsEvents.SoundEffectsVolumeSliderSet += SoundEffectsVolumeSliderSetHandler;
        SettingsEvents.FullScreenToggleSet += FullScreenToggleSetHandler;
    }
    private void UnsubscribeFromEvents()
    {
        SettingsEvents.ResolutionDropdownOptionsSet -= ResolutionDropdownOptionsSetHandler;
        SettingsEvents.ResolutionDropdownIndexSet -= ResolutionDropdownIndexSetHandler;
        SettingsEvents.MasterVolumeSliderSet -= MasterVolumeSliderSetHandler;
        SettingsEvents.MusicVolumeSliderSet -= MusicVolumeSliderSetHandler;
        SettingsEvents.SoundEffectsVolumeSliderSet -= SoundEffectsVolumeSliderSetHandler;
        SettingsEvents.FullScreenToggleSet -= FullScreenToggleSetHandler;
    }

    private void FullScreenToggleSetHandler(bool newValue)
    {
        m_fullScreenToggle.value = newValue;
    }

    private void SetVisualElements()
    {
        m_BackButton = m_RootElement.Q<Button>("back-button");
        m_MasterVolumeSlider = m_RootElement.Q<Slider>("master-volume-slider");
        m_MusicVolumeSlider = m_RootElement.Q<Slider>("music-volume-slider");
        m_SFXVolumeSlider = m_RootElement.Q<Slider>("sfx-volume-slider");
        m_ResolutionDropdown = m_RootElement.Q<DropdownField>("resolution-dropdown");
        m_fullScreenToggle = m_RootElement.Q<Toggle>("fullscreen-toggle");
    }
    private void RegisterCallbacks()
    {
        m_EventRegistry.RegisterCallback<ClickEvent>(m_BackButton, evt => UIScreenEvents.ScreenClosed?.Invoke());
        m_EventRegistry.RegisterDropdownValueChangedCallback(m_ResolutionDropdown, ResolutionDropdownChangeEventHandler);

        m_EventRegistry.RegisterValueChangedCallback<float>(m_MasterVolumeSlider, MasterVolumeChangeHandler);
        m_EventRegistry.RegisterValueChangedCallback<float>(m_SFXVolumeSlider, SFXVolumeChangeHandler);
        m_EventRegistry.RegisterValueChangedCallback<float>(m_MusicVolumeSlider, MusicVolumeChangeHandler);
        m_EventRegistry.RegisterValueChangedCallback<bool>(m_fullScreenToggle, FullScreenToggleChangeHandler);
    }
    private void SoundEffectsVolumeSliderSetHandler(float volume)
    {
        m_SFXVolumeSlider.value = volume;
    }

    private void MusicVolumeSliderSetHandler(float volume)
    {
        m_MusicVolumeSlider.value = volume;
    }

    private void MasterVolumeSliderSetHandler(float volume)
    {
        m_MasterVolumeSlider.value = volume;
    }

    private void MusicVolumeChangeHandler(float newValue)
    {
        SettingsEvents.MusicVolumeSliderChanged?.Invoke(newValue);
    }

    private void SFXVolumeChangeHandler(float newValue)
    {
        SettingsEvents.SoundEffectsVolumeSliderChanged?.Invoke(newValue);
    }

    private void MasterVolumeChangeHandler(float newValue)
    {
        SettingsEvents.MasterVolumeSliderChanged?.Invoke(newValue);
    }

    private void ResolutionDropdownChangeEventHandler(string evt)
    {
        SettingsEvents.ResolutionChanged?.Invoke(m_ResolutionDropdown.index);
    }

    private void ResolutionDropdownOptionsSetHandler(string[] options)
    {
        m_ResolutionDropdown.choices = new System.Collections.Generic.List<string>(options);
    }

    private void ResolutionDropdownIndexSetHandler(int index)
    {
        m_ResolutionDropdown.index = index;
    }

    private void FullScreenToggleChangeHandler(bool newValue)
    {
        SettingsEvents.FullScreenToggleChanged?.Invoke(newValue);
    }
}
