using UnityEngine;

public class PossessedState : IState
{
    private readonly PossessableTransformation possessable;

    public PossessedState(PossessableTransformation possessable)
    {
        this.possessable = possessable;
    }

    public void Enter()
    {
        possessable.Ghost.GetComponent<Rigidbody2D>().transform.position = possessable.Rb.gameObject.transform.position;
        possessable.Ghost.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        possessable.GetComponent<PosessableMovement>().SetPossessedTrue();

    }

    public void Update()
    {
        possessable.Ghost.GetComponent<Rigidbody2D>().transform.position = possessable.Rb.gameObject.transform.position;
    }

    public void Exit()
    {
    }
}
