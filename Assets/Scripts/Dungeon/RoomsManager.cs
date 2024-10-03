using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsManager : MonoBehaviour
{
    [SerializeField]
    private EnemyData[] enemyData = new EnemyData[3];

    private IDungeonMediator mediator;

    private HashSet<Room> rooms = new HashSet<Room>();
    private BinarySpacePartitioningDungeonGenerator dungeonGenerator;
    private Transform player;
    private EnemySpawner spawner;

    private bool isClosed = false;

    public void SetMediator(IDungeonMediator mediator)
    {
        this.mediator = mediator;
    }

    private void Start()
    {
        rooms = mediator.GetManager<DungeonManager>().Rooms;
        dungeonGenerator = mediator.GetManager<DungeonManager>().DungeonGenerator;
        spawner = GetComponent<EnemySpawner>();
        player = mediator.GetManager<PlayerManager>().Player;
        
    }

    private void Update()
    {
        /*if (rooms.Count == 0)
        {
            rooms = mediator.GetManager<DungeonManager>().Rooms;
        }*/
        if (player != null)
        {

            CheckPlayerEnteredRoom();
            foreach (var room in rooms)
            {
                var actualRooms = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
                var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);
                if (actualRooms.Contains(playerPosition) && isClosed)
                {
                    if (CheckIsRoomCleared(room))
                    {
                        room.isFinished = true;
                        isClosed = false;
                        OpenAllFinishedRooms();

                        if (CheckDungeonWin())
                        {
                            NotifyDungeonWin();
                        }
                    }
                }
            }

        }


    }

    private bool CheckDungeonWin()
    {
        return rooms.All(r => r.isFinished);
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
            else if (room.isBossRoom && rooms.Where(r => !r.isBossRoom).All(x => x.isFinished))
            {
                foreach (var door in room.doorTilesPositions)
                {
                    dungeonGenerator.TilemapVisualizer.PaintOpenGateTile(door, null);
                }
            }
        }



    }

    private void CheckPlayerEnteredRoom()
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
            else if  (rooms.Contains(playerPosition) && !room.isFinished && room.isBossRoom && !room.isVisited)
            {
                Debug.Log("Player entered boss room" + room.bounds.center);

                room.isVisited = true;

                StartCoroutine(DelayCloseBossRoom(room));

            }
        }

    }

    private IEnumerator DelayCloseCurrentRoom(Room room)
    {

        yield return new WaitForSeconds(1f);
        CloseCurrentRoom(room);
        SpawnEnemies(room);
        isClosed = true;
    }

    private IEnumerator DelayCloseBossRoom(Room room)
    {

        yield return new WaitForSeconds(1f);
        CloseCurrentRoom(room);
        SpawnBoss(room);
        isClosed = true;
    }
    private void SpawnEnemies(Room room)
    {
        foreach (var enemy in enemyData.Where(e => !e.isBoss))
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

    private void SpawnBoss(Room room)
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

        room.enemies.Add(spawner.SpawnEnemy(enemyData.Where(e => e.isBoss).First(), room.bounds.center));

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

        // TODO maybe check again if the player and ghost are in the room
        var currentPlayerRoomFloorPositions = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { currentPlayerRoom.bounds });
        var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);

        foreach (var door in currentPlayerRoom.doorTilesPositions)
        {
            dungeonGenerator.TilemapVisualizer.PaintDoorTile(door, Color.red);
        }


    }
    private bool CheckIsRoomCleared(Room room)
    {
        return !room.enemies.Any(e => e != null);
    }

    private void NotifyDungeonWin()
    {
        mediator.Notify(this, DungeonEvents.DungeonWin);
    }

}
