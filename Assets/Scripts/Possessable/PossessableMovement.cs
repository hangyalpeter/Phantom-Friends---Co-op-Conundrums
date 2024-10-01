using UnityEngine;

public class PosessableMovement : MonoBehaviour
{

    [SerializeField] private Transform secondPlayer;
    [SerializeField] private float maxDistanceFromSecondPlayer = 35f;

    private float dirX = 0f;
    private float dirY = 0f;
    private float moveSpeed = 7f;

    private Rigidbody2D rb;

    private bool isPossessed = false;
    private SpriteRenderer rbSprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbSprite = GetComponent<SpriteRenderer>();
        secondPlayer = GameObject.FindGameObjectWithTag("Player_Child").transform;
    }

    void Update()
    {
        if (dirX > 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
        }
        else if (dirX < 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        }

        if (isPossessed && !Equals(Time.timeScale, 0f))
        {
            Move();
        }
   
    }
    private void Move()
    {
        float distance = Vector2.Distance(gameObject.transform.position, secondPlayer.position);

        //TODO: don't allow player and ghost to be too far away

        /* if (distance > maxDistanceFromSecondPlayer)
         {
             Vector2 direction = (transform.position - secondPlayer.position).normalized;
             transform.position = secondPlayer.position + new Vector3(direction.x, direction.y, 0) * maxDistanceFromSecondPlayer;
         }
 */

        dirX = Input.GetAxisRaw("Horizontal_Ghost");
        dirY = Input.GetAxisRaw("Vertical_Ghost");

        rb.velocity = new Vector2(dirX * moveSpeed, dirY * moveSpeed);
    }

    public void SetPossessedTrue()
    {
        isPossessed = true;
    }
    public void SetPossessedFalse()
    {
        isPossessed = false;
        rb.velocity = Vector2.zero;
    }
}
