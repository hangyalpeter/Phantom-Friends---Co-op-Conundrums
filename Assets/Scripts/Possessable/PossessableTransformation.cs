using Unity.Netcode;
using UnityEngine;

public class PossessableTransformation : NetworkBehaviour
{
    [SerializeField] private float distanceOffset = 2f;
    [SerializeField] private float possessionDuration = 15f;

    private PossessMediator mediator;

    public float PossessionDuration => possessionDuration;
    public Rigidbody2D Rb { get; private set; }
    public GameObject Ghost { get; private set; }


    private void Awake()
    {
        mediator = FindObjectOfType<PossessMediator>();
    }

    public bool IsWithinTransformationRange(GameObject ghost)
    {
        return Vector2.Distance(transform.position, ghost.transform.position) < distanceOffset;
    }

    public void RequestPossession()
    {
        mediator.RegisterPossessionRequest(this);
    }
}
