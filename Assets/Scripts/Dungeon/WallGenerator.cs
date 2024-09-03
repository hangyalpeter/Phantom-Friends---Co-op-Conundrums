using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector3Int> floorPositions, TilemapVisualizer tilemapVisualizer)
    {
        var directions = new Vector3Int[]
        {

            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };
        
        var wallPositionsList = FindWallsInDirections(floorPositions, directions);
        foreach (var wallPosition in wallPositionsList)
        {
            tilemapVisualizer.PaintSingleWall(wallPosition, null);
        }
    }

    private static HashSet<Vector3Int> FindWallsInDirections(HashSet<Vector3Int> floorPositions, Vector3Int[] directions)
    {
        HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();
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
