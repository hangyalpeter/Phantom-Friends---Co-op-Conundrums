using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public BoundsInt bounds;
    public HashSet<Vector3Int> floorTilesPositions;
    public HashSet<Vector3Int> doorTilesPositions;
    public bool isBossRoom;
    public bool isFinished;
    public bool isVisited;
    public Room(BoundsInt bounds)
    {
        this.bounds = bounds;
        floorTilesPositions = new HashSet<Vector3Int>();
        doorTilesPositions = new HashSet<Vector3Int>();
        isBossRoom = false;
        isFinished = false;
        isVisited = false;
    }
}
