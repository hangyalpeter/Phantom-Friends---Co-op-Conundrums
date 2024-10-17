using Unity.Netcode;
using UnityEngine;

public class PossessMediator : NetworkBehaviour
{
    private PossessableTransformation currentlyPossessedObject;
    private PossessionTimer possessionTimer;

    public GameObject Ghost { get; private set; }

    private void Awake()
    {
        possessionTimer = GetComponent<PossessionTimer>();
        StartCoroutine(FindPlayers());
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
            currentlyPossessedObject.Possess();

            possessionTimer.StartTimer(target, target.PossessionDuration);
        }
    }

    public void RegisterDepossessionRequest()
    {
        if (currentlyPossessedObject != null)
        {
            currentlyPossessedObject.Depossess();

            var behavior = currentlyPossessedObject.GetComponent<PossessableBehavior>();
            if (behavior != null)
            {
                behavior.OnDePossess();
            }
            Ghost.GetComponent<GhostController>().ToggleIsPossessed(false);

            currentlyPossessedObject = null;

            possessionTimer.StopTimer();
        }
    }

    public void UpdateTimerDisplay(float remainingTime)
    {
        possessionTimer.UpdateTimerDisplay(remainingTime);
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
