using UnityEngine;

public class DepossessedState : IState
{
    private readonly PossessableTransformation possessable;

    public DepossessedState(PossessableTransformation possessable)
    {
        this.possessable = possessable;
    }

    public void Enter()
    {
        possessable.Rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            possessable.RequestPossession();
        }
    }

    public void Exit()
    {
        // intentionally empty - could be the same as possessed state's Enter
    }
}
