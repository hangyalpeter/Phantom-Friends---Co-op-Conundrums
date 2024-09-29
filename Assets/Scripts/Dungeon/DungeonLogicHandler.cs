using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonLogicHandler : MonoBehaviour
{
    public static Action<int, int> OnPLayerDied;

    [SerializeField]
    private GameObject player;

    private HealthComponent playerHealthComponent;

    [SerializeField]
    private EnemyData[] enemyData = new EnemyData[3];

    [SerializeField]
    private BinarySpacePartitioningDungeonGenerator dungeonGenerator;
    private HashSet<Room> rooms = new HashSet<Room>();

    private EnemySpawner spawner;

    private int enemiesKilled = 0;

    void Start()
    {
        rooms = dungeonGenerator.GenerateDungeon();
        spawner = gameObject.GetComponent<EnemySpawner>();
        playerHealthComponent = player.GetComponent<HealthComponent>();
        playerHealthComponent.OnDied += HandlePlayerDeath;
        enemiesKilled = 0;
        HealthComponent.OnEnemyDied += UpdateEnemyDiedCount;
    }

    private void OnDisable()
    {
        playerHealthComponent.OnDied -= HandlePlayerDeath;
        HealthComponent.OnEnemyDied += UpdateEnemyDiedCount;
    }
    
    private void UpdateEnemyDiedCount(string name)
    {
        enemiesKilled += 1;
        Debug.Log("Enemies killed " + enemiesKilled + " " +  name);
    }

    private void Update()
    {
        if (player != null)
        {

            CheckIfPlayerEnteredRoom();
            foreach (var room in rooms)
            {
                var actualRooms = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
                var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);
                if (actualRooms.Contains(playerPosition))
                {
                    if (CheckIsRoomCleared(room))
                    {
                        OpenAllFinishedRooms();
                        room.isFinished = true;
                    }
                }
            }
        }

    }

    private bool CheckIsRoomCleared(Room room)
    {
       return !room.enemies.Any(e => e != null);
    }


    private void CheckIfPlayerEnteredRoom()
    {
        foreach (var room in rooms)
        {
            var rooms = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);

            if (rooms.Contains(playerPosition) && !room.isFinished && !room.isBossRoom && !room.isVisited)
            {
                Debug.Log("Player entered room" + room.bounds.center);


                room.isVisited = true;
                StartCoroutine(DelayCloseCurrentRoom(room));

            }
        }
    }

    private IEnumerator DelayCloseCurrentRoom(Room room)
    {

        yield return new WaitForSeconds(1f);
        CloseCurrentRoom(room);
        SpawnEnemies(room);
    }

    private void SpawnEnemies(Room room)
    {
        foreach (var enemy in enemyData)
        {
            if (enemy != null)
            {
                var neighborOffsets = new List<Vector3Int>{
                    Vector3Int.right,
                    Vector3Int.left,
                    Vector3Int.up,
                    Vector3Int.down
                };
                HashSet<Vector3Int> wallAndNeighborPositions = GetWallNeighborPositions(room, neighborOffsets);

                IEnumerable<Vector3Int> potentialSwawnPositions = CalculatePotentialSpawnPositions(room, wallAndNeighborPositions);
                Vector3 position = potentialSwawnPositions.ElementAt(UnityEngine.Random.Range(0, potentialSwawnPositions.Count()));

                room.enemies.Add(spawner.SpawnEnemy(enemy, position));

            }
        }

    }

    private IEnumerable<Vector3Int> CalculatePotentialSpawnPositions(Room room, HashSet<Vector3Int> wallAndNeighborPositions)
    {
        return room.floorTilesPositions
            .Where(p => Vector3.Distance(player.transform.position, p) > 0.5f &&
         !room.doorTilesPositions.Contains(p) &&
         !wallAndNeighborPositions.Contains(p)
         );
    }

    private static HashSet<Vector3Int> GetWallNeighborPositions(Room room, List<Vector3Int> neighborOffsets)
    {
        return room.wallTilesPositions
                            .SelectMany(wallPos => neighborOffsets.Select(offset => wallPos + offset))
                            .Concat(room.wallTilesPositions)
                            .ToHashSet();
    }

    private void CloseCurrentRoom(Room currentPlayerRoom)
    {

        // TODO maybe check again if the player is in the room
        var currentPlayerRoomFloorPositions = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { currentPlayerRoom.bounds });
        var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);

        foreach (var door in currentPlayerRoom.doorTilesPositions)
        {
            dungeonGenerator.TilemapVisualizer.PaintDoorTile(door, Color.red);
        }


    }

    private void OpenAllFinishedRooms()
    {

        foreach (var room in rooms)
        {
            if (!room.isBossRoom)
            {
                foreach (var door in room.doorTilesPositions)
                {
                    dungeonGenerator.TilemapVisualizer.PaintOpenGateTile(door, null);
                }
            }
            else if (!rooms.Any(x => !x.isFinished && !x.isBossRoom))
            {
                foreach (var door in room.doorTilesPositions)
                {
                    dungeonGenerator.TilemapVisualizer.PaintOpenGateTile(door, null);
                }
            }
        }
    }

    private void HandlePlayerDeath()
    {
        var roomsCleared = rooms.Where(x => x.isFinished).Count();
        OnPLayerDied?.Invoke(roomsCleared-1, enemiesKilled);
        GameEvents.DungeonFinished?.Invoke();
        UIScreenEvents.DungeonGameOverShown?.Invoke();
        Time.timeScale = 0;
    }

}
