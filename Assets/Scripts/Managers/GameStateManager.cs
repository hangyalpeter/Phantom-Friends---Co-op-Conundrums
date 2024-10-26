using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStateType
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}
public class GameStateManager : NetworkBehaviour
{
    public IGameState CurrentState { get; private set; }

    public PlayingState PlayingState { get; private set; }
    public PausedState PausedState { get; private set; }
    public GameOverState GameOverState { get; private set; }
    public MainMenuState MainMenuState { get; private set; }

    public float ElapsedTime { get; set; }
    private NetworkVariable<float> ElapsedTimeSynced = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool isLocalPlayerReady = false;
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Scene selectedScene;

    private Action OnAllReady;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        ElapsedTimeSynced.OnValueChanged += (float previousValue, float newValue) =>
            {
                ElapsedTime = newValue;
            };
    }


    private void OnEnable()
    {
        LevelManager.LevelChanged += ResetElapsedTime;
        GameEvents.DungeonFinished += TransitionToGameOverState;
        GameEvents.LevelFinished += TransitionToGameOverState;
        GameEvents.OnNewDungeon += HandleNewDungeon;
        UIScreenEvents.OnClientReady += UIScreenEvents_OnClientReady;
        UIScreenEvents.OnHostReady += UIScreenEvents_OnHostReady;
        OnAllReady += OnStartGame;
    }

    private void OnStartGame()
    {
        ResetLocalPlayerReadyClientRpc();
        UIScreenEvents.OnHostStart?.Invoke(selectedScene);
        UIScreenEvents.HideAllScreens?.Invoke();
    }

    [ClientRpc]
    private void ResetLocalPlayerReadyClientRpc()
    {
        isLocalPlayerReady = false;
        playerReadyDictionary = new Dictionary<ulong, bool>();
    }
    private void UIScreenEvents_OnHostReady(Scene scene)
    {
        isLocalPlayerReady = true;
        selectedScene = scene;

        UIScreenEvents.HideAllScreens?.Invoke();
        UIScreenEvents.WaitingForPlayersScreenShown?.Invoke();
        SetPlayerReadyServerRpc();
    }

    private void UIScreenEvents_OnClientReady()
    {
        isLocalPlayerReady = true;
        UIScreenEvents.HideAllScreens?.Invoke();
        UIScreenEvents.WaitingForPlayersScreenShown?.Invoke();

        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                allReady = false;
                break;
            }
        }
        if (allReady)
        {
            OnAllReady?.Invoke();
        }
    }

    private void OnDisable()
    {
        LevelManager.LevelChanged -= ResetElapsedTime;
        GameEvents.DungeonFinished -= TransitionToGameOverState;
        GameEvents.LevelFinished -= TransitionToGameOverState;
        GameEvents.OnNewDungeon -= HandleNewDungeon;
        UIScreenEvents.OnClientReady -= UIScreenEvents_OnClientReady;
        UIScreenEvents.OnHostReady -= UIScreenEvents_OnHostReady;
        OnAllReady -= OnStartGame;
    }

    public void UpdateElapsedTimeSync(float elapsedTime, bool update=true)
    {
        if (IsServer)
        {
            if (update)
            {
                ElapsedTimeSynced.Value += elapsedTime;
            }
            else
            {
                ElapsedTimeSynced.Value = elapsedTime;
            }
        }
    }

    private void HandleNewDungeon()
    {
        ResetElapsedTime();
        UIScreenEvents.ScreenClosed?.Invoke();
        TransitionToState(PlayingState);
    }

    private void TransitionToGameOverState()
    {
        TransitionToState(GameOverState);
    }

    private void Awake()
    {
        PlayingState = new PlayingState(this);
        PausedState = new PausedState(this);
        GameOverState = new GameOverState(this);
        MainMenuState = new MainMenuState(this);
        playerReadyDictionary = new Dictionary<ulong, bool>();

        TransitionToState(MainMenuState);

        UIScreenEvents.MainMenuShown += () => TransitionToState(MainMenuState);
    }

    private void Update()
    {
        CurrentState?.UpdateState();
    }

    private void TransitionStateee(IGameState newState)
    {
        CurrentState?.ExitState();
        CurrentState = newState;
        CurrentState?.EnterState();

    }


    public void TransitionToState(IGameState newState)
    {
        if (IsServer)
        {
            TransitionToStateClientRpc(GetGameStateTypeFromState(newState));
        }
    }

    [ServerRpc]
    private void TransitionToStateServerRpc(GameStateType newStateType)
    {
        TransitionStateee(GetStateFromType(newStateType));
    }

    [ClientRpc]
    private void TransitionToStateClientRpc(GameStateType newStateType)
    {
        TransitionStateee(GetStateFromType(newStateType));
    }

    private GameStateType GetGameStateTypeFromState(IGameState state)
    {
        if (state is MainMenuState)
            return GameStateType.MainMenu;
        else if (state is PlayingState)
            return GameStateType.Playing;
        else if (state is PausedState)
            return GameStateType.Paused;
        else if (state is GameOverState)
            return GameStateType.GameOver;

        return GameStateType.MainMenu; 
    }

    private IGameState GetStateFromType(GameStateType type)
    {
        switch (type)
        {
            case GameStateType.MainMenu:
                return new MainMenuState(this);
            case GameStateType.Playing:
                return new PlayingState(this);
            case GameStateType.Paused:
                return new PausedState(this);
            case GameStateType.GameOver:
                return new GameOverState(this);
            default:
                return new MainMenuState(this);
        }
    }

    private void ResetElapsedTime()
    {
        if (IsServer)
        {
            ElapsedTimeSynced.Value = 0;
            TransitionToState(PlayingState);
        }
    }
}
