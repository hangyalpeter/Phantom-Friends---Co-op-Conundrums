using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralGenerationUtilityAlgorithms
{
    public static HashSet<Vector3Int> RandomWalk(Vector3Int start, int length)
    {
        HashSet<Vector3Int> path = new HashSet<Vector3Int>();
        path.Add(start);
        Vector3Int current = start;
        for (int i = 0; i < length; i++)
        {
            Vector3Int next = current + RandomDirection();
            path.Add(next);
            current = next;
        }
        return path;
    }

    public static List<Vector3Int> RandomWalkCorridor(Vector3Int start, int length)
    {
        List<Vector3Int> path = new List<Vector3Int>();
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

    public static HashSet<Room>BinarySpacePartitioning(BoundsInt space, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        HashSet<Room> rooms = new HashSet<Room>();
        roomsQueue.Enqueue(space);
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if (room.size.x >= minWidth && room.size.y >= minHeight)
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    if (room.size.x >= 2 * minWidth)
                    {
                        SplitVertically(roomsQueue, room, minHeight);
                    }
                    else if (room.size.y >= 2 * minHeight)
                    {
                        SplitHorizontally(roomsQueue, room, minWidth);
                    }
                    else
                    {
                        rooms.Add(new Room(room));
                    }
                }
                else
                {
                    if (room.size.y >= 2 * minHeight)
                    {
                        SplitHorizontally(roomsQueue, room, minWidth);
                    }
                    else if (room.size.x >= 2 * minWidth)
                    {
                        SplitVertically(roomsQueue, room, minHeight);
                    }
                    else
                    {
                        rooms.Add(new Room(room));
                    }
                }
            }
        }
        return rooms;
    }


    private static void SplitVertically(Queue<BoundsInt> roomsQueue, BoundsInt room, int minHeight)
    {
        //var split = UnityEngine.Random.Range(minHeight, room.size.y - minHeight);
        var split = UnityEngine.Random.Range(1, room.size.x);
        
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(split, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + split, room.min.y, room.min.z), new Vector3Int(room.size.x - split, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);

    }

    private static void SplitHorizontally(Queue<BoundsInt> roomsQueue, BoundsInt room, int minWidth)
    {
        //var split = UnityEngine.Random.Range(minWidth, room.size.x - minWidth);
        var split = UnityEngine.Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x,split, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + split, room.min.z), new Vector3Int(room.size.x, room.size.y - split, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    private static Vector3Int RandomDirection()
    {
        Vector3Int[] directions = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };
        return directions[UnityEngine.Random.Range(0, directions.Length)];
        
    }
}
