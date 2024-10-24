using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public BoundsInt bounds;
    public HashSet<Vector3Int> floorTilesPositions;
    public HashSet<Vector3Int> doorTilesPositions;
    public HashSet<Vector3Int> wallTilesPositions;
    public HashSet<Vector3Int> corridorTilePositions;
    public List<GameObject> enemies;
    public bool isBossRoom;
    public bool isFinished;
    public bool isVisited;
    public bool spawned;
    public Room(BoundsInt bounds)
    {
        this.bounds = bounds;
        floorTilesPositions = new HashSet<Vector3Int>();
        doorTilesPositions = new HashSet<Vector3Int>();
        wallTilesPositions = new HashSet<Vector3Int>();
        corridorTilePositions = new HashSet<Vector3Int>();
        isBossRoom = false;
        isFinished = false;
        isVisited = false;
        enemies = new List<GameObject>();
        spawned = false;
    }
}
