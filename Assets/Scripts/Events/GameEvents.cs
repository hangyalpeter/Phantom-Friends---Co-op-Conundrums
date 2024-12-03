using System;
public class GameEvents
{
    public static Action<string> GamePaused;
    public static Action GameResumed;
    public static Action OnLevelRestart;
    public static Action LevelFinished;
    public static Action<string, int> StarsChanged;
    public static Action BestTimesChanged;
    public static Action<string> LevelFinishedWithTime;
    public static Action DungeonFinished;
    public static Action OnNewDungeon;
}
