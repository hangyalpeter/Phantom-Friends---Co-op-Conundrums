using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    float elapsedTime = 0f;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsPanel;
    // Update is called once per frame
    void Update()
    {
       if (Input.GetKeyDown(KeyCode.Escape))
       { 
            if (isPaused)
            {
                Resume();
                UIScreenEvents.ScreenClosed?.Invoke();
            }
            else
            {
                Pause();
                UIScreenEvents.SettingsShownInPlayMode?.Invoke(true);
            }
       }

        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            int milliseconds = (int)((elapsedTime * 1000) % 1000);
            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

        }

    }

    public void Resume()
    {
        if (optionsPanel.activeSelf)
        {
            optionsPanel.SetActive(false);
        }
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

    }

    public void Pause()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    } 

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void OpenOptions()
    {
        pauseMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        pauseMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }


}

