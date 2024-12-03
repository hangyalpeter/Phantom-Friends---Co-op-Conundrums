using UnityEngine;

public class PausedState : IGameState
{
    private GameStateManager context;
    private bool settingsShown = false;

    public PausedState(GameStateManager context)
    {
        this.context = context;
    }

    public void EnterState()
    {
        Time.timeScale = 0f;
        UIScreenEvents.PauseShown?.Invoke();

        UIScreenEvents.SettingsShown += SettingsShown;

        UIScreenEvents.ScreenClosed += ScreenClosed;

        int minutes = (int)(context.ElapsedTime / 60);
        int seconds = (int)(context.ElapsedTime % 60);
        float hundreth = (float)((context.ElapsedTime - Mathf.Floor(context.ElapsedTime)) * 100);

        GameEvents.GamePaused?.Invoke(string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundreth));

    }
    private void SettingsShown()
    {
        settingsShown = true;

    }

    private void ScreenClosed()
    {
        if (!settingsShown)
        {
            context.TransitionToState(context.PlayingState);
        }
        settingsShown = false;
    }

    public void UpdateState()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !settingsShown)
        {
            context.TransitionToState(context.PlayingState);
        }
    }

    public void ExitState()
    {
        UIScreenEvents.SettingsShown -= SettingsShown;
        UIScreenEvents.ScreenClosed -= ScreenClosed;

        Time.timeScale = 1f;
        UIScreenEvents.ScreenClosed?.Invoke();
    }
}
