using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PossessableTransformation : MonoBehaviour
{
    [SerializeField] private GameObject ghost;
    [SerializeField] private float distanceOffset = 2f;
    [SerializeField] private float possessionDuration = 15f;
    [SerializeField] private TextMeshProUGUI timerText;

    private GameObject timerGO;

    private Rigidbody2D rbGhost;
    private Rigidbody2D rb;
    private HealthComponent health;

    public UnityEvent OnPossess;
    public UnityEvent OnDePossess;

    public static event Action OnPossessEvent;
    public static event Action OnDePossessEvent;

    private bool isPossessed = false;
    private Coroutine possessionTimer;

    void Start()
    {
        ghost = GameObject.FindGameObjectWithTag("Player_Ghost");
        rbGhost = ghost.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<HealthComponent>();
        timerText = GameObject.FindGameObjectWithTag("TimerText").GetComponent<TextMeshProUGUI>();
        timerText.text = "";

        health.OnDied += UnPossess;
    }

    private void OnDisable()
    {
        health.OnDied -= UnPossess;
    }

    private void Update()
    {

        if (isPossessed && Input.GetKeyDown(KeyCode.R) && rbGhost.GetComponent<GhostController>().IsPossessed )
        {
            UnPossess();
        }

        if (isWithinTransformationRange() && !isPossessed && Input.GetKeyDown(KeyCode.E) && !rbGhost.GetComponent<GhostController>().IsPossessed )
        {
            Possess();
        }

    }

    private void StartPossessionTimer()
    {
        possessionTimer = StartCoroutine(PossessionTimerCoroutine());
    }

    private IEnumerator PossessionTimerCoroutine()
    {
        float currentTime = possessionDuration;

        while (currentTime > 0f)
        {
            UpdateTimerDisplay(currentTime);
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
        }

        UnPossess();
    }
    private void UpdateTimerDisplay(float remainingTime)
    {
        if (timerText != null)
        {
           timerText.text = " Remaining Posession Time: " + Mathf.Ceil(remainingTime).ToString();
        }
    }
    private void UnPossess()
    {
        isPossessed = false;
        rbGhost.GetComponent<SpriteRenderer>().enabled = true;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        OnDePossess?.Invoke();

        if (possessionTimer != null)
        {
            StopCoroutine(possessionTimer);
            possessionTimer = null;
        }

        timerText.text = "";
        OnDePossessEvent?.Invoke();
    }

    private bool isWithinTransformationRange() 
    {
        return Vector2.Distance(transform.position, ghost.transform.position) < distanceOffset;
    }


    public void Possess()
    {
        if (isPossessed)
        {
            return;
        }

        isPossessed = true;
        
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rbGhost.velocity = Vector2.zero;
        //rbGhost.angularVelocity = 0f;
        rbGhost.transform.position = transform.position;
        rbGhost.transform.rotation = transform.rotation;
        rbGhost.GetComponent<SpriteRenderer>().enabled = false;

        StartPossessionTimer();
        OnPossess?.Invoke();
        OnPossessEvent?.Invoke();
    }
  
}
