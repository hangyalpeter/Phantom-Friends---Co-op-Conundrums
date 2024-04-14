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
    private Vector2Int start = Vector2Int.zero;
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
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnds(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);

        for (int i = 0; i < corridors.Count; i++)
        {
            corridors[i] = WidenCorridor(corridors[i]);
            floorPositions.UnionWith(corridors[i]);
        }

        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    private List<Vector2Int> WidenCorridor(List<Vector2Int> corridors)
    {
        List<Vector2Int> widenedCorridor = new List<Vector2Int>();
        for (int i = 0;i < corridors.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                for (int k = 0; k < 3; k++)
                {
                    widenedCorridor.Add(new Vector2Int(corridors[i].x + j - 1, corridors[i].y + k - 1));
                }
            }
        }
        return widenedCorridor;

    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> floors)
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

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            if (IsDeadEnd(position, floorPositions))
            {
                deadEnds.Add(position);
            }
        }
        return deadEnds;
    }

    private bool IsDeadEnd(Vector2Int position, HashSet<Vector2Int> floorPositions)
    {
        var directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
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

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);
        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomCount).ToList();
        foreach (var roomPosition in roomsToCreate)
        {
            var room = randomWalkDungeonGenerator.RunRandomWalk(roomPosition);
            roomPositions.UnionWith(room);
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var current = start;
        potentialRoomPositions.Add(current);
        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();
        for (int i = 0; i < count; i++)
        {
            List<Vector2Int> path = ProceduralGenerationUtilityAlgorithms.RandomWalkCorridor(current, length);
            corridors.Add(path);
            current = path[path.Count - 1];
            potentialRoomPositions.Add(current);
            floorPositions.UnionWith(path);
        }

        return corridors;
    }
}
