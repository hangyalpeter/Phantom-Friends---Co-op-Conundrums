using Assets.Scripts.Dungeon;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class BinarySpacePartitioningDungeonGenerator : DungeonGeneratorStrategy
{
    [SerializeField]
    private int minWidth = 20;

    [SerializeField]
    private int minHeight = 20;

    [SerializeField]
    private int dungeonWidth = 80;
    [SerializeField]
    private int dungeonHeight = 80;
    [SerializeField]
    private int roomWidthOffset = 1;

    [SerializeField]
    private TilemapVisualizer tilemapVisualizer;

    [SerializeField]
    private Transform player;
    [SerializeField]
    private Transform ghost;

    private HashSet<Room> rooms = new HashSet<Room>();
    private HashSet<Vector3Int> corridorPositions = new HashSet<Vector3Int>();
    private readonly HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>();

    private Dictionary<Vector3Int, List<Vector3Int>> roomGraph = new Dictionary<Vector3Int, List<Vector3Int>>();
    public TilemapVisualizer TilemapVisualizer => tilemapVisualizer;

    public bool useStoredSeed = true;
    private int seed;


    private Vector3Int firstRoomCenter;
    private Vector3Int bossRoomCenter = Vector3Int.zero;

    [SerializeField]
    private int minimumRoomsCount = 4;
    [SerializeField]
    private GameObject playerGhostPrefab;
    [SerializeField]
    private GameObject playerChildPrefab;

    private void Awake()
    {
        if (useStoredSeed)
        {
            if (PlayerPrefs.HasKey("DungeonSeed"))
            {
                seed = PlayerPrefs.GetInt("DungeonSeed");
            }
            else
            {
                seed = System.DateTime.Now.GetHashCode();
                PlayerPrefs.SetInt("DungeonSeed", seed);
            }
            Random.InitState(seed);
        }

    }

    public override void RunProceduralGeneration()
    {
        GenerateDungeon(false, PlayerPrefs.GetInt("DungeonSeed"));
    }

    public HashSet<Room> GenerateDungeon(bool alreadySpawned, int seed)
    {
        InitializeMap();
        while (rooms.Count < minimumRoomsCount)
        {
            CreateRooms();
        }

        if (IsClient && !IsServer)
        {
            RequestSpawnPlayerServerRpc(alreadySpawned);
        }

        if (IsServer)
        {
            SpawnPlayer(NetworkManager.Singleton.LocalClientId, alreadySpawned);

            //Couch-coop TODO: get it from a singleton if we should make it couch-coop or not, maybe sessionmanager or something like that could store it
            //SpawnLocalPlayersForCouchCoop();
        }
        PrintRoomGraph();
        PrintGraph(roomGraph);

        return rooms;
    }

    private void CreateRooms()
    {
        InitializeMap();
        rooms = ProceduralGenerationUtilityAlgorithms.BinarySpacePartitioning(new BoundsInt(Vector3Int.zero, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minWidth, minHeight);

        GenerateRoomPositions();

        ConnectRoomsWithCorridors();

        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer, rooms);

        PlaceDoors();

    }

    private void ConnectRoomsWithCorridors()
    {

        corridorPositions = ConnectRooms();
        rooms.First().corridorTilePositions = corridorPositions;


        RemoveCorridorTilesFromRoomTiles();

        //this is needed to connect the corridors to the rooms
        floorPositions.UnionWith(corridorPositions);


        tilemapVisualizer.PaintFloorTiles(floorPositions, null);
    }

    private void RemoveCorridorTilesFromRoomTiles()
    {
        var corridorPositionsCopy = new HashSet<Vector3Int>(corridorPositions);

        foreach (var corridor in corridorPositions)
        {
            if (!floorPositions.Contains(corridor))
            {
                tilemapVisualizer.PaintFloorTiles(new HashSet<Vector3Int> { corridor }, Color.black);
                //tilemapVisualizer.PaintFloorTiles(new HashSet<Vector3Int> { corridor }, null);
            }
            else
            {
                corridorPositionsCopy.Remove(corridor);
            }
        }

        corridorPositions = corridorPositionsCopy;
    }

    private void GenerateRoomPositions()
    {
        foreach (var room in rooms)
        {
            var color = GenerateRandomColor();

            tilemapVisualizer.PaintFloorTiles(GetActualRoomFloorPositions(new List<BoundsInt> { room.bounds }), color);
            //tilemapVisualizer.PaintFloorTiles(GetActualRoomFloorPositions(new List<BoundsInt> { room.Value }), null);

            floorPositions.UnionWith(GetActualRoomFloorPositions(new List<BoundsInt> { room.bounds }));

            room.floorTilesPositions.UnionWith(GetActualRoomFloorPositions(new List<BoundsInt> { room.bounds }));
        }
    }

    private void InitializeMap()
    {
        tilemapVisualizer.Clear();
        rooms.Clear();
        floorPositions.Clear();
        corridorPositions.Clear();
        firstRoomCenter = Vector3Int.zero;
        roomGraph = new Dictionary<Vector3Int, List<Vector3Int>>();
    }

    private Color? GenerateRandomColor()
    {
        List<Color> colors = new List<Color>
        {
            Color.red,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            Color.cyan
        };
        return colors[Random.Range(0, colors.Count)];
    }

    private HashSet<Vector3Int> ConnectRooms()
    {
        bossRoomCenter = Vector3Int.zero;
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();

        var roomCenters = rooms.Select(x => Vector3Int.RoundToInt(x.bounds.center)).ToList();

        var current = roomCenters[0];
        firstRoomCenter = roomCenters[0];
        roomCenters.Remove(current);

        // Initialize graph with room centers
        foreach (var roomCenter in rooms.Select(x => Vector3Int.RoundToInt(x.bounds.center)))
        {
            roomGraph[roomCenter] = new List<Vector3Int>();
        }

        while (roomCenters.Count > 0)
        {
            var closest = FindClosestRoom(current, roomCenters);
            roomCenters.Remove(closest);

            HashSet<Vector3Int> newCorridor = CreateCorridor(current, closest);

            roomGraph[current].Add(closest);
            roomGraph[closest].Add(current);

            current = closest;
            corridors.UnionWith(newCorridor);
        }
        if (rooms.Count >= 3)
        {
            MarkBossRoom();
        }
        else
        {
            rooms.FirstOrDefault(room => room.floorTilesPositions.Contains(current)).isBossRoom = true;
        }


        return corridors;
    }

    private void MarkBossRoom()
    {
        var bossRoomCenter = GetNonCutVertices().Last();
        if (bossRoomCenter == firstRoomCenter)
        {
            firstRoomCenter = Vector3Int.RoundToInt(rooms.First(r => r.bounds.center != bossRoomCenter).bounds.center);
        }
        rooms.FirstOrDefault(room => room.floorTilesPositions.Contains(bossRoomCenter)).isBossRoom = true;
    }

    void DFS(Dictionary<Vector3Int, List<Vector3Int>> graph, Vector3Int current, HashSet<Vector3Int> visited)
    {
        visited.Add(current);
        foreach (var neighbor in graph[current])
        {
            if (!visited.Contains(neighbor))
            {
                DFS(graph, neighbor, visited);
            }
        }
    }

    void RemoveVertex(Dictionary<Vector3Int, List<Vector3Int>> graph, Vector3Int vertex)
    {
        graph.Remove(vertex);
        foreach (var neighbours in graph.Values)
        {
            neighbours.Remove(vertex);
        }
    }

    Dictionary<Vector3Int, List<Vector3Int>> CloneGraph(Dictionary<Vector3Int, List<Vector3Int>> originalGraph)
    {
        var clone = new Dictionary<Vector3Int, List<Vector3Int>>();
        foreach (var kvp in originalGraph)
        {
            clone[kvp.Key] = new List<Vector3Int>(kvp.Value);
        }
        return clone;
    }

    private List<Vector3Int> GetNonCutVertices()
    {
        List<Vector3Int> nonCutVertices = new List<Vector3Int>();
        foreach (var vertex in roomGraph.Keys)
        {
            Dictionary<Vector3Int, List<Vector3Int>> modifiedGraph = CloneGraph(roomGraph);
            modifiedGraph = GetRemainingGraphFromCuttingVertex(modifiedGraph, vertex);

            Vector3Int startVertex = roomGraph.Keys.First(v => v != vertex); // Pick a vertex that isn't the one we're testing
            HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

            DFS(modifiedGraph, startVertex, visited);

            // If the number of visited nodes equals the total number of nodes minus 1, it's still connected
            if (visited.Count == roomGraph.Count - 1)
            {
                nonCutVertices.Add(vertex);
            }

        }
        return nonCutVertices;
    }



    private Dictionary<Vector3Int, List<Vector3Int>> GetRemainingGraphFromCuttingVertex(Dictionary<Vector3Int, List<Vector3Int>> graph, Vector3Int vertex)
    {
        var roomToCheck = rooms.Where(r => r.floorTilesPositions.Contains(vertex)).First();
        var modifiedGraph = CloneGraph(graph);
        foreach (var room in modifiedGraph.Keys.ToList())
        {
            List<Vector3Int> neighborsToRemove = new List<Vector3Int>();

            foreach (var neighbor in modifiedGraph[room])
            {
                HashSet<Vector3Int> corridor = CreateCorridor(room, neighbor);

                if (roomToCheck.floorTilesPositions.Intersect(corridor).Count() != 0)
                {
                    neighborsToRemove.Add(neighbor);
                }
            }

            foreach (var neighbor in neighborsToRemove)
            {
                modifiedGraph[room].Remove(neighbor);
                modifiedGraph[neighbor].Remove(room);
            }
        }
        return modifiedGraph;
    }



    private void PrintRoomGraph()
    {
        HashSet<Vector3Int> visitedRooms = new HashSet<Vector3Int>();

        foreach (var roomCenter in roomGraph.Keys)
        {
            if (!visitedRooms.Contains(roomCenter))
            {
                PrintRoomAndNeighbors(roomCenter, 0, visitedRooms);
            }
        }
    }

    private void PrintRoomAndNeighbors(Vector3Int roomCenter, int depth, HashSet<Vector3Int> visitedRooms)
    {
        visitedRooms.Add(roomCenter);

        string indent = new string('-', depth * 2); // 2 dashes per depth level for clarity
        Debug.Log(indent + roomCenter);

        foreach (var neighbor in roomGraph[roomCenter])
        {
            if (!visitedRooms.Contains(neighbor))
            {
                PrintRoomAndNeighbors(neighbor, depth + 1, visitedRooms);
            }
        }
    }

    void PrintGraph(Dictionary<Vector3Int, List<Vector3Int>> roomGraph)
    {
        foreach (var room in roomGraph)
        {
            Debug.Log("Room: " + room.Key + " Neighbours: " + string.Join(", ", room.Value));
        }
    }

    void PlaceDoors()
    {
        var visited = new HashSet<Vector3Int>();
        var queue = new Queue<Vector3Int>();


        var startPosition = floorPositions.First();

        foreach (var room in rooms)
        {
            var rooms = GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            startPosition = rooms.First();
            break;

        }


        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentRoom = rooms.FirstOrDefault(x => x.floorTilesPositions.Contains(current));

            if (IsRoomPosition(current))
            {
                foreach (var neighbor in GetNeighbors(current))
                {
                    if (corridorPositions.Contains(neighbor))
                    {
                        if (currentRoom.isBossRoom)
                        {
                            PlaceDoor(current, Color.red);
                            currentRoom.doorTilesPositions.Add(current);
                        }
                        else
                        {
                            PlaceDoor(current, Color.green);
                            currentRoom.doorTilesPositions.Add(current);
                        }

                    }

                }

            }

            foreach (var neighbor in GetNeighbors(current))
            {
                if (floorPositions.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }

    bool IsRoomPosition(Vector3Int position)
    {
        foreach (var room in rooms)
        {
            var rooms = GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            if (rooms.Contains(position))
            {
                return true;
            }
        }
        return false;
    }


    void PlaceDoor(Vector3Int position, Color? color)
    {
        tilemapVisualizer.PaintDoorTile(position, color);
    }

    List<Vector3Int> GetNeighbors(Vector3Int position)
    {
        return new List<Vector3Int>
        {
            position + Vector3Int.up,
            position + Vector3Int.down,
            position + Vector3Int.left,
            position + Vector3Int.right
        };
    }


    private HashSet<Vector3Int> CreateCorridor(Vector3Int current, Vector3Int destination)
    {
        HashSet<Vector3Int> corridor = new HashSet<Vector3Int>();
        var position = current;

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector3Int.up;

            }
            else if (destination.y < position.y)
            {
                position += Vector3Int.down;
            }
            corridor.Add(position);
            // widen corridor
            corridor.Add(position + Vector3Int.left);
            corridor.Add(position + Vector3Int.right);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector3Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector3Int.left;
            }
            corridor.Add(position);
            // widen corridor
            corridor.Add(position + Vector3Int.up);
            corridor.Add(position + Vector3Int.down);
        }
        return corridor;
    }
    private Vector3Int FindClosestRoom(Vector3Int current, List<Vector3Int> roomCenters)
    {
        var closest = Vector3Int.zero;
        var distance = float.MaxValue;

        foreach (var room in roomCenters)
        {
            var newDistance = Vector3Int.Distance(current, room);
            if (newDistance < distance)
            {
                distance = newDistance;
                closest = room;
            }
        }
        return closest;

    }

    public HashSet<Vector3Int> GetActualRoomFloorPositions(List<BoundsInt> roomList)
    {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        foreach (var room in roomList)
        {
            for (int y = roomWidthOffset; y < room.size.x - roomWidthOffset; y++)
            {
                for (int x = roomWidthOffset; x < room.size.y - roomWidthOffset; x++)
                {
                    Vector3Int position = room.min + new Vector3Int(y, x);
                    floor.Add(position);
                }

            }
        }
        return floor;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(bool alreadySpawned, ServerRpcParams rpcParams = default)
    {
        SpawnPlayer(rpcParams.Receive.SenderClientId, alreadySpawned);
    }

    private void SpawnPlayer(ulong clientId, bool alreadyspawned)
    {
        GameObject playerPrefabToSpawn;

        if (clientId == NetworkManager.Singleton.LocalClientId) // Host player
        {
            playerPrefabToSpawn = playerChildPrefab; // host get playerChild
        }
        else
        {
            playerPrefabToSpawn = playerGhostPrefab; // client gets ghost
        }

        if (!alreadyspawned)
        {
            GameObject playerInstance = Instantiate(playerPrefabToSpawn, firstRoomCenter, Quaternion.identity);
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(clientId);

        }
        else
        {
            MoveAllPlayersClientRpc(firstRoomCenter);
        }

    }

    [ClientRpc]
    private void MoveAllPlayersClientRpc(Vector3Int roomCenter)
    {
        // Check if this is not the host, because the host's object has already been moved
        // Get the player object for this client and move it
        NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        if (playerObject != null)
        {
            playerObject.transform.position = roomCenter;
        }
        else
        {
            Debug.LogWarning("Player object not found for this client!");
        }
    }

    private void MoveAllPlayersToRoom(Vector3Int roomCenter)
    {
        // Iterate over all connected clients
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            // Get the player's NetworkObject for this client
            NetworkObject playerObject = client.PlayerObject;

            if (playerObject != null)
            {
                // Set the player's position to the firstRoomCenter
                playerObject.transform.position = roomCenter;
            }
            else
            {
                Debug.LogWarning($"Player object for client {client.ClientId} not found!");
            }
        }

    }
}


