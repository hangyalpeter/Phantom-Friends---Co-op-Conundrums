using Assets.Scripts.Dungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalkDungeonGenerator : DungeonGeneratorStrategy
{
    private Vector2Int start = Vector2Int.zero;

    [SerializeField]
    private int length = 100;
    [SerializeField]
    private int iterations = 10;
    [SerializeField]
    private bool randomizeStart = true;

    [SerializeField]
    private TilemapVisualizer tilemapVisualizer;
    public override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(start);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    public HashSet<Vector2Int> RunRandomWalk(Vector2Int position)
    {
        var current = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++)
        {
            HashSet<Vector2Int> path = ProceduralGenerationUtilityAlgorithms.RandomWalk(current, length);
            floorPositions.UnionWith(path);

            if (randomizeStart)
            {
                current = floorPositions.ElementAt(UnityEngine.Random.Range(0, floorPositions.Count));
            }
        }

        return floorPositions;
    }
}
