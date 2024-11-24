using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PossessMediator : NetworkBehaviour
{
    private PossessableTransformation currentlyPossessedObject;
    private PossessionTimer possessionTimer;

    private GameObject Ghost;

    private void Awake()
    {
        possessionTimer = GetComponent<PossessionTimer>();
        StartCoroutine(FindPlayers());
    }

    private void Update()
    {
        if (IsPossessing())
        {
            Ghost.GetComponent<Rigidbody2D>().transform.position = currentlyPossessedObject.gameObject.transform.position;
        }
    }

    public void RegisterPossessionRequest(PossessableTransformation target)
    {
        if (currentlyPossessedObject == null)
        {
            currentlyPossessedObject = target;

            var behavior = currentlyPossessedObject.GetComponent<PossessableBehavior>();
            if (behavior != null)
            {
                behavior.OnPossess();
            }
            Ghost.GetComponent<GhostController>().ToggleIsPossessed(true);
            Ghost.GetComponent<Rigidbody2D>().transform.position = target.gameObject.transform.position;
            Ghost.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            target.GetComponent<PosessableMovement>().SetPossessedTrue();

            StartTimerServerRpc(target.PossessionDuration);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartTimerServerRpc(float duration)
    {
        StartTimerClientRpc(duration);
    }

    [ClientRpc]
    private void StartTimerClientRpc(float duration)
    {
        possessionTimer.StartTimer(duration);
    }

    public void RegisterDepossessionRequest()
    {
        if (currentlyPossessedObject != null)
        {
            currentlyPossessedObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            currentlyPossessedObject.GetComponent<PosessableMovement>().SetPossessedFalse();

            var behavior = currentlyPossessedObject.GetComponent<PossessableBehavior>();
            if (behavior != null)
            {
                behavior.OnDePossess();
            }
            Ghost.GetComponent<GhostController>().ToggleIsPossessed(false);

            currentlyPossessedObject = null;

            StopTimerServerRpc();
        }
    }

  [ServerRpc(RequireOwnership = false)]
    private void StopTimerServerRpc()
    {
        StopTimerClientRpc();
    }

    [ClientRpc]
    private void StopTimerClientRpc()
    {
        possessionTimer.StopTimer();
    }


    public bool IsPossessing() => currentlyPossessedObject != null;

    private System.Collections.IEnumerator FindPlayers()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer);

        while (Ghost == null)
        {
            GameObject ghost = GameObject.FindWithTag("Player_Ghost");
            if (ghost != null)
            {
                Ghost = ghost;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
  }
