using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    private IDungeonMediator mediator;

    [SerializeField]
    private BinarySpacePartitioningDungeonGenerator dungeonGenerator;

    private HashSet<Room> rooms = new HashSet<Room>();

    private int enemiesKilled = 0;

    public static Action<int, int, string, string> OnDungeonFinish;

    public HashSet<Room> Rooms { get { return rooms; } }

    public BinarySpacePartitioningDungeonGenerator DungeonGenerator => dungeonGenerator;

    private void OnEnable()
    {
        GameEvents.OnNewDungeon += GenerateNewDungeonAfterWin;
    }

    private void OnDisable()
    {
        GameEvents.OnNewDungeon -= GenerateNewDungeonAfterWin;
    }

    public void SetMediator(IDungeonMediator mediator)
    {
        this.mediator = mediator;
    }

    private void Start()
    {
        rooms = dungeonGenerator.GenerateDungeon();
    }

    private void GenerateNewDungeonAfterWin()
    {
        var possessables = GameObject.FindGameObjectsWithTag("Possessable").Concat(GameObject.FindGameObjectsWithTag("BossHealthBar"));
        foreach (var possessable in possessables)
        {
            Destroy(possessable);
        }
        rooms.Clear();
        rooms.UnionWith(dungeonGenerator.GenerateDungeon());
    }

    public void IncrementKilledEnemies()
    {
        enemiesKilled++;
    }

    public void HandlePlayerDeath()
    {
        var roomsCleared = rooms.Where(x => x.isFinished).Count();
        OnDungeonFinish?.Invoke(roomsCleared, enemiesKilled, "Game Over", "Restart");
        UIScreenEvents.DungeonGameOverShown?.Invoke();
        GameEvents.DungeonFinished?.Invoke();
    }
    public void HandleDungeonWin()
    {
        Debug.Log("Player won the dungeon!");
        dungeonGenerator.useStoredSeed = false;
        PlayerPrefs.DeleteKey("DungeonSeed");

        var roomsCleared = rooms.Where(x => x.isFinished).Count();
        OnDungeonFinish?.Invoke(roomsCleared, enemiesKilled, "You won!", "New Dungeon");

        enemiesKilled = 0;

        UIScreenEvents.DungeonGameOverShown?.Invoke();
        GameEvents.DungeonFinished?.Invoke();
    }

}