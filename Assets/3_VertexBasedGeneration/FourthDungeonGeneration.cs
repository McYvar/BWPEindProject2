using System.Collections.Generic;
using UnityEngine;

public class FourthDungeonGeneration : MonoBehaviour
{
    Dictionary<int, GameObject> dungeon = new Dictionary<int, GameObject>();
    [SerializeField] Material mat;
    [SerializeField] bool seeTroughMode;
    [SerializeField] GameObject player;

    private void Start()
    {
        float randomPointA = Random.Range(20, 30);
        float randomPointB = Random.Range(30, 60);
        Rooms room = new Rooms(new Vector3(0, 0, 0), new Vector3(0, 0, randomPointB), new Vector3(randomPointA, 0, 0), new Vector3(randomPointA, 0, randomPointB), dungeon.Count, mat, seeTroughMode);
        room.CreateRoomTile();
        room.AddDoor(new Vector3(Random.Range(room.GetRoomLowerLeft().x, room.GetRoomUpperLeft().x), 0, Random.Range(room.GetRoomLowerLeft().z, room.GetRoomUpperLeft().z)));
        room.AddDoor(new Vector3(Random.Range(room.GetRoomUpperLeft().x, room.GetRoomUpperRight().x), 0, Random.Range(room.GetRoomUpperLeft().z, room.GetRoomUpperRight().z)));
        room.AddDoor(new Vector3(Random.Range(room.GetRoomLowerRight().x, room.GetRoomUpperRight().x), 0, Random.Range(room.GetRoomLowerRight().z, room.GetRoomUpperRight().z)));
        room.AddDoor(new Vector3(Random.Range(room.GetRoomLowerLeft().x, room.GetRoomLowerRight().x), 0, Random.Range(room.GetRoomLowerLeft().z, room.GetRoomLowerRight().z)));
        room.CreateRoomWall(room.GetRoomLowerLeft(), room.GetRoomUpperLeft(), 2.5f);
        room.CreateRoomWall(room.GetRoomUpperLeft(), room.GetRoomUpperRight(), 2.5f);
        room.CreateRoomWall(room.GetRoomLowerRight(), room.GetRoomUpperRight(), 2.5f);
        room.CreateRoomWall(room.GetRoomLowerLeft(), room.GetRoomLowerRight(), 2.5f);
        Instantiate(player, Vector3.one * 3.5f + Vector3.up * 5, Quaternion.identity);
    }


    void AddRoom(GameObject obj)
    {
        dungeon.Add(dungeon.Count, obj);
        Debug.Log(dungeon[dungeon.Count - 1] + " has been added!");
    }
}
