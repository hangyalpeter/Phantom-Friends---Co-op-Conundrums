using UnityEngine;

public class GhostController : MonoBehaviour
{

    [SerializeField] private Transform secondPlayer;
    [SerializeField] private float maxDistanceFromSecondPlayer = 10f; 
    
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private float dirX = 0f;
    private float dirY = 0f;
    private float moveSpeed = 7f;

    public bool isPossessed = false;

    private enum MovementState { idle, moving}
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        if (float.Equals(Time.timeScale, 0f))
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, secondPlayer.position);

        if (distance > maxDistanceFromSecondPlayer)
        {
            Vector2 direction = (transform.position - secondPlayer.position).normalized;
            transform.position = secondPlayer.position + new Vector3(direction.x, direction.y, 0) * maxDistanceFromSecondPlayer;
        }

        dirX = Input.GetAxisRaw("Horizontal_Ghost");
        dirY = Input.GetAxisRaw("Vertical_Ghost");
        rb.velocity = new Vector2(dirX * moveSpeed, dirY * moveSpeed);
        
        UpdateAnimationState();

    }
    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f || dirY > 0f)
        {
            state = MovementState.moving;
            sr.flipX = true;
        }
        else if (dirX < 0f || dirY < 0f)
        {
            state = MovementState.moving;
            sr.flipX = false;
        }
        else
        {
            state = MovementState.idle;
        }

        anim.SetInteger("state", (int)state);
 
    }

}
