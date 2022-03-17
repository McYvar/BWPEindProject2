using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomInfo
{
    public string Name;
    public int X;
    public int Y;
}

public class RoomController : MonoBehaviour
{
    public static RoomController instance;

    string CurrentWorldName = "Basement";

    RoomInfo currentLoadRoomData;

    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();

    public List<Room> loadRooms = new List<Room>();

    bool isLoadingRoom = false;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue()
    {
        if(isLoadingRoom)
        {
            return;
        }

        if(loadRoomQueue.Count == 0)
        {
            Time.timeScale = 1f;
            return;
        }

        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;
        StartCoroutine(LoadRoomRoutine(currentLoadRoomData));
    }

    public void LoadRoom(string name, int x, int y)
    {
        if (DoesRoomExist(x, y))
        {
            return;
        }

        RoomInfo newRoomData = new RoomInfo();
        newRoomData.Name = name;
        newRoomData.X = x;
        newRoomData.Y = y;

        loadRoomQueue.Enqueue(newRoomData);
    }

    IEnumerator LoadRoomRoutine(RoomInfo info)
    {
        string roomName = CurrentWorldName + info.Name;

        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);

        while(!loadRoom.isDone)
        {
            yield return null;
        }
    }

    public void RegisterRoom(Room room)
    {
        room.transform.position = new Vector3(currentLoadRoomData.X * room.width, 0, currentLoadRoomData.Y * room.height);

        room.x = currentLoadRoomData.X;
        room.y = currentLoadRoomData.Y;
        room.name = CurrentWorldName + "-" + currentLoadRoomData.Name + "-" + room.x + ", " + room.y; ;
        room.transform.parent = transform;

        isLoadingRoom = false;

        loadRooms.Add(room);
    }

    public bool DoesRoomExist(int x, int y)
    {
        return loadRooms.Find(item => item.x == x && item.y == y) != null;
    }
}
