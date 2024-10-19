
using UnityEngine;

public class MainMenuState : IGameState
{
    private GameStateManager context;
    public MainMenuState(GameStateManager context)
    {
        this.context = context;
    }

    public void EnterState()
    {
        Debug.Log("Enter Main Menu state");
    }
    public void UpdateState()
    {

    }
    public void ExitState()
    {

    }


}
