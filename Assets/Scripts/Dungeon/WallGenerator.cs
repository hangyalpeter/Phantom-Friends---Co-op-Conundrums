using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector3Int> floorPositions, TilemapVisualizer tilemapVisualizer, HashSet<Room>? rooms)
    {
        var directions = new Vector3Int[]
        {

            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        var wallPositionsList = FindWallsInDirections(floorPositions, directions, rooms);
        foreach (var wallPosition in wallPositionsList)
        {
            tilemapVisualizer.PaintSingleWall(wallPosition, null);
        }
    }

    private static HashSet<Vector3Int> FindWallsInDirections(HashSet<Vector3Int> floorPositions, Vector3Int[] directions, HashSet<Room>? rooms)
    {
        HashSet<Vector3Int> wallPositions = new HashSet<Vector3Int>();
        foreach (var floorPosition in floorPositions)
        {
            Room room = null;
            if (rooms != null)
            {
                room = rooms.Where(r => r.floorTilesPositions.Contains(floorPosition)).FirstOrDefault();
            }
            foreach (var direction in directions)
            {
                var wallPosition = floorPosition + direction;
                if (!floorPositions.Contains(wallPosition))
                {
                    wallPositions.Add(wallPosition);
                    if (room != null)
                    {
                        room.wallTilesPositions.Add(wallPosition);
                    }
                }
            }
        }
        return wallPositions;
    }
}
