using Assets.Scripts.Dungeon;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    BinaryTreeNode root = new BinaryTreeNode();

    private Vector3Int firstRoomCenter;
    private Vector3Int bossRoomCenter = Vector3Int.zero;

    [SerializeField]
    private GameObject playerGhostPrefab;
    [SerializeField]
    private GameObject playerChildPrefab;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var kvp in roomGraph)
        {
            foreach (var neighbor in kvp.Value)
            {
                Gizmos.DrawLine(kvp.Key, neighbor);
            }
        }
    }


    public override void RunProceduralGeneration()
    {
        GenerateDungeon(false, PlayerPrefs.GetInt("DungeonSeed"));
    }

    public HashSet<Room> GenerateDungeon(bool alreadySpawned, int seed)
    {
        InitializeMap();

        CreateRooms();


        if (alreadySpawned)
        {
            SpawnPlayerCharacters(alreadySpawned);
        }

        // TODO: remove print methods before final turn in
        PrintGraph(roomGraph);

        return rooms;
    }

    public void SpawnPlayerCharacters(bool alreadySpawned)
    {
        if (StartingSceneController.ChoosenPlayMode != StartingSceneController.PlayMode.CouchCoop)
        {
            RequestSpawnPlayerServerRpc(alreadySpawned);
        }
        else
        {
            SpawnLocalPlayersForCouchCoop(alreadySpawned);
        }
    }

    private void CreateRooms()
    {
        InitializeMap();
        //rooms = ProceduralGenerationUtilityAlgorithms.BinarySpacePartitioning(new BoundsInt(Vector3Int.zero, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minWidth, minHeight);
        //rooms = ProceduralGenerationUtilityAlgorithms.BinarySpacePartitioning2(new BoundsInt(Vector3Int.zero, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minWidth, minHeight);

        rooms = ProceduralGenerationUtilityAlgorithmsExperiments.BinarySpacePartitioning3(new BoundsInt(Vector3Int.zero, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minWidth, minHeight, out root);

        GenerateRoomPositions();

        ConnectRoomsWithCorridors();

        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer, rooms);
        BuildGraphFromCorridors(corridorPositions, rooms);
        firstRoomCenter = roomGraph.Keys.First();

        MarkBossRoom();

        PlaceDoors();

    }

    private void ConnectRoomsWithCorridors()
    {

        // commented out code left for "linear type" dungeon generation

        //corridorPositions = ConnectRooms();
        //rooms.First().corridorTilePositions = corridorPositions;

        corridorPositions = rooms.First(c => c.corridorTilePositions.Count != 0).corridorTilePositions;

        //rooms.First().corridorTilePositions = corridorPositions;



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
        root = new BinaryTreeNode();
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

    // TODO: slow solution
    private void MarkBossRoom2()
    {
        var bossroomPositions = new HashSet<Room>();
        foreach (var room in rooms)
        {
            if (FindBossRoomHelper(room))
            {
                bossroomPositions.Add(room);
            }
        }
        bossroomPositions.Last().isBossRoom = true;


        // Parallelize the check for each room - it takes about half the time compared to the single threaded solution

        //var bossroomPositions = new ConcurrentBag<Room>();
       /* Parallel.ForEach(rooms, room =>
        {
            if (FindBossRoomHelper(room))
            {
                bossroomPositions.Add(room);
            }
        });

        // Mark the last room in the result as the boss room
        if (bossroomPositions.Any())
        {
            var bossroompos = Vector3Int.RoundToInt(bossroomPositions.Last().bounds.center);
            bossroomPositions.Last().isBossRoom = true;
            if (firstRoomCenter == bossroompos)
            {
                firstRoomCenter = Vector3Int.RoundToInt(rooms.First(r => !r.isBossRoom).bounds.center);
            }
        }*/

        /*     FindBossRoomWithCallback(bossroomPositions =>
             {
                 if (bossroomPositions.Any())
                 {
                     var bossroompos = Vector3Int.RoundToInt(bossroomPositions.Last().bounds.center);
                     bossroomPositions.Last().isBossRoom = true;
                     if (firstRoomCenter == bossroompos)
                     {
                         firstRoomCenter = Vector3Int.RoundToInt(rooms.First(r => !r.isBossRoom).bounds.center);
                     }
                 }
             });*/
    }


    private void FindBossRoomWithCallback(Action<HashSet<Room>> onComplete)
    {
        Task.Run(() =>
        {
            var bossroomPositions = new ConcurrentBag<Room>();

            Parallel.ForEach(rooms, room =>
            {
                if (FindBossRoomHelper(room))
                {
                    bossroomPositions.Add(room);
                }
            });

            // Convert ConcurrentBag to HashSet and pass it to the callback
            onComplete(new HashSet<Room>(bossroomPositions));
        });
    }

    // Usage



    bool FindBossRoomHelper(Room roomToExclude)
    {
        var visited = new HashSet<Vector3Int>();
        var queue = new Queue<Vector3Int>();
        var visitedRooms = new HashSet<Room>();

        var corridorsss = new HashSet<Vector3Int>();

        var copiedRooms = rooms.Where(r => r != roomToExclude);

        // remove potential corridors that would pass through the excluded room
        foreach (var tile in roomToExclude.floorTilesPositions)
        {
            foreach (var neighbour in GetNeighbors(tile))
            {
                if (corridorPositions.Contains(neighbour))
                {
                    corridorsss.Add(neighbour);
                }
            }
        }

        var copiedFloorpos = floorPositions.Except(roomToExclude.floorTilesPositions).Except(corridorsss);

        var startPosition = copiedFloorpos.First();

        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentRoom = copiedRooms.FirstOrDefault(x => x.floorTilesPositions.Contains(current));
            if (IsRoomPosition(current))
            {
                visitedRooms.Add(currentRoom);
            }

            foreach (var neighbor in GetNeighbors(current))
            {
                if (copiedFloorpos.Contains(neighbor) && !visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        return visitedRooms.Count() == copiedRooms.Count();
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
            RemoveVertex(modifiedGraph, vertex);

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


    // used for the other generation algorithm, left it for future reference
    private List<Vector3Int> GetNonCutVertices2()
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


    void BuildGraphFromCorridors(HashSet<Vector3Int> corridorPositions, HashSet<Room> rooms)
    {

        foreach (var room in rooms)
        {
            var roomCenter = Vector3Int.RoundToInt(room.bounds.center);
            roomGraph[roomCenter] = new List<Vector3Int>();
        }

        var visitedCorridors = new HashSet<Vector3Int>();
        var queue = new Queue<Vector3Int>();

        foreach (var startCorridor in corridorPositions)
        {
            if (visitedCorridors.Contains(startCorridor)) continue;

            queue.Enqueue(startCorridor);
            visitedCorridors.Add(startCorridor);

            // Track rooms connected to this corridor section
            var connectedRooms = new HashSet<Vector3Int>();

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (corridorPositions.Contains(neighbor) && !visitedCorridors.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visitedCorridors.Add(neighbor);
                    }
                    else
                    {
                        // Check if the neighbor is part of a room
                        var room = rooms.FirstOrDefault(r => r.floorTilesPositions.Contains(neighbor));
                        if (room != null)
                        {
                            var roomCenter = Vector3Int.RoundToInt(room.bounds.center);
                            connectedRooms.Add(roomCenter);
                        }
                    }
                }
            }

            // Connect all detected rooms in this corridor section
            var roomCenters = connectedRooms.ToList();
            for (int i = 0; i < roomCenters.Count; i++)
            {
                for (int j = i + 1; j < roomCenters.Count; j++)
                {
                    AddEdge(roomGraph, roomCenters[i], roomCenters[j]);
                }
            }
        }

    }
    void AddEdge(Dictionary<Vector3Int, List<Vector3Int>> graph, Vector3Int room1, Vector3Int room2)
    {
        if (!graph[room1].Contains(room2))
        {
            graph[room1].Add(room2);
        }
        if (!graph[room2].Contains(room1))
        {
            graph[room2].Add(room1);
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
            //this is just the check for the doors positions, not "strictly" part of the bfs
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

            // This is the actual bfs step

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
            networkObject.SpawnAsPlayerObject(clientId, destroyWithScene: true);

        }
        else
        {
            MoveAllPlayersClientRpc(firstRoomCenter);
        }

    }

    [ClientRpc]
    private void MoveAllPlayersClientRpc(Vector3Int roomCenter)
    {
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

    private void SpawnLocalPlayersForCouchCoop(bool alreadySpawned)
    {
        if (!alreadySpawned)
        {
            GameObject player1Instance = Instantiate(playerChildPrefab, firstRoomCenter, Quaternion.identity);
            NetworkObject player1NetworkObject = player1Instance.GetComponent<NetworkObject>();
            player1NetworkObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, destroyWithScene: true);

            GameObject player2Instance = Instantiate(playerGhostPrefab, firstRoomCenter, Quaternion.identity);
            NetworkObject player2NetworkObject = player2Instance.GetComponent<NetworkObject>();
            player2NetworkObject.SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, destroyWithScene: true);
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player_Child").transform.position = firstRoomCenter;
            GameObject.FindGameObjectWithTag("Player_Ghost").transform.position = firstRoomCenter;
        }
    }



}

