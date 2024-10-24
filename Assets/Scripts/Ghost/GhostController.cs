using Unity.Netcode;
using UnityEngine;

public class GhostController : NetworkBehaviour
{
    private PossessMediator mediator;
    
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;

    private float dirX = 0f;
    private float dirY = 0f;
    private readonly float moveSpeed = 7f;

    public bool IsPossessed { get; set; } = false;

    private bool possessRequested = false;
    private bool dePossessRequested = false;
    private enum MovementState { idle, moving}

    private NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isSpriteEnabled = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<bool> isPossessedNetwork = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        mediator = FindObjectOfType<PossessMediator>();

        isFlipped.OnValueChanged += (oldValue, newValue) =>
        {
            sr.flipX = newValue;
        };

        isSpriteEnabled.OnValueChanged += (oldValue, newValue) =>
        {
            sr.enabled = newValue;
        };

        isPossessedNetwork.OnValueChanged += (oldValue, newValue) =>
        {
            IsPossessed = newValue;
        };

        HealthComponent.OnPossessedObjectDies += RequestDepossession;
    }

    private void OnDisable()
    {
        HealthComponent.OnPossessedObjectDies -= RequestDepossession;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            possessRequested = true;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            dePossessRequested = true;
        }
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;

        if (Equals(Time.timeScale, 0f))
        {
            return;
        }

        if (possessRequested)
        {
            possessRequested = false;
            var target = GetClosestPossessableTarget();

            if (target != null)
            {
                RequestOwnershipServerRpc(target.NetworkObjectId);
                target.GetComponent<PossessableTransformation>().RequestPossession();
            }
        }

        if (dePossessRequested)
        {
            dePossessRequested = false;
            mediator.RegisterDepossessionRequest();
        }

        dirX = Input.GetAxisRaw("Horizontal_Ghost");
        dirY = Input.GetAxisRaw("Vertical_Ghost");
        rb.velocity = new Vector2(dirX * moveSpeed, dirY * moveSpeed);
        
        UpdateAnimationState();

    }

     [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong targetNetworkObjectId, ServerRpcParams rpcParams = default)
    {

        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectId, out NetworkObject targetNetworkObject))
        {
            targetNetworkObject.ChangeOwnership(rpcParams.Receive.SenderClientId);
        }
        else
        {
            Debug.LogWarning($"NetworkObject with ID {targetNetworkObjectId} not found.");
        }

    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f || dirY > 0f)
        {
            state = MovementState.moving;
            UpdateFlipX(true);
        }
        else if (dirX < 0f || dirY < 0f)
        {
            state = MovementState.moving;
            UpdateFlipX(false);
        }
        else
        {
            state = MovementState.idle;
        }

        anim.SetInteger("state", (int)state);
 
    }

     private void UpdateFlipX(bool flipX)
    {
        if (IsServer) 
        {
            isFlipped.Value = flipX;
        }
        else if (IsOwner) 
        {
            UpdateFlipXServerRpc(flipX);
        }
    }


    [ServerRpc]
    private void UpdateFlipXServerRpc(bool flipX)
    {
        isFlipped.Value = flipX;
    }


    [ServerRpc]
    private void ToggleSpriteRendererServerRpc(bool enabled)
    {
        isSpriteEnabled.Value = enabled;
    }
    public void ToggleIsPossessed(bool isPossessed)
    {
        if (IsServer)
        {
            isPossessedNetwork.Value = isPossessed;
            isSpriteEnabled.Value = !isPossessed;
        }
        else if (IsOwner)
        {
            ToggleIsPossessedServerRpc(isPossessed);
            ToggleSpriteRendererServerRpc(!isPossessed);
        }
    }

    [ServerRpc]
    private void ToggleIsPossessedServerRpc(bool isPossessed)
    {
        isSpriteEnabled.Value = isPossessed;
    }

    private PossessableTransformation GetClosestPossessableTarget()
    {
        PossessableTransformation closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var target in FindObjectsOfType<PossessableTransformation>())
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance < closestDistance && target.IsWithinTransformationRange(gameObject))
            {
                closestTarget = target;
                closestDistance = distance;
            }
        }
        return closestTarget;
    }

    private void RequestDepossession()
    {
        ToggleIsPossessed(false);
    }

}
