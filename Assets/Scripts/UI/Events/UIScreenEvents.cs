using System;

public static class UIScreenEvents 
{

    public static Action HideAllScreens;
    public static Action OnClientReady;
    public static Action<Scene> OnHostReady;
    public static Action<Scene> OnHostStart;
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
    public static Action WaitingForPlayersScreenShown;
    public static Action OnBackToTitleScreen;
    public static Action Unready;
    public static Action<string> DisconnectMessageShown;


    // Pause/LevelFinish Screen
    public static Action OnLevelRestart;
    public static Action MainMenuClicked;

  
}
