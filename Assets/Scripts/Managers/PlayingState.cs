
using UnityEngine;

public class PlayingState : IGameState
{
    private GameStateManager context;

    public PlayingState(GameStateManager context)
    {
        this.context = context;
    }

    public void EnterState()
    {
        Debug.Log("Entered Playing State");
        PlayerChildLife.OnPlayerChildDeath += OnPlayerChildDeath;
        UIScreenEvents.HideAllScreens?.Invoke();
        if (context.ElapsedTime <= 0 )
        {
            context.UpdateElapsedTimeSync(0, false);
        }
        Time.timeScale = 1f;
    }

    public void UpdateState()
    {
        context.UpdateElapsedTimeSync(Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            context.TransitionToState(context.PausedState);
        }
    }

    private void OnPlayerChildDeath()
    {
        context.TransitionToState(context.PausedState);
    }

    public void ExitState()
    {
        PlayerChildLife.OnPlayerChildDeath -= OnPlayerChildDeath;
        Debug.Log("Exiting Playing State");
    }
}
