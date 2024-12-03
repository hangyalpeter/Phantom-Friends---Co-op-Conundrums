using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RoomsManager : NetworkBehaviour
{
    [SerializeField]
    private List<EnemyData> roomEnemiesData = new List<EnemyData>();
    private List<EnemyData> clonedEnemiesData = new List<EnemyData>();

    private int numberOfEnemiesToGenerate = 2;

    [SerializeField]
    private List<EnemyData> bossEnemiesData = new List<EnemyData>();
    private List<EnemyData> clonedBossEnemiesData = new List<EnemyData>();

    [SerializeField]
    private List<GameObject> possessableObjects = new List<GameObject>();

    private IDungeonMediator mediator;

    private HashSet<Room> rooms = new HashSet<Room>();
    private BinarySpacePartitioningDungeonGenerator dungeonGenerator;

    private Transform player;
    private Transform ghost;

    private EnemySpawner spawner;
    private Room currentRoom;

    private bool isClosed = false;
    private bool isClosingRoom = false;

    public void SetMediator(IDungeonMediator mediator)
    {
        this.mediator = mediator;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(FindObjects());
    }

    private void Start()
    {
        StartCoroutine(FindObjects());

        foreach (var originalEnemy in roomEnemiesData)
        {
            EnemyData clonedEnemy = Instantiate(originalEnemy);
            clonedEnemiesData.Add(clonedEnemy);
        }
        foreach (var originalBoss in bossEnemiesData)
        {
            EnemyData clonedBoss = Instantiate(originalBoss);
            clonedBossEnemiesData.Add(clonedBoss);
        }
    }

    private IEnumerator FindObjects()
    {
        while (GameObject.FindGameObjectWithTag("Player_Child") == null || GameObject.FindGameObjectWithTag("Player_Ghost") == null)
        {
            yield return null;
        }

        rooms = mediator.GetManager<DungeonManager>().Rooms;
        dungeonGenerator = mediator.GetManager<DungeonManager>().DungeonGenerator;
        spawner = GetComponent<EnemySpawner>();
        player = mediator.GetManager<PlayerManager>().Player;
        ghost = mediator.GetManager<PlayerManager>().Ghost;
        currentRoom = rooms.First();
    }

    private void Update()
    {
        if (!IsServer) return;

        if (player != null && ghost != null)
        {
            CheckPlayerEnteredRoom();
            var currentActualRoompos = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { currentRoom.bounds });
            var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);
            if (currentActualRoompos.Contains(playerPosition) && isClosed)
            {
                if (CheckIsRoomCleared(currentRoom))
                {
                    currentRoom.isFinished = true;
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
                OpenRoomClientRpc(room.bounds.center);
            }
            else if (room.isBossRoom && rooms.Where(r => !r.isBossRoom).All(x => x.isFinished))
            {
                OpenRoomClientRpc(room.bounds.center);
            }
        }

    }

    private void OpenRoom(Vector3 roomCenter)
    {
        var room = rooms.First(r => r.bounds.center == roomCenter);
        foreach (var door in room.doorTilesPositions)
        {
            dungeonGenerator.TilemapVisualizer.PaintOpenGateTile(door, null);
        }
    }

    [ClientRpc]
    private void OpenRoomClientRpc(Vector3 center)
    {
        OpenRoom(center);
    }

    private void CheckPlayerEnteredRoom()
    {
        foreach (var room in rooms)
        {
            var roomPositions = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.transform.position);
            var ghostPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(ghost.transform.position);

            if (roomPositions.Contains(playerPosition) && roomPositions.Contains(ghostPosition) && !room.isFinished && !room.isBossRoom && !room.isVisited)
            {
                if (!isClosingRoom)
                {
                    StartCoroutine(DelayCloseCurrentRoom(room));
                }
            }
            else if (roomPositions.Contains(playerPosition) && roomPositions.Contains(ghostPosition) && !room.isFinished && room.isBossRoom && !room.isVisited)
            {
                if (!isClosingRoom)
                {
                    StartCoroutine(DelayCloseBossRoom(room));
                }

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
        if (!IsServer) return;
        var neighborOffsets = new List<Vector3Int>{
                    Vector3Int.right,
                    Vector3Int.left,
                    Vector3Int.up,
                    Vector3Int.down
                };
        HashSet<Vector3Int> wallAndNeighborPositions = GetWallNeighborPositions(room, neighborOffsets);

        IEnumerable<Vector3Int> potentialSwawnPositions = CalculatePotentialSpawnPositions(room, wallAndNeighborPositions);

        int i = 0;
        while (i < numberOfEnemiesToGenerate)
        {
            var randomEnemy = clonedEnemiesData[UnityEngine.Random.Range(0, clonedEnemiesData.Count)];
            Vector3 position = potentialSwawnPositions.ElementAt(UnityEngine.Random.Range(0, potentialSwawnPositions.Count()));
            room.enemies.Add(spawner.SpawnEnemy(randomEnemy, position));
            i++;
        }

        foreach (var possessable in possessableObjects)
        {
            Vector3 position = potentialSwawnPositions.ElementAt(UnityEngine.Random.Range(0, potentialSwawnPositions.Count()));
            var p = Instantiate(possessable, position, Quaternion.identity);
            NetworkObject n = p.GetComponent<NetworkObject>();
            n.Spawn(destroyWithScene: true);
        }

        room.spawned = true;
    }

    private void SpawnBoss(Room room)
    {
        if (!IsServer) return;
        var neighborOffsets = new List<Vector3Int>{
                    Vector3Int.right,
                    Vector3Int.left,
                    Vector3Int.up,
                    Vector3Int.down
                };
        HashSet<Vector3Int> wallAndNeighborPositions = GetWallNeighborPositions(room, neighborOffsets);

        IEnumerable<Vector3Int> potentialSwawnPositions = CalculatePotentialSpawnPositions(room, wallAndNeighborPositions);
        Vector3 spawnPosition = potentialSwawnPositions.ElementAt(UnityEngine.Random.Range(0, potentialSwawnPositions.Count()));

        foreach (var possessable in possessableObjects)
        {
            Vector3 position = potentialSwawnPositions.ElementAt(UnityEngine.Random.Range(0, potentialSwawnPositions.Count()));
            var p = Instantiate(possessable, position, Quaternion.identity);
            NetworkObject n = p.GetComponent<NetworkObject>();
            n.Spawn(destroyWithScene: true);
        }

        room.enemies.Add(spawner.SpawnEnemy(clonedBossEnemiesData.First(), room.bounds.center));
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
            CloseRoomWithDoorsClientRpc(currentPlayerRoom.bounds.center);
        }
        return flag;
    }

    [ClientRpc]
    private void CloseRoomWithDoorsClientRpc(Vector3 currentPlayerRoomCenter)
    {
        CloseRoomWithDoors(currentPlayerRoomCenter);
    }
    private void CloseRoomWithDoors(Vector3 currentPlayerRoomCenter)
    {
        var room = rooms.First(r => r.bounds.center == currentPlayerRoomCenter);
        foreach (var door in room.doorTilesPositions)
        {
            dungeonGenerator.TilemapVisualizer.PaintDoorTile(door, Color.red);
        }
    }

    private bool CheckIsRoomCleared(Room room)
    {
        return room.enemies.All(e =>
        {
            if (e != null)
            {
                return e.GetComponent<HealthBase>().CurrentHealth <= 0;
            }
            else
            {
                return true;
            }
        });
    }

    private void NotifyDungeonWin()
    {
        mediator.Notify(this, DungeonEvents.DungeonWin);

        clonedBossEnemiesData.ForEach(b =>
        {
            b.health = b.health * 1.2f;
            b.damage = b.damage * 1.05f;
        });
        clonedEnemiesData.ForEach(e =>
        {
            e.health = e.health * 1.2f;
            e.damage = e.damage * 1.05f;
        });
        numberOfEnemiesToGenerate++;
        DungeonWinClientRpc();

    }

    [ClientRpc]
    private void DungeonWinClientRpc()
    {
        Destroy(GameObject.FindGameObjectWithTag("BossHealthBar"));
    }
}
