using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonLogicHandler : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private BinarySpacePartitioningDungeonGenerator dungeonGenerator;
    private HashSet<Room> rooms = new HashSet<Room>();

    private EnemySpawner spawner;
    void Start()
    {
        rooms = dungeonGenerator.GenerateDungeon();
        spawner = gameObject.GetComponent<EnemySpawner>();
    }

    private void Update()
    {
        CheckIfPlayerEnteredRoom();
        foreach (var room in rooms)
        {
            var actualRooms = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.position);
            if (actualRooms.Contains(playerPosition))
            {
                if (CheckIsRoomCleared(room))
                {
                    OpenAllFinishedRooms();
                    room.isFinished = true;
                }
            }
        }

    }

    private bool CheckIsRoomCleared(Room room)
    {
       return !room.enemies.Any(e => e != null);
    }


    private void CheckIfPlayerEnteredRoom()
    {
        foreach (var room in rooms)
        {
            var rooms = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { room.bounds });
            var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.position);

            if (rooms.Contains(playerPosition) && !room.isFinished && !room.isBossRoom && !room.isVisited)
            {
                Debug.Log("Player entered room" + room.bounds.center);


                room.isVisited = true;
                // TODO: generate enemies after delay
                StartCoroutine(DelayCloseCurrentRoom(room));

            }
        }
    }

    private IEnumerator DelayCloseCurrentRoom(Room room)
    {

        yield return new WaitForSeconds(1f);
        CloseCurrentRoom(room);
        room.enemies.AddRange(spawner.SpawnEnemies(room.bounds.center));
    }


    private void CloseCurrentRoom(Room currentPlayerRoom)
    {

        var currentPlayerRoomFloorPositions = dungeonGenerator.GetActualRoomFloorPositions(new List<BoundsInt>() { currentPlayerRoom.bounds });
        var playerPosition = dungeonGenerator.TilemapVisualizer.FloorTilemap.WorldToCell(player.position);

        foreach (var door in currentPlayerRoom.doorTilesPositions)
        {
            dungeonGenerator.TilemapVisualizer.PaintDoorTile(door, Color.red);
        }


    }

    private void OpenAllFinishedRooms()
    {

        foreach (var room in rooms)
        {
            if (!room.isBossRoom)
            {
                foreach (var door in room.doorTilesPositions)
                {
                    //tilemapVisualizer.PaintDoorTile(door, Color.green);
                    dungeonGenerator.TilemapVisualizer.PaintOpenGateTile(door, null);
                }
            }
            else if (!rooms.Any(x => !x.isFinished && !x.isBossRoom))
            {
                foreach (var door in room.doorTilesPositions)
                {
                    dungeonGenerator.TilemapVisualizer.PaintOpenGateTile(door, null);
                }
            }
        }
    }

}
