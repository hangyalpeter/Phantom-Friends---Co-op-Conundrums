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

    public NetworkTime ElapsedTimee;

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
    }

    private void OnDisable()
    {
        LevelManager.LevelChanged -= ResetElapsedTime;
        GameEvents.DungeonFinished -= TransitionToGameOverState;
        GameEvents.LevelFinished -= TransitionToGameOverState;
        GameEvents.OnNewDungeon -= HandleNewDungeon;
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
