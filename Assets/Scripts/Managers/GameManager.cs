using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private bool isLevelPlaying = false;
    private bool isPaused = false;
    float elapsedTime = 0f;

    public float ElapsedTime => elapsedTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        elapsedTime = 0f;
        isLevelPlaying = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isLevelPlaying)
        {
            if (isPaused)
            {
                UIScreenEvents.ScreenClosed?.Invoke();
            }
            else
            {
                UIScreenEvents.PauseShown?.Invoke();
            }
        }
        if (!isPaused && isLevelPlaying)
        {
            elapsedTime += Time.deltaTime;
        }

    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        UIScreenEvents.OnGameStart += OnGameStartClicked;
        UIScreenEvents.OnLevelRestart += OnLevelRestartClicked;
        UIScreenEvents.MainMenuClicked += OnMainMenuClicked;
        UIScreenEvents.PauseClosed += PauseClosed;
        UIScreenEvents.PauseShown += PauseShown;

        GameEvents.OnLevelRestart += OnLevelRestartClicked;
    }

    private void UnsubscribeFromEvents()
    {
        UIScreenEvents.OnGameStart -= OnGameStartClicked;
        UIScreenEvents.OnLevelRestart -= OnLevelRestartClicked;
        UIScreenEvents.MainMenuClicked -= OnMainMenuClicked;
        UIScreenEvents.PauseClosed -= PauseClosed;
        UIScreenEvents.PauseShown -= PauseShown;

        GameEvents.OnLevelRestart -= OnLevelRestartClicked;

    }

    private void PauseShown()
    {
        Time.timeScale = 0f;
        isPaused = true;
        int minutes = (int)(elapsedTime / 60);
        int seconds = (int)(elapsedTime % 60);
        int milliseconds = (int)((elapsedTime * 1000) % 1000);

        GameEvents.GamePaused?.Invoke(string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds));
    }

    private void PauseClosed()
    {
        Time.timeScale = 1f;
        isPaused = false;
    }

    private IEnumerator LoadFirstLevel()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Level 1");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Time.timeScale = 1f;
        isLevelPlaying = true;
        elapsedTime = 0f;


    }

    private void OnGameStartClicked()
    {
        StartCoroutine(LoadFirstLevel());
    }

    private void OnLevelRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UIScreenEvents.ScreenClosed?.Invoke();
        isLevelPlaying = true;
        elapsedTime = 0f;
    }

    private IEnumerator LoadMainMenu()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main Menu");

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        UIScreenEvents.ScreenClosed?.Invoke();
        UIScreenEvents.MainMenuShown?.Invoke();
        elapsedTime = 0f;
        isLevelPlaying = false;

    }

    private void OnMainMenuClicked()
    {

        StartCoroutine(LoadMainMenu());
    }
}
