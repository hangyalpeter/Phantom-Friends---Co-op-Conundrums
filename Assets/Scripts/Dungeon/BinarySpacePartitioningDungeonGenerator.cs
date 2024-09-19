using Assets.Scripts.Dungeon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public TilemapVisualizer TilemapVisualizer => tilemapVisualizer;
    public override void RunProceduralGeneration()
    {
        GenerateDungeon();
    }
  
    public HashSet<Room> GenerateDungeon()
    {
        InitializeMap();
        while(rooms.Count < 5)
        {
            CreateRooms();
        }
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
        Debug.Log("Rooms count: " + rooms.Count);

    }

    private void ConnectRoomsWithCorridors()
    {

        corridorPositions = ConnectRooms();

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
        var bossRoomCenter = Vector3Int.zero;
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();
        var roomCenters = rooms.Select(x => (Vector3Int.RoundToInt(x.bounds.center))).ToList();
        var current = roomCenters[0];
        player.transform.position = current;
        ghost.transform.position = current;
        roomCenters.Remove(current);

        while (roomCenters.Count > 0)
        {
            var closest = FindClosestRoom(current, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector3Int> newCorridor = CreateCorridor(current, closest);
            current = closest;
            corridors.UnionWith(newCorridor);
        }
        bossRoomCenter = current;
        Debug.Log("Boss room center: " + bossRoomCenter);
        rooms.FirstOrDefault(room => room.floorTilesPositions.Contains(bossRoomCenter)).isBossRoom = true;
        return corridors;
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

        //TestDoorGenerationIfAddsToRooms();
    }

    private void TestDoorGenerationIfAddsToRooms()
    {
        foreach (var item in rooms)
        {
            foreach (var door in item.doorTilesPositions)
            {
                Debug.Log("Door position: " + door);
                tilemapVisualizer.PaintSingleWall(door, Color.red);
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

}