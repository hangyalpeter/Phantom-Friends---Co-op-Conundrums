using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TMPro.TMP_Dropdown resolutionDropdown;
    [SerializeField] private AudioMixer masterAudioMixer;
    [SerializeField] private AudioMixer musicAudioMixer;

    Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        // Loop through the resolutions and add them to the dropdown
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
            
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(5);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (settingsPanel.activeSelf)
            {
                settingsPanel.SetActive(false);
                if (pausePanel != null)
                {
                    pausePanel.SetActive(true);
                }
            }
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void Back()
    {

        settingsPanel.SetActive(false);
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetMasterVolume(float volume)
    {
        masterAudioMixer.SetFloat("volume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        musicAudioMixer.SetFloat("volume", volume);
    }
    
    public void OpenOptions()
    {
        settingsPanel.SetActive(true);
    }
 
}
