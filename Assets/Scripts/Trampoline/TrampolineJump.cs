using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TrampolineJump : NetworkBehaviour
{
    private Animator anim;
    [SerializeField] private float jumpForce = 20f;
    [SerializeField] private AudioSource trampolineSound;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player_Child"))
        {
            RequestTrampolineActionServerRpc(collision.gameObject.GetComponent<NetworkObject>());
        }
    }
 
    [ServerRpc(RequireOwnership = false)]
    private void RequestTrampolineActionServerRpc(NetworkObjectReference player)
    {
        JumpPlayerClientRpc(player);
    }

    [ClientRpc]
    private void JumpPlayerClientRpc(NetworkObjectReference player)
    {
        player.TryGet(out NetworkObject playerObject);
        if (playerObject != null)
        {
            trampolineSound.Play();
            anim.SetTrigger("jump");
            playerObject.GetComponent<Rigidbody2D>().velocity = new Vector2(playerObject.GetComponent<Rigidbody2D>().velocity.x, jumpForce);
        }

    }
}
