using Assets.Scripts.Dungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        roomsDictionary = ProceduralGenerationUtilityAlgorithms.BinarySpacePartitioning(new BoundsInt(Vector3Int.zero, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minWidth, minHeight);

        tilemapVisualizer.Clear();
        List<Vector3Int> roomCenters = new List<Vector3Int>();

        HashSet<Vector3Int> floors = new HashSet<Vector3Int>();

        var roomColors = new Dictionary<BoundsInt, Color>();

        foreach (var room in roomsDictionary)
        {
            var color = GenerateRandomColor();
            roomColors.Add(room.Value, color.Value);

            tilemapVisualizer.PaintFloorTiles(CreateSimpleRooms(new List<BoundsInt> { room.Value }), color);
            
            roomCenters.Add(Vector3Int.RoundToInt(room.Value.center));
            floors.UnionWith(CreateSimpleRooms(new List<BoundsInt> { room.Value }));
        }

  
        Debug.Log("Room centers: " + roomCenters.Count);

        corridorPositions = ConnectRooms(roomCenters);
        
        foreach (var corridor in corridorPositions)
        {
            if (floors.Contains(corridor))
            {
                tilemapVisualizer.PaintFloorTiles(new HashSet<Vector3Int> { corridor }, null);
            }
            else
            {
                tilemapVisualizer.PaintFloorTiles(new HashSet<Vector3Int> { corridor }, Color.black);
            }
        }
        
        floors.UnionWith(corridorPositions);
        WallGenerator.CreateWalls(floors, tilemapVisualizer);

                   
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

    private HashSet<Vector3Int> CreateSimpleRooms(List<BoundsInt> roomList)
    {
        HashSet<Vector3Int> floor = new HashSet<Vector3Int>();
        foreach (var room in roomList)
        {
            for (int y = roomWidthOffset; y < room.size.y - roomWidthOffset; y++)
            {
                for (int x = roomWidthOffset; x < room.size.y - roomWidthOffset; x++)
                {
                    Vector3Int position =  room.min + new Vector3Int(y, x);
                    floor.Add(position);
                }

            }
        }
        return floor;
    }


}
