using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    private bool isRunning = true;
    float elapsedTime = 0f;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

    [SerializeField] private GameObject endLevelPanel;
    [SerializeField] private AudioSource levelFinishSound;
    
    void Start()
    {
        Debug.Log("best time: " + PlayerPrefs.GetFloat(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_BestCompletionTime"));
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);
            int milliseconds = (int)((elapsedTime * 1000) % 1000);
            timerText.text = string.Format("Time: {0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);

        }
    }

    private void SaveBestCompletionTime()
    {
        var currentLevelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (PlayerPrefs.HasKey(currentLevelName + "_BestCompletionTime"))
        {
            var bestCompletionTime = PlayerPrefs.GetFloat(currentLevelName + "_BestCompletionTime");
            if (elapsedTime < bestCompletionTime)
            {
                PlayerPrefs.SetFloat(currentLevelName + "_BestCompletionTime", elapsedTime);
            }
        }
        else
        {
            PlayerPrefs.SetFloat(currentLevelName + "_BestCompletionTime", elapsedTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player_Child")
        {
            levelFinishSound.Play();
            endLevelPanel.SetActive(true);
            isRunning = false;
            SaveBestCompletionTime();
            Time.timeScale = 0f;
        }

    }
}
