using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance { get; private set; }

    public static Action LevelChanged;

    private bool alreadyLoadedOnce = false;

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
    private void OnGameStartClicked()
    {
        LoadLevel("Level 1");
    }

    private void LoadLevel(string levelName)
    {
        if (!IsServer) return;

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                playerObject.Despawn();
            }
        }

        NetworkManager.Singleton.SceneManager.LoadScene(levelName, LoadSceneMode.Single);
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
        LoadLevel("Dungeon Crawler");
    }

    private void OnLevelRestartClicked()
    {
        if (!IsServer) return;

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                playerObject.Despawn();
            }
        }

        NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        UIScreenEvents.ScreenClosed?.Invoke();
        LevelChanged?.Invoke();
    }
    private void OnMainMenuClicked()
    {
        LoadLevel("Main Menu");
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
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                playerObject.Despawn();
            }
        }

        NetworkManager.Singleton.SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1).ToString(), LoadSceneMode.Single);

        UIScreenEvents.ScreenClosed?.Invoke();
        LevelChanged?.Invoke();
    }

    private void OnLevelSelected(string name)
    {
        UIScreenEvents.ScreenClosed?.Invoke();
        LoadLevel(name);
        LevelChanged?.Invoke();
    }
}
