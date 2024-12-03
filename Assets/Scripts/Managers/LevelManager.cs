using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public enum Scene { First, Main_Menu, Level_1, Level_2, Dungeon_Crawler };
public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance { get; private set; }

    public static Action LevelChanged;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += NetworkManager_OnLoadEventCompleted;
    }

    private void NetworkManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        UIScreenEvents.ScreenClosed?.Invoke();
        if (sceneName == "Main_Menu")
        {
            UIScreenEvents.MainMenuShown?.Invoke();
        }
    }

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

    private void LoadLevel(string levelName)
    {
        if (!IsServer) return;

        NetworkManager.Singleton.SceneManager.LoadScene(levelName, LoadSceneMode.Single);
        LevelChanged?.Invoke();
    }

    private void SubscribeToEvents()
    {
        UIScreenEvents.OnLevelRestart += OnLevelRestartClicked;
        UIScreenEvents.MainMenuClicked += OnMainMenuClicked;
        UIScreenEvents.OnNextLevel += OnNextLevel;
        UIScreenEvents.OnHostStart += UIScreenEvents_OnHostStart;
        UIScreenEvents.OnBackToTitleScreen += UIScreenEvents_OnBackToTitleScreen;

        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Main_Menu" && StartingSceneController.ChoosenPlayMode == StartingSceneController.PlayMode.Client)
        {
            UIScreenEvents.MainMenuShown?.Invoke();
        }
    }

    private void UIScreenEvents_OnBackToTitleScreen()
    {
        NetworkManager.Singleton.Shutdown();
        UIScreenEvents.HideAllScreens?.Invoke();
        SceneManager.LoadScene(Scene.First.ToString());
    }

    private void UIScreenEvents_OnHostStart(Scene scene)
    {
        LoadLevel(scene.ToString());
    }

    private void UnSubscribeFromEvents()
    {
        UIScreenEvents.OnLevelRestart -= OnLevelRestartClicked;
        UIScreenEvents.MainMenuClicked -= OnMainMenuClicked;
        UIScreenEvents.OnNextLevel -= OnNextLevel;

        GameEvents.OnLevelRestart -= OnLevelRestartClicked;
        UIScreenEvents.OnHostStart -= UIScreenEvents_OnHostStart;
        UIScreenEvents.OnBackToTitleScreen -= UIScreenEvents_OnBackToTitleScreen;
    }

    private void OnLevelRestartClicked()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        UIScreenEvents.ScreenClosed?.Invoke();
        LevelChanged?.Invoke();
    }
    private void OnMainMenuClicked()
    {

        LoadLevel(Scene.Main_Menu.ToString());
        UIScreenEvents.ScreenClosed?.Invoke();
        UIScreenEvents.MainMenuShown?.Invoke();
    }

    private void OnNextLevel()
    {
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        if (!IsServer) { return; }

        string currentSceneName = SceneManager.GetActiveScene().name;

        int currentLevelNumber = int.Parse(currentSceneName.Split('_')[1]);

        int nextLevelNumber = currentLevelNumber + 1;

        string nextSceneName = "Level_" + nextLevelNumber;

        NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);

        UIScreenEvents.ScreenClosed?.Invoke();
        LevelChanged?.Invoke();
    }

}
