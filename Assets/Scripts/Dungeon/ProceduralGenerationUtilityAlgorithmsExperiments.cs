using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BinaryTreeNode
{
    public Room room; 

    public BinaryTreeNode left;
    public BinaryTreeNode right;
}

public static class ProceduralGenerationUtilityAlgorithmsExperiments
{
    public static Dictionary<Vector3Int, List<Vector3Int>> roomGraph = new Dictionary<Vector3Int, List<Vector3Int>>();
    private static Vector3Int firstRoomCenter;

    private static HashSet<Room> rooms = new HashSet<Room>();
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

    // iterative approach using a queue
    public static HashSet<Room> BinarySpacePartitioning(BoundsInt space, int minWidth, int minHeight)
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
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, split, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + split, room.min.z), new Vector3Int(room.size.x, room.size.y - split, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    // recursive approach, not using binary tree
    public static HashSet<Room> BinarySpacePartitioning2(BoundsInt space, int minWidth, int minHeight)
    {
        HashSet<Room> rooms = new HashSet<Room>();
        SplitSpaceRecursively(space, minWidth, minHeight, rooms);
        return rooms;
    }

    private static void SplitSpaceRecursively(BoundsInt room, int minWidth, int minHeight, HashSet<Room> rooms)
    {
        if (room.size.x >= minWidth && room.size.y >= minHeight)
        {
            bool splitVertically = UnityEngine.Random.value > 0.5f;
            if (splitVertically)
            {
                if (room.size.x >= 2 * minWidth)
                {
                    SplitVertically(room, minWidth, minHeight, rooms);
                }
                else if (room.size.y >= 2 * minHeight)
                {
                    SplitHorizontally(room, minWidth, minHeight, rooms);
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
                    SplitHorizontally(room, minWidth, minHeight, rooms);
                }
                else if (room.size.x >= 2 * minWidth)
                {
                    SplitVertically(room, minWidth, minHeight, rooms);
                }
                else
                {
                    rooms.Add(new Room(room));
                }
            }
        }
        else
        {
            rooms.Add(new Room(room));
        }
    }

    private static void SplitVertically(BoundsInt room, int minWidth, int minHeight, HashSet<Room> rooms)
    {
        //int split = UnityEngine.Random.Range(minWidth, room.size.x - minWidth);
        var split = UnityEngine.Random.Range(1, room.size.x);

        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(split, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + split, room.min.y, room.min.z), new Vector3Int(room.size.x - split, room.size.y, room.size.z));

        SplitSpaceRecursively(room1, minWidth, minHeight, rooms);
        SplitSpaceRecursively(room2, minWidth, minHeight, rooms);
    }

    private static void SplitHorizontally(BoundsInt room, int minWidth, int minHeight, HashSet<Room> rooms)
    {
        //int split = UnityEngine.Random.Range(minHeight, room.size.y - minHeight);
        var split = UnityEngine.Random.Range(1, room.size.y);

        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, split, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + split, room.min.z), new Vector3Int(room.size.x, room.size.y - split, room.size.z));

        SplitSpaceRecursively(room1, minWidth, minHeight, rooms);
        SplitSpaceRecursively(room2, minWidth, minHeight, rooms);
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


    // Binary tree solution

    private static BinaryTreeNode SplitSpaceRecursively(BoundsInt room, int minWidth, int minHeight)
    {
        if (room.size.x >= minWidth && room.size.y >= minHeight)
        {
            bool splitVertically = UnityEngine.Random.value > 0.5f;
            if (splitVertically)
            {
                if (room.size.x >= 2 * minWidth)
                {
                    return SplitVertically(room, minWidth, minHeight);
                }
                else if (room.size.y >= 2 * minHeight)
                {
                    return SplitHorizontally(room, minWidth, minHeight);
                }
                else
                {
                    return new BinaryTreeNode { room = new Room(room) };
                }
            }
            else
            {
                if (room.size.y >= 2 * minHeight)
                {
                    return SplitHorizontally(room, minWidth, minHeight);
                }
                else if (room.size.x >= 2 * minWidth)
                {
                    return SplitVertically(room, minWidth, minHeight);
                }
                else
                {
                    return new BinaryTreeNode { room = new Room(room) };
                }
            }
        }
        else
        {
            return new BinaryTreeNode { room = new Room(room) }; 
        }
    }

    private static BinaryTreeNode SplitVertically(BoundsInt room, int minWidth, int minHeight)
    {
        int split = UnityEngine.Random.Range(minWidth, room.size.x - minWidth);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(split, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + split, room.min.y, room.min.z), new Vector3Int(room.size.x - split, room.size.y, room.size.z));

        BinaryTreeNode node = new BinaryTreeNode { room = new Room(room) };
        node.left = SplitSpaceRecursively(room1, minWidth, minHeight);
        node.right = SplitSpaceRecursively(room2, minWidth, minHeight);
        return node;
    }

    private static BinaryTreeNode SplitHorizontally(BoundsInt room, int minWidth, int minHeight)
    {
        int split = UnityEngine.Random.Range(minHeight, room.size.y - minHeight);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, split, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + split, room.min.z), new Vector3Int(room.size.x, room.size.y - split, room.size.z));

        BinaryTreeNode node = new BinaryTreeNode { room = new Room(room) };
        node.left = SplitSpaceRecursively(room1, minWidth, minHeight);
        node.right = SplitSpaceRecursively(room2, minWidth, minHeight);
        return node;
    }

    public static HashSet<Room> BinarySpacePartitioning3(BoundsInt space, int minWidth, int minHeight, out BinaryTreeNode root2)
    {
        BinaryTreeNode root = SplitSpaceRecursively(space, minWidth, minHeight);
        CollectRooms(root, rooms);

        root.room.corridorTilePositions = ConnectSiblings(root);

        rooms.First().corridorTilePositions = root.room.corridorTilePositions;

        root2 = root;
        return rooms;
    }

    private static void CollectRooms(BinaryTreeNode node, HashSet<Room> rooms)
    {
        if (node == null) return;

        if (node.left == null && node.right == null)
        {
            rooms.Add(new Room(node.room.bounds));
        }

        CollectRooms(node.left, rooms);
        CollectRooms(node.right, rooms);
    }

    public static HashSet<Vector3Int> ConnectSiblings(BinaryTreeNode node)
    {
        HashSet<Vector3Int> siblingCorridors = new HashSet<Vector3Int>();
   

        if (node == null) return siblingCorridors;

        if (node.left != null && node.right != null)
        {

            var centers = rooms.Select(r => Vector3Int.RoundToInt(r.bounds.center));

            var siblingCorridor = CreateCorridor(
                Vector3Int.RoundToInt(node.left.room.bounds.center),
                Vector3Int.RoundToInt(node.right.room.bounds.center)
            );

            siblingCorridors.UnionWith(siblingCorridor);
        }

        siblingCorridors.UnionWith(ConnectSiblings(node.left));
        siblingCorridors.UnionWith(ConnectSiblings(node.right));

        return siblingCorridors;
    }



    private static HashSet<Vector3Int> CreateCorridor(Vector3Int current, Vector3Int destination)
    {
        HashSet<Vector3Int> corridor = new HashSet<Vector3Int>();
        var position = current;

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector3Int.right;
            }
            else
            {
                position += Vector3Int.left;
            }
            corridor.Add(position);
            corridor.Add(position + Vector3Int.up);
            corridor.Add(position + Vector3Int.down);
        }

        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector3Int.up;
            }
            else
            {
                position += Vector3Int.down;
            }
            corridor.Add(position);
            corridor.Add(position + Vector3Int.left); 
            corridor.Add(position + Vector3Int.right);
        }

        return corridor;
    }

}

