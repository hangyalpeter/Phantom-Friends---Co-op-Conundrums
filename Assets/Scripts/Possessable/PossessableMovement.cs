using UnityEngine;
using UnityEngine.Events;

public class PosessableMovement : MonoBehaviour
{

    [SerializeField] private Transform secondPlayer;
    [SerializeField] private float maxDistanceFromSecondPlayer = 10f;

    private float dirX = 0f;
    private float dirY = 0f;
    private float moveSpeed = 7f;

    private Rigidbody2D rb;

    private bool isPossessed = false;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isPossessed)
        {
            Move();
        }
    }
    private void Move()
    {
        float distance = Vector2.Distance(transform.position, secondPlayer.position);

        if (distance > maxDistanceFromSecondPlayer)
        {
            Vector2 direction = (transform.position - secondPlayer.position).normalized;
            transform.position = secondPlayer.position + new Vector3(direction.x, direction.y, 0) * maxDistanceFromSecondPlayer;
        }


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
    }
}
