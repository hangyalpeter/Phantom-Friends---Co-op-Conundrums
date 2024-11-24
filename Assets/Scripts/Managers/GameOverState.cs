using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverState : IGameState
{
    private GameStateManager context;

    public GameOverState(GameStateManager context)
    {
        this.context = context;
    }
    public void EnterState()
    {
        Debug.Log("Entered Game Over State");
        Time.timeScale = 0f;

        int minutes = (int)(context.ElapsedTime / 60);
        int seconds = (int)(context.ElapsedTime % 60);
        //int milliseconds = (int)((context.ElapsedTime * 1000) % 1000);
        float hundreth = (float)((context.ElapsedTime - Mathf.Floor(context.ElapsedTime)) * 100);


        var currentLevelName = SceneManager.GetActiveScene().name;
        if (PlayerPrefs.HasKey(currentLevelName + "_BestCompletionTime"))
       {
            var bestCompletionTime = PlayerPrefs.GetFloat(currentLevelName + "_BestCompletionTime");
            if (context.ElapsedTime < bestCompletionTime)
            {
                PlayerPrefs.SetFloat(currentLevelName + "_BestCompletionTime", context.ElapsedTime);
                PlayerPrefs.Save();
                GameEvents.BestTimesChanged?.Invoke();
            }
        }
        else
        {
            PlayerPrefs.SetFloat(currentLevelName + "_BestCompletionTime", context.ElapsedTime);
            PlayerPrefs.Save();
            GameEvents.BestTimesChanged?.Invoke();
        }

        GameEvents.LevelFinishedWithTime?.Invoke(string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, hundreth));

    }

    public void UpdateState()
    {
        // intentionally empty
    }

    public void ExitState()
    {
        Debug.Log("Exiting Game Over State");
        Time.timeScale = 1f;
    }
}
