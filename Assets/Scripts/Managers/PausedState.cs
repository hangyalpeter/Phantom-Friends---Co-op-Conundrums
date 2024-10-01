using UnityEngine;

public class PausedState : IGameState
{
    private GameStateManager context;

    public PausedState(GameStateManager context)
    {
        this.context = context;
    }

    public void EnterState()
    {
        Time.timeScale = 0f;
        UIScreenEvents.PauseShown?.Invoke();

        int minutes = (int)(context.ElapsedTime / 60);
        int seconds = (int)(context.ElapsedTime % 60);
        int milliseconds = (int)((context.ElapsedTime * 1000) % 1000);

        GameEvents.GamePaused?.Invoke(string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds));

    }

    public void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            context.TransitionToState(context.PlayingState);
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Paused State");
        Time.timeScale = 1f;
        UIScreenEvents.ScreenClosed?.Invoke();
    }
}
