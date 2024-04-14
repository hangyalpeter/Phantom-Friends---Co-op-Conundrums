using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationUtilityAlgorithms
{
    public static HashSet<Vector2Int> RandomWalk(Vector2Int start, int length)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();
        path.Add(start);
        Vector2Int current = start;
        for (int i = 0; i < length; i++)
        {
            Vector2Int next = current + RandomDirection();
            path.Add(next);
            current = next;
        }
        return path;
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int start, int length)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        var direction = RandomDirection();
        var current = start;
        path.Add(current);
        for (int i = 0; i < length; i++)
        {
            current += direction;
            path.Add(current);
        }

        return path;
    }

    private static Vector2Int RandomDirection()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        return directions[UnityEngine.Random.Range(0, directions.Length)];
        
    }
}
