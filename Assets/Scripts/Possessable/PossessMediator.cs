using UnityEngine;

public class PossessMediator : MonoBehaviour
{
    private PossessableTransformation currentlyPossessedObject;
    private GameObject ghost;
    private PossessionTimer possessionTimer;

    public GameObject Ghost => ghost;

    private void Awake()
    {
        ghost = GameObject.FindGameObjectWithTag("Player_Ghost");
        possessionTimer = GetComponent<PossessionTimer>();
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
            ghost.GetComponent<GhostController>().IsPossessed = true;
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
            ghost.GetComponent<GhostController>().IsPossessed = false;

            currentlyPossessedObject = null;

            possessionTimer.StopTimer();
        }
    }

    public void UpdateTimerDisplay(float remainingTime)
    {
        possessionTimer.UpdateTimerDisplay(remainingTime);
    }

    public bool IsPossessing() => currentlyPossessedObject != null;
}
