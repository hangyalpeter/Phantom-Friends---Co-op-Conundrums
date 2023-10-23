using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    private bool isRunning = true;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

    [SerializeField] private GameObject endLevelPanel;
    Coroutine timerCoroutine;
    private Stopwatch stopwatch;

    void Start()
    {
        stopwatch = new Stopwatch();
        stopwatch.Start();

        timerCoroutine = StartCoroutine(UpdateTimer());
    }

    IEnumerator UpdateTimer()
    {
        while (isRunning)
        {
            long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
            int minutes = (int)(elapsedMilliseconds / 60000);
            int seconds = (int)((elapsedMilliseconds / 1000) % 60);
            int milliseconds = (int)(elapsedMilliseconds % 1000);
            timerText.text = string.Format("Time: {0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
            yield return null;
        }
    }

    public void StopTimer()
    {
        isRunning = false;
        stopwatch.Stop();
        StopCoroutine(timerCoroutine);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player_Child")
        {
            endLevelPanel.SetActive(true);
            StopTimer();
            Time.timeScale = 0f;
        }

    }
}
