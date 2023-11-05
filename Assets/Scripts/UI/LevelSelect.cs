using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject mainMenuPanel;
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
        mainMenuPanel.SetActive(true);
    }
   public void OnLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

}
