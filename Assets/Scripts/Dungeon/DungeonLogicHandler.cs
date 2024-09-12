using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonLogicHandler : MonoBehaviour
{
    [SerializeField]
    private Transform player;


    [SerializeField]
    private EnemyData[] enemyData = new EnemyData[3];

    [SerializeField]
    private BinarySpacePartitioningDungeonGenerator dungeonGenerator;
    private HashSet<Room> rooms = new HashSet<Room>();

    private EnemySpawner spawner;

    void Start()
    {
        rooms = dungeonGenerator.GenerateDungeon();
        spawner = gameObject.GetComponent<EnemySpawner>();
    }

    private void Update()
    {
        CheckIfPlayerEnteredRoom();
        foreach (var room in rooms)
        {
            var actualRooms = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.position);
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

    private bool CheckIsRoomCleared(Room room)
    {
       return !room.enemies.Any(e => e != null);
    }


    private void CheckIfPlayerEnteredRoom()
    {
        foreach (var room in rooms)
        {
            var rooms = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.position);

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
                //TODO refactor to methods

                var neighborOffsets = new List<Vector3Int>{
                    new Vector3Int(1, 0, 0),  // Right neighbor
                    new Vector3Int(-1, 0, 0), // Left neighbor
                     new Vector3Int(0, 1, 0),  // Up neighbor
                    new Vector3Int(0, -1, 0)  // Down neighbor}; 
                };
                // Get all neighboring positions for each wall tile
                var wallAndNeighborPositions = room.wallTilesPositions
                    .SelectMany(wallPos => neighborOffsets.Select(offset => wallPos + offset))
                    .Concat(room.wallTilesPositions)  // Include the wall tiles themselves
                    .ToHashSet(); // Use a HashSet for fast lookups

                var potentialSwawnPositions = room.floorTilesPositions
                    .Where(p => Vector3.Distance(player.transform.position, p) > 0.5f &&
                 !room.doorTilesPositions.Contains(p) &&
                 !wallAndNeighborPositions.Contains(p)
                 );
                Vector3 position = potentialSwawnPositions.ElementAt(Random.Range(0, potentialSwawnPositions.Count()));

                room.enemies.Add(spawner.SpawnEnemy(enemy, position));
                
            }
        }

    }


    private void CloseCurrentRoom(Room currentPlayerRoom)
    {

        // TODO maybe check again if the player is in the room
        var currentPlayerRoomFloorPositions = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { currentPlayerRoom.bounds });
        var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.position);

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

}
