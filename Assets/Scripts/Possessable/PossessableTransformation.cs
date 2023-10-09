using UnityEngine;
using UnityEngine.Events;

public class PossessableTransformation : MonoBehaviour
{
    [SerializeField] private GameObject ghost;
    [SerializeField] private float distanceOffset = 2f;
    private Rigidbody2D rbGhost;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public UnityEvent OnPossess;
    public UnityEvent OnDePossess;
    private bool isPossessed = false;

    void Start()
    {
        rbGhost = ghost.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isPossessed && Input.GetKeyDown(KeyCode.R))
        {
            UnPossess();
        }

        if (isWithinRange() && !isPossessed && Input.GetKeyDown(KeyCode.E))
        {
            Possess();
        }

    }

    private void UnPossess()
    {
        isPossessed = false;
        rbGhost.GetComponent<SpriteRenderer>().enabled = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        OnDePossess.Invoke();
    }

    private bool isWithinRange() 
    {
        return Vector2.Distance(transform.position, ghost.transform.position) < distanceOffset;
    }


    public void Possess()
    {
        isPossessed = true;
        rb.constraints = RigidbodyConstraints2D.None;
        rbGhost.velocity = Vector2.zero;
        rbGhost.angularVelocity = 0f;
        rbGhost.transform.position = transform.position;
        rbGhost.transform.rotation = transform.rotation;
        rbGhost.GetComponent<SpriteRenderer>().enabled = false;

        OnPossess.Invoke();
    }

}
