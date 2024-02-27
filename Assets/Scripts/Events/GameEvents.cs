using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents 
{
    public static Action<string> GamePaused;
    public static Action GameResumed;
    public static Action OnLevelRestart;
}
