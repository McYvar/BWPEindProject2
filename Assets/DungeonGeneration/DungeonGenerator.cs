using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonGenerationData;
    private List<Vector2Int> dungeonRooms;

    private void Start()
    {
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData);
        SpawnRooms(dungeonRooms);
    }

    private void SpawnRooms(List<Vector2Int> rooms)
    {
        RoomController.instance.LoadRoom("Start", 0, 0);
        /*foreach(Vector2Int roomlLocation in rooms)
        {
            int i = Random.Range(0, 3);
            RoomController.instance.LoadRoom("Room" + i, roomlLocation.x, roomlLocation.y);
        }*/
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i < rooms.Count - 1)
            {
                int r = Random.Range(0, 3);
                RoomController.instance.LoadRoom("Room" + r, rooms[i].x, rooms[i].y);
            }
            else RoomController.instance.LoadRoom("End", rooms[i].x, rooms[i].y);
        }
    }
}
