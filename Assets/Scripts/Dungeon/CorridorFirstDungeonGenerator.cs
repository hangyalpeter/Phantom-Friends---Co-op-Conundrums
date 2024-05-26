using Assets.Scripts.Dungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : DungeonGeneratorStrategy
{
    [SerializeField]
    private RandomWalkDungeonGenerator randomWalkDungeonGenerator;

    [SerializeField]
    private TilemapVisualizer tilemapVisualizer;
    private Vector3Int start = Vector3Int.zero;
    [SerializeField]
    private int length = 15;
    [SerializeField]
    private int count = 5;
    [SerializeField]
    private float roomPercent = 0.8f;

    public override void RunProceduralGeneration()
    {
        tilemapVisualizer.Clear();
        CorridorFirstDungeonGeneration();

    }

    private void CorridorFirstDungeonGeneration()
    {
        HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>();
        HashSet<Vector3Int> potentialRoomPositions = new HashSet<Vector3Int>();

        List<List<Vector3Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector3Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector3Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = WidenCorridor(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        tilemapVisualizer.PaintFloorTiles(floorPositions, null);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private List<Vector3Int> WidenCorridor(List<Vector3Int> corridors)
    {
        List<Vector3Int> widenedCorridor = new List<Vector3Int>();
        for (int i = 0;i < corridors.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    widenedCorridor.Add(new Vector3Int(corridors[i].x + j - 1, corridors[i].y + k - 1));
                }
            }
        }
        return widenedCorridor;

    }

    private void CreateRoomsAtDeadEnds(List<Vector3Int> deadEnds, HashSet<Vector3Int> floors)
    {
        foreach (var position in deadEnds)
        {
            if (!floors.Contains(position))
            {
                var room = randomWalkDungeonGenerator.RunRandomWalk(position);
                floors.UnionWith(room);
            }
        }
    }

    private List<Vector3Int> FindAllDeadEnds(HashSet<Vector3Int> floorPositions)
    {
        List<Vector3Int> deadEnds = new List<Vector3Int>();
        foreach (var position in floorPositions)
        {
            if (IsDeadEnd(position, floorPositions))
            {
                deadEnds.Add(position);
            }
        }
        return deadEnds;
    }

    private bool IsDeadEnd(Vector3Int position, HashSet<Vector3Int> floorPositions)
    {
        var directions = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };
        int neighbourCount = 0;
        foreach (var direction in directions )
        {
            if (floorPositions.Contains(position + direction))
            {
                neighbourCount++;
            }
        }
        if (neighbourCount == 1)
        {
            return true;
        }
        return false;
    }

    private HashSet<Vector3Int> CreateRooms(HashSet<Vector3Int> potentialRoomPositions)
    {
        HashSet<Vector3Int> roomPositions = new HashSet<Vector3Int>();
        int roomCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        List<Vector3Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomCount).ToList();
        foreach (var roomPosition in roomsToCreate)
        {
            var room = randomWalkDungeonGenerator.RunRandomWalk(roomPosition);
            roomPositions.UnionWith(room);
        }

        return roomPositions;
    }

    private List<List<Vector3Int>> CreateCorridors(HashSet<Vector3Int> floorPositions, HashSet<Vector3Int> potentialRoomPositions)
    {
        var current = start;
        potentialRoomPositions.Add(current);
        List<List<Vector3Int>> corridors = new List<List<Vector3Int>>();
        for (int i = 0; i < count; i++)
        {
            List<Vector3Int> path = ProceduralGenerationUtilityAlgorithms.RandomWalkCorridor(current, length);
            corridors.Add(path);
            current = path[path.Count - 1];
            potentialRoomPositions.Add(current);
            floorPositions.UnionWith(path);
        }

        return corridors;
    }
}
