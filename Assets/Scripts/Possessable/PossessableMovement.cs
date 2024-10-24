using Unity.Netcode;
using UnityEngine;

public class PosessableMovement : NetworkBehaviour
{

    private float dirX = 0f;
    private float dirY = 0f;
    private float moveSpeed = 7f;

    private Rigidbody2D rb;

    private bool isPossessed = false;
    private SpriteRenderer rbSprite;

    public bool IsPossessed => isPossessed;

    private NetworkVariable<Vector2> velocity = new NetworkVariable<Vector2>();
    private NetworkVariable<bool> isPossessedNetwork = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isPossessedNetwork.OnValueChanged += (oldValue, newValue) =>
        {
            isPossessed = newValue;
            if (newValue == true)
            {
                rb.constraints = RigidbodyConstraints2D.None;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        };

        velocity.OnValueChanged += (oldValue, newValue) =>
        {
            rb.velocity = newValue;
        };
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbSprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner || !isPossessed || Equals(Time.timeScale, 0f)) { return; }

        if (dirX > 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, transform.rotation.w);
        }
        else if (dirX < 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        }

        dirX = Input.GetAxisRaw("Horizontal_Ghost");
        dirY = Input.GetAxisRaw("Vertical_Ghost");

        UpdateVelocityServerRpc(new Vector2(dirX * moveSpeed, dirY * moveSpeed));
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateVelocityServerRpc(Vector2 velocityNew)
    {
        velocity.Value = velocityNew;
    }

    [ServerRpc (RequireOwnership = false)]
    private void TogglePossessedServerRpc(bool possessed)
    {
        isPossessedNetwork.Value = possessed;
    }


    public void SetPossessedTrue()
    {
        if (IsServer)
        {
            isPossessedNetwork.Value = true;
        } else 
        {
            TogglePossessedServerRpc(true);
        }
    }
    public void SetPossessedFalse()
    {
        if (IsServer)
        {
            isPossessedNetwork.Value = false;
            velocity.Value = Vector2.zero;
        } else
        {
            TogglePossessedServerRpc(false);
            UpdateVelocityServerRpc(Vector2.zero);
        }
    }
}
