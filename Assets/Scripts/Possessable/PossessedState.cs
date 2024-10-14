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
        if (possessable.GetComponent<HealthComponent>() != null)
        {
            possessable.GetComponent<HealthComponent>().OnDied += Exit;
        }
        possessable.Rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        possessable.Ghost.GetComponent<SpriteRenderer>().enabled = false;
        possessable.Ghost.GetComponent<BoxCollider2D>().enabled = false;

        possessable.Ghost.GetComponent<Rigidbody2D>().transform.position = possessable.Rb.gameObject.transform.position;
        possessable.Ghost.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        possessable.GetComponent<PosessableMovement>().SetPossessedTrue();

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            possessable.RequestDepossession();
        }
        possessable.Ghost.GetComponent<Rigidbody2D>().transform.position = possessable.Rb.gameObject.transform.position;
    }

    public void Exit()
    {
        possessable.Ghost.GetComponent<SpriteRenderer>().enabled = true;
        possessable.Ghost.GetComponent<BoxCollider2D>().enabled = true;
    }
}
