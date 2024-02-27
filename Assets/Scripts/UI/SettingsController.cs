using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{   
    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;

    private Button backButton;
    
    private DropdownField resolutionDropdown;
    private DropdownField qualityDropdown;

    Resolution[] resolutions;

    private VisualElement settingsPanel;
    private VisualElement body;

    [SerializeField] private AudioMixer musicAudioMixer;
    void Start()
    {
        
        var root = GetComponent<UIDocument>().rootVisualElement;

        settingsPanel = root.Q<VisualElement>("SettingsPanel");
        backButton = root.Q<Button>("Back_Button");

        settingsPanel.style.display = DisplayStyle.None;
        body = root.Q<VisualElement>("body");

        masterVolumeSlider = root.Q<Slider>("MasterVolume");
        musicVolumeSlider = root.Q<Slider>("MusicVolume");

        resolutionDropdown = root.Q<DropdownField>("ResolutionDropdown");
        qualityDropdown = root.Q<DropdownField>("QualityDropdown");

        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0f);



        resolutions = Screen.resolutions;
        resolutionDropdown.choices = new List<string>();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            resolutionDropdown.choices.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }

        }

        resolutionDropdown.index = currentResolutionIndex;

        resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            Resolution resolution = resolutions[resolutionDropdown.index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        });

        QualitySettings.SetQualityLevel(5);

        
        musicVolumeSlider.RegisterValueChangedCallback(evt =>
        {

            musicAudioMixer.SetFloat("volume", evt.newValue);
            PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
            PlayerPrefs.Save();
        });

        backButton.clicked += () => CloseSettings();


    }
   


    private void CloseSettings()
    {
        settingsPanel.style.display = DisplayStyle.None;
        body.style.display = DisplayStyle.Flex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
