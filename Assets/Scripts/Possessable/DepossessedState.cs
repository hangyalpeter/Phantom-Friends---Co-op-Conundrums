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
        Debug.Log("Enter Depossessed State");
        possessable.Rb.constraints = RigidbodyConstraints2D.FreezeAll;

        possessable.GetComponent<PosessableMovement>().SetPossessedFalse();
    }

    public void Update()
    {

    }

    public void Exit()
    {
        // intentionally empty - could be the same as possessed state's Enter
    }
}
