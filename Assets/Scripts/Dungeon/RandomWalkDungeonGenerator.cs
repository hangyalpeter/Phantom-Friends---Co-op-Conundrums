using Assets.Scripts.Dungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomWalkDungeonGenerator : DungeonGeneratorStrategyExperiment
{
    private Vector3Int start = Vector3Int.zero;

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
        HashSet<Vector3Int> floorPositions = RunRandomWalk(start);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions, null);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer, null);
    }

    public HashSet<Vector3Int> RunRandomWalk(Vector3Int position)
    {
        var current = position;
        HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>();
        for (int i = 0; i < iterations; i++)
        {
            HashSet<Vector3Int> path = ProceduralGenerationUtilityAlgorithms.RandomWalk(current, length);
            floorPositions.UnionWith(path);

            if (randomizeStart)
            {
                current = floorPositions.ElementAt(UnityEngine.Random.Range(0, floorPositions.Count));
            }
        }

        return floorPositions;
    }
}
