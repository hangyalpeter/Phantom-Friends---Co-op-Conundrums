using System;

public static class UIScreenEvents 
{

    public static Action ScreenClosed;
    public static Action MainMenuShown;
    public static Action SettingsShown;
    public static Action PauseShown;
    public static Action LevelFinishShown;
    public static Action PauseClosed;
    public static Action OnGameStart;
    public static Action OnDungeonGameStart;
    public static Action OnNextLevel;
    public static Action LevelSelectShown;
    public static Action DungeonGameOverShown;

    public static Action<string> OnLevelSelected;


    // Pause/LevelFinish Screen
    public static Action OnLevelRestart;
    public static Action MainMenuClicked;

  
}
