using Unity.Netcode;
using UnityEngine;

public class PossessableTransformation : NetworkBehaviour
{
    [SerializeField] private float distanceOffset = 2f;
    [SerializeField] private float possessionDuration = 15f;

    private PossessMediator mediator;
    private IState currentState;

    public float PossessionDuration => possessionDuration;
    public Rigidbody2D Rb { get; private set; }
    public GameObject Ghost { get; private set; }

    public IState DepossessedState { get; private set; }
    public IState PossessedState { get; private set; }

    private void Awake()
    {
        mediator = FindObjectOfType<PossessMediator>();
        Ghost = mediator.Ghost != null ? mediator.Ghost : null;

        DepossessedState = new DepossessedState(this);
        PossessedState = new PossessedState(this);
        Rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        SetState(DepossessedState); 
       
    }

    private void Update()
    {
        if (Ghost == null)
        {
            Ghost = mediator.Ghost;
        }
        currentState.Update();
    }

    public void SetState(IState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Possess()
    {
        SetState(PossessedState);
    }

    public void Depossess()
    {
        SetState(DepossessedState);
    }

    public bool IsWithinTransformationRange(GameObject ghost)
    {
        return Vector2.Distance(transform.position, ghost.transform.position) < distanceOffset;
    }

    public void RequestPossession()
    {
        if (!mediator.IsPossessing() && IsWithinTransformationRange(mediator.Ghost))
        {
            mediator.RegisterPossessionRequest(this);
        }
    }
}
