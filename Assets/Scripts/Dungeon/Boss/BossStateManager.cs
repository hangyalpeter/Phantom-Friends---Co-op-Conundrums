using System.Collections;
using UnityEngine;

public class BossStateManager : MonoBehaviour
{
    private BossController bossController;
    private IBossState initialState;
    private IBossState currentState;
    private void Start()
    {
        bossController = GetComponent<BossController>();
        initialState = new ShootInCircleState();

        currentState = initialState;

        bossController.TransitionToState(initialState);

        StartCoroutine(switchState());
    }

    private IEnumerator switchState()
    {
        while (currentState.GetNextState() != null)
        {
            yield return new WaitForSeconds(5);
            bossController.TransitionToState(currentState.GetNextState());
            currentState = currentState.GetNextState();
        }
    }
}

