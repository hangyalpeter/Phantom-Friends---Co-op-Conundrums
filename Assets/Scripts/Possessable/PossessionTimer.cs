﻿using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PossessionTimer : NetworkBehaviour
{
    private Coroutine timerCoroutine;
    private TextMeshProUGUI timerText;
    private PossessMediator mediator;

    private void OnEnable()
    {
        PossessableHealth.OnPossessedObjectDies += StopTimer;
    }
    private void OnDisable()
    {
        PossessableHealth.OnPossessedObjectDies -= StopTimer;
    }
    private void Start()
    {
        mediator = FindObjectOfType<PossessMediator>();
        timerText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<TextMeshProUGUI>();
    }

    public void StartTimer(float duration)
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        timerCoroutine = StartCoroutine(TimerCoroutine(duration));
    }

    public void StopTimer()
    {
        StopTimerServerRpc();
    }
    private void Stopti()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
            ResetTimerDisplay();
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
        Stopti();
    }

    private IEnumerator TimerCoroutine(float duration)
    {
        float remainingTime = duration;

        while (remainingTime > 0f)
        {
            UpdateTimerDisplay(remainingTime);
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }
        mediator.RegisterDepossessionRequest();

    }

    public void UpdateTimerDisplay(float remainingTime)
    {
        if (timerText != null)
        {
            timerText.text = "Remaining Possession Time: " + Mathf.Ceil(remainingTime).ToString();
        }
    }

    private void ResetTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = "";
        }
    }
}
