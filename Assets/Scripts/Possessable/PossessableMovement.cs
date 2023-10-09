using UnityEngine;
using UnityEngine.Events;

public class PosessableMovement : MonoBehaviour
{
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
