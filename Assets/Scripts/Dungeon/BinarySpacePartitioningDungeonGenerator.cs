using Assets.Scripts.Dungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BinarySpacePartitioningDungeonGenerator : DungeonGeneratorStrategy
{
    [SerializeField]
    private int minWidth = 10;

    [SerializeField]
    private int minHeight = 10;

    [SerializeField]
    private int dungeonWidth = 30;
    [SerializeField]
    private int dungeonHeight = 30;
    [SerializeField]
    private int roomWidthOffset = 2;

    [SerializeField]
    private TilemapVisualizer tilemapVisualizer;

    private Dictionary<Vector3, BoundsInt> roomsDictionary = new Dictionary<Vector3, BoundsInt>();
    private HashSet<Vector3Int> corridorPositions = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>();
    private List<Vector3Int> roomCenters = new List<Vector3Int>();
    public override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        roomCenters.Clear();
        roomsDictionary = ProceduralGenerationUtilityAlgorithms.BinarySpacePartitioning(new BoundsInt(Vector3Int.zero, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minWidth, minHeight);

        tilemapVisualizer.Clear();

        floorPositions.Clear();

        var roomColors = new Dictionary<BoundsInt, Color>();

        foreach (var room in roomsDictionary)
        {
            var color = GenerateRandomColor();
            roomColors.Add(room.Value, color.Value);

            tilemapVisualizer.PaintFloorTiles(GetActualRoomFloorPositions(new List<BoundsInt> { room.Value }), color);

            roomCenters.Add(Vector3Int.RoundToInt(room.Value.center));
            floorPositions.UnionWith(GetActualRoomFloorPositions(new List<BoundsInt> { room.Value }));
        }


        Debug.Log("Room centers: " + roomCenters.Count);

        corridorPositions = ConnectRooms(roomCenters);
        var corridorPositionsCopy = new HashSet<Vector3Int>(corridorPositions);

        foreach (var corridor in corridorPositions)
        {
            if (!floorPositions.Contains(corridor))
            {
                tilemapVisualizer.PaintFloorTiles(new HashSet<Vector3Int> { corridor }, Color.black);
            }
            else
            {
                corridorPositionsCopy.Remove(corridor);
            }
        }

        corridorPositions = corridorPositionsCopy;

        //this is needed to connect the corridors to the rooms
        floorPositions.UnionWith(corridorPositions);


        tilemapVisualizer.PaintFloorTiles(floorPositions, null);

        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        PlaceDoors();

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
        return colors[UnityEngine.Random.Range(0, colors.Count)];
    }

    private HashSet<Vector3Int> ConnectRooms(List<Vector3Int> roomCenters)
    {
        HashSet<Vector3Int> corridors = new HashSet<Vector3Int>();
        var current = roomCenters[0];
        roomCenters.Remove(current);

        while (roomCenters.Count > 0)
        {
            var closest = FindClosestRoom(current, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector3Int> newCorridor = CreateCorridor(current, closest);
            current = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }

    
    void PlaceDoors()
    {
        var visited = new HashSet<Vector3Int>();
        var queue = new Queue<Vector3Int>();


        var startPosition = floorPositions.First();

        foreach (var room in roomsDictionary.Values)
        {
            var rooms = GetActualRoomFloorPositions(new List<BoundsInt>() { room });
            startPosition = rooms.First();
            break;
            
        }


        queue.Enqueue(startPosition);
        visited.Add(startPosition);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (IsRoomPosition(current))
            {
                Debug.Log("Current is room position: " + current);
                foreach (var neighbor in GetNeighbors(current))
                {
                    if (corridorPositions.Contains(neighbor))
                    {
                        Debug.Log("Placing door at: " + current);
                        PlaceDoor(current);
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
        foreach (var room in roomsDictionary.Values)
        {
            var rooms = GetActualRoomFloorPositions(new List<BoundsInt>() { room });
            if (rooms.Contains(position))
            {
                return true;
            }
        }
        return false;
    }


    void PlaceDoor(Vector3Int position)
    {
        tilemapVisualizer.PaintDoorTile(position, null);
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

    private HashSet<Vector3Int> GetActualRoomFloorPositions(List<BoundsInt> roomList)
    {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        foreach (var room in roomList)
        {
            for (int y = roomWidthOffset; y < room.size.y - roomWidthOffset; y++)
            {
                for (int x = roomWidthOffset; x < room.size.x - roomWidthOffset; x++)
                {
                    Vector3Int position = room.min + new Vector3Int(y, x);
                    floor.Add(position);
                }

            }
        }
        return floor;
    }




}

public class Node : System.IComparable<Node>
{
    public Vector3Int Position { get; }
    public int Priority { get; }

    public Node(Vector3Int position, int priority)
    {
        Position = position;
        Priority = priority;
    }

    public int CompareTo(Node other)
    {
        int result = Priority.CompareTo(other.Priority);
        if (result == 0)
        {
            result = Position.x.CompareTo(other.Position.x);
            if (result == 0)
            {
                result = Position.y.CompareTo(other.Position.y);
                if (result == 0)
                {
                    result = Position.z.CompareTo(other.Position.z);
                }
            }
        }
        return result;
    }
}

public class PriorityQueue<T>
{
    private List<KeyValuePair<T, int>> elements = new List<KeyValuePair<T, int>>();

    public int Count => elements.Count;

    public void Enqueue(T item, int priority)
    {
        elements.Add(new KeyValuePair<T, int>(item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Value < elements[bestIndex].Value)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Key;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }
}
