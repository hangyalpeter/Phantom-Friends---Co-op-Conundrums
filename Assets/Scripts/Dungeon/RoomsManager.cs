using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomsManager : MonoBehaviour
{
    [SerializeField]
    private List<EnemyData> roomEnemiesData = new List<EnemyData>();
    [SerializeField]
    private List<EnemyData> bossEnemiesData = new List<EnemyData>();

    private IDungeonMediator mediator;

    private HashSet<Room> rooms = new HashSet<Room>();
    private BinarySpacePartitioningDungeonGenerator dungeonGenerator;
    private Transform player;
    private Transform ghost;
    private EnemySpawner spawner;
    Room currentRoom;

    private bool isClosed = false;
    private bool isClosingRoom = false;

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
        ghost = mediator.GetManager<PlayerManager>().Ghost;
        currentRoom = rooms.First();
        if (!PlayerPrefs.HasKey("MakeHarder"))
        {
            PlayerPrefs.SetInt("MakeHarder", 0);
        }
        // if you finish a dungeon the next dungeon is harder,
        // but if you fail then it returns to being the initial difficulity
        // game design question, question of further development
        if (PlayerPrefs.GetInt("MakeHarder") == 1)
        {
            roomEnemiesData.Add(roomEnemiesData.ElementAt(1));
            PlayerPrefs.SetInt("MakeHarder", 0);
        }

    }

    private void Update()
    {
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
            var roomPositions = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);
            var ghostPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(ghost.transform.position);
            Debug.Log("playerpositon: " + playerPosition);

            if (roomPositions.Contains(playerPosition) && roomPositions.Contains(ghostPosition) && !room.isFinished && !room.isBossRoom && !room.isVisited)
            {
                Debug.Log("Player entered room" + room.bounds.center);

                if (!isClosingRoom)
                {
                    StartCoroutine(DelayCloseCurrentRoom(room));
                }
            }
            else if (roomPositions.Contains(playerPosition) && roomPositions.Contains(ghostPosition) && !room.isFinished && room.isBossRoom && !room.isVisited)
            {
                Debug.Log("Player entered boss room" + room.bounds.center);

                if (!isClosingRoom)
                {
                    StartCoroutine(DelayCloseBossRoom(room));
                }

            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                player.transform.position = currentRoom.bounds.center;
                ghost.transform.position = currentRoom.bounds.center;
            }
        }

    }

    private IEnumerator DelayCloseCurrentRoom(Room room)
    {

        isClosingRoom = true;
        yield return new WaitForSeconds(1f);

        isClosed = CloseCurrentRoom(room);
        if (isClosed && !room.spawned)
        {
            SpawnEnemies(room);
            room.isVisited = true;
            currentRoom = room;
        }
        isClosingRoom = false;
    }

    private IEnumerator DelayCloseBossRoom(Room room)
    {

        yield return new WaitForSeconds(1f);

        isClosed = CloseCurrentRoom(room);
        if (isClosed && !room.spawned)
        {
            SpawnBoss(room);
            room.isVisited = true;
            currentRoom = room;
        }
        isClosingRoom = false;
    }
    private void SpawnEnemies(Room room)
    {
        foreach (var enemy in roomEnemiesData)
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
                Vector3 position = potentialSwawnPositions.ElementAt(Random.Range(0, potentialSwawnPositions.Count()));

                room.enemies.Add(spawner.SpawnEnemy(enemy, position));

            }
        }

        room.spawned = true;

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
        Vector3 position = potentialSwawnPositions.ElementAt(Random.Range(0, potentialSwawnPositions.Count()));

        room.enemies.Add(spawner.SpawnEnemy(bossEnemiesData.First(), room.bounds.center));
        room.spawned = true;

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

    private bool CloseCurrentRoom(Room currentPlayerRoom)
    {

        var currentPlayerRoomFloorPositions = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { currentPlayerRoom.bounds });
        var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);
        var ghostPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(ghost.transform.position);


        List<Vector3Int> offsets = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),   // Center (original position)
            new Vector3Int(1, 0, 0),   // Right
            new Vector3Int(-1, 0, 0),  // Left
            new Vector3Int(0, 1, 0),   // Up
            new Vector3Int(0, -1, 0),  // Down
            new Vector3Int(1, 1, 0),   // Top-right diagonal
            new Vector3Int(-1, 1, 0),  // Top-left diagonal
            new Vector3Int(1, -1, 0),  // Bottom-right diagonal
            new Vector3Int(-1, -1, 0)  // Bottom-left diagonal
        };

        var validPositions = new HashSet<Vector3Int>();
        foreach (var item in currentPlayerRoom.floorTilesPositions)
        {
            if (!currentPlayerRoom.corridorTilePositions.Contains(item))
            {
                validPositions.Add(item);
            }
            
        }

        const int offsetRange = 2;
        var flag = true;

        foreach (var offset in offsets)
        {
            Vector3Int offsetPlayerPosition = playerPosition + offset * offsetRange;
            Vector3Int offsetGhostPosition = ghostPosition + offset * offsetRange;

            if (!validPositions.Contains(offsetPlayerPosition)
            || !validPositions.Contains(offsetGhostPosition)
            || currentPlayerRoom.doorTilesPositions.Contains(offsetPlayerPosition)
            || currentPlayerRoom.doorTilesPositions.Contains(offsetGhostPosition)
            || currentPlayerRoom.wallTilesPositions.Contains(offsetPlayerPosition)
            || currentPlayerRoom.wallTilesPositions.Contains(offsetGhostPosition)
            || currentPlayerRoom.corridorTilePositions.Contains(offsetPlayerPosition)
            || currentPlayerRoom.corridorTilePositions.Contains(offsetGhostPosition))
            {
                flag = false; break;
            }
        }

        if (flag)
        {
            foreach (var door in currentPlayerRoom.doorTilesPositions)
            {
                dungeonGenerator.TilemapVisualizer.PaintDoorTile(door, Color.red);
            }
        }
        return flag;
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
