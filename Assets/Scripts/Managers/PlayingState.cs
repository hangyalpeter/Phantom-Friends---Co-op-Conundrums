
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
        if (context.ElapsedTime <= 0 )
        {
            context.ElapsedTime = 0f;
        }
        Time.timeScale = 1f;
    }

    public void UpdateState()
    {
        context.ElapsedTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            context.TransitionToState(context.PausedState);
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Playing State");
    }
}
