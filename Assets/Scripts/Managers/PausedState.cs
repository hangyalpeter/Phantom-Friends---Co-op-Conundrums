using System;
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

        int minutes = (int)(context.ElapsedTime / 60);
        int seconds = (int)(context.ElapsedTime % 60);
        int milliseconds = (int)((context.ElapsedTime * 1000) % 1000);

        GameEvents.GamePaused?.Invoke(string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds));

    }

    private void SettingsShown()
    {
        settingsShown = true;

        UIScreenEvents.ScreenClosed += () => settingsShown = false;
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
        Debug.Log("Exiting Paused State");
        Time.timeScale = 1f;
        UIScreenEvents.ScreenClosed -= () => settingsShown = false;
        UIScreenEvents.ScreenClosed?.Invoke();
    }
}
