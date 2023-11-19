using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] public TextMeshProUGUI? bestTimeText;
    [SerializeField] public TextMeshProUGUI? buttonLabel;

    private void Start()
    {
        var bestTime = PlayerPrefs.GetFloat(buttonLabel.text + "_BestCompletionTime");
        
        int minutes = (int)(bestTime / 60);
        int seconds = (int)(bestTime % 60);
        int milliseconds = (int)((bestTime * 1000) % 1000);
        bestTimeText.text = string.Format("Time: {0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
    public void Level1()
    {
        levelSelectPanel.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void Level2()
    {
        levelSelectPanel.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }

    public void Back()
    {
        levelSelectPanel.SetActive(false);
        //mainMenuPanel.SetActive(true);
    }
   public void OnLevelSelect()
    {
        //mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

}
