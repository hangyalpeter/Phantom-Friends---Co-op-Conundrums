using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var directions = new Vector2Int[]
        {

            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        
        var wallPositionsList = FindWallsInDirections(floorPositions, directions);
        foreach (var wallPosition in wallPositionsList)
        {
            tilemapVisualizer.PaintSingleWall(wallPosition);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, Vector2Int[] directions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var floorPosition in floorPositions)
        {
            foreach (var direction in directions)
            {
                var wallPosition = floorPosition + direction;
                if (!floorPositions.Contains(wallPosition))
                {
                    wallPositions.Add(wallPosition);
                }
            }
        }
        return wallPositions;
    }
}
