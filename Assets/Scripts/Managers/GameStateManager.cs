using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public IGameState CurrentState { get; private set; }

    public PlayingState PlayingState { get; private set; }
    public PausedState PausedState { get; private set; }
    public GameOverState GameOverState { get; private set; }

    public float ElapsedTime { get; set; }


    private void OnEnable()
    {
        LevelManager.LevelChanged += ResetElapsedTime;
        GameEvents.DungeonFinished += TransitionToGameOverState;
        GameEvents.LevelFinished += TransitionToGameOverState;
    }

    private void OnDisable()
    {
        LevelManager.LevelChanged -= ResetElapsedTime;
        GameEvents.DungeonFinished -= TransitionToGameOverState;
        GameEvents.LevelFinished -= TransitionToGameOverState;
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

        TransitionToState(PlayingState);
    }

    private void Update()
    {
        CurrentState?.UpdateState();
    }

    public void TransitionToState(IGameState newState)
    {
        CurrentState?.ExitState();
        CurrentState = newState; 
        CurrentState?.EnterState();
    }

    private void ResetElapsedTime()
    {
        ElapsedTime = 0;
    }
}
