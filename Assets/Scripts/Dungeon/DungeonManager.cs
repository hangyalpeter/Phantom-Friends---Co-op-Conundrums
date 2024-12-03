using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DungeonManager : NetworkBehaviour
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
        if (IsServer)
        {
            // TODO: uncomment below for the seed, 0 is for testing
            //int seed = 0;
            //TODO: set int maybe for restart or no?
            //int seed = PlayerPrefs.HasKey("DungeonSeed") ? PlayerPrefs.GetInt("DungeonSeed") : System.DateTime.Now.GetHashCode();
            int seed = System.DateTime.Now.GetHashCode();
            GenerateNewDungeonAtStartClientRpc(seed);
            Debug.Log("seed: " + seed);
        }
        dungeonGenerator.SpawnPlayerCharacters(false);
    }

    private void GenerateNewDungeonAfterWin()
    {
        if (!IsServer) return;
        var possessables = GameObject.FindGameObjectsWithTag("Possessable").Concat(GameObject.FindGameObjectsWithTag("BossHealthBar"));
        foreach (var possessable in possessables)
        {
            Destroy(possessable);
        }

        int seed = System.DateTime.Now.GetHashCode();
        rooms.Clear();
        rooms.UnionWith(dungeonGenerator.GenerateDungeon(true, seed));

        GenerateNewDungeonAfterWinClientRpc(seed);
    }
    [ClientRpc]
    private void GenerateNewDungeonAfterWinClientRpc(int seed)
    {
        UnityEngine.Random.InitState(seed);

        rooms.Clear();
        rooms.UnionWith(dungeonGenerator.GenerateDungeon(true, seed));

    }
    [ClientRpc]
    private void GenerateNewDungeonAtStartClientRpc(int seed)
    {
        UnityEngine.Random.InitState(seed);
        rooms.UnionWith(dungeonGenerator.GenerateDungeon(false, seed));
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
        HandlePlayerDeathClientRpc(roomsCleared, enemiesKilled);
    }

    [ClientRpc]
    private void HandlePlayerDeathClientRpc(int roomsCleared, int enemiesKilled)
    {
        OnDungeonFinish?.Invoke(roomsCleared, enemiesKilled, "Game Over", "Restart");
        UIScreenEvents.DungeonGameOverShown?.Invoke();
    }
    public void HandleDungeonWin()
    {
        Debug.Log("Player won the dungeon!");
        dungeonGenerator.useStoredSeed = false;
        //PlayerPrefs.DeleteKey("DungeonSeed");

        var roomsCleared = rooms.Where(x => x.isFinished).Count();
        //OnDungeonFinish?.Invoke(roomsCleared, enemiesKilled, "You won!", "New Dungeon");

        HandleDungeonWinClientRpc(roomsCleared, enemiesKilled);
        enemiesKilled = 0;

        //UIScreenEvents.DungeonGameOverShown?.Invoke();
        GameEvents.DungeonFinished?.Invoke();
    }

    [ClientRpc]
    private void HandleDungeonWinClientRpc(int roomsCleared, int enemiesKilled)
    {
        OnDungeonFinish?.Invoke(roomsCleared, enemiesKilled, "You won!", "New Dungeon");
        UIScreenEvents.DungeonGameOverShown?.Invoke();
    }

}