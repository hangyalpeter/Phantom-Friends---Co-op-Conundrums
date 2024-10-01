using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public static Action LevelChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnSubscribeFromEvents();
    }

    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadLevelAsync(levelName));
    }
    private void OnGameStartClicked()
    {
        StartCoroutine(LoadLevelAsync("Level 1"));
    }

    private IEnumerator LoadLevelAsync(string levelName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        LevelChanged?.Invoke();

    }

    private void SubscribeToEvents()
    {
        UIScreenEvents.OnGameStart += OnGameStartClicked;
        UIScreenEvents.OnDungeonGameStart += OnDungeonGameStartClicked;
        UIScreenEvents.OnLevelRestart += OnLevelRestartClicked;
        UIScreenEvents.MainMenuClicked += OnMainMenuClicked;
        UIScreenEvents.OnNextLevel += OnNextLevel;
        UIScreenEvents.OnLevelSelected += OnLevelSelected;

        GameEvents.OnLevelRestart += OnLevelRestartClicked;

    }

    private void UnSubscribeFromEvents()
    {
        UIScreenEvents.OnGameStart -= OnGameStartClicked;
        UIScreenEvents.OnDungeonGameStart -= OnDungeonGameStartClicked;
        UIScreenEvents.OnLevelRestart -= OnLevelRestartClicked;
        UIScreenEvents.MainMenuClicked -= OnMainMenuClicked;
        UIScreenEvents.OnNextLevel -= OnNextLevel;
        UIScreenEvents.OnLevelSelected -= OnLevelSelected;

        GameEvents.OnLevelRestart -= OnLevelRestartClicked;

    }


    private void OnDungeonGameStartClicked()
    {
        StartCoroutine(LoadLevelAsync("Level 3"));
    }

    private void OnLevelRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UIScreenEvents.ScreenClosed?.Invoke();
        LevelChanged?.Invoke();
    }
    private void OnMainMenuClicked()
    {
        StartCoroutine(LoadMainMenu());
    }

    private IEnumerator LoadMainMenu()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        UIScreenEvents.ScreenClosed?.Invoke();
        UIScreenEvents.MainMenuShown?.Invoke();
        LevelChanged?.Invoke();
    }
    private void OnNextLevel()
    {
        StartCoroutine(LoadNextLevel());
    }
    private IEnumerator LoadNextLevel()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        UIScreenEvents.ScreenClosed?.Invoke();
        LevelChanged?.Invoke();
    }

    private void OnLevelSelected(string name)
    {
        UIScreenEvents.ScreenClosed?.Invoke();
        StartCoroutine(LoadLevelAsync(name));
        LevelChanged?.Invoke();
    }



}
