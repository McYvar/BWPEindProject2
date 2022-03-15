using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ThirdDungeonGeneration : MonoBehaviour
{
    [SerializeField] int gridWidth = 100;
    [SerializeField] int gridHeight = 100;

    [SerializeField] int roomSizeMin = 3;
    [SerializeField] int roomSizeMax = 7;

    [SerializeField] int numRooms = 10;

    [SerializeField] int minDistanceBetweenRooms = 1;

    [SerializeField] int minHallWidth;
    [SerializeField] int maxHallWidth;

    [SerializeField] Dictionary<Vector3Int, RoomInformation> dungeon = new Dictionary<Vector3Int, RoomInformation>();

    [SerializeField] GameObject FloorPrefab;

    private void Start()
    {
        Generate();
    }

    public void Generate()
    {
        for (int i = 0; i < numRooms; i++)
        {
            int xMin = Random.Range(0, gridWidth);
            int xMax = xMin + Random.Range(roomSizeMin, roomSizeMax + 1);
            int zMin = Random.Range(0, gridHeight);
            int zMax = zMin + Random.Range(roomSizeMin, roomSizeMax + 1);

            int xScale = xMax - xMin;
            int zScale = zMax - zMin;

            RoomInformation room = new RoomInformation(xMin, xMax, zMin, zMax, xScale, zScale);
            if (DoesRoomExists(room)) { i--; continue; }

            AddRoom(room);
            FloorPrefab.transform.localScale = new Vector3Int(room.xScale, 1, room.zScale);
            Instantiate(FloorPrefab, room.GetCenter(), Quaternion.identity, transform);

            RoomInformation existingRoom;
            foreach (RoomInformation value in RandomValues(dungeon).Take(1))
            {
                if (room == value) break;
                existingRoom = value;
                ConnectRooms(room, existingRoom);
            }
        }
    }

    public IEnumerable<RoomInformation> RandomValues<Vector3Int, RoomInformation>(IDictionary<Vector3Int, RoomInformation> dict)
    {
        System.Random rand = new System.Random();
        List<RoomInformation> values = Enumerable.ToList(dict.Values);
        int size = dict.Count;
        while (true)
        {
            yield return values[rand.Next(size)];
        }
    }


    public bool DoesRoomExists(RoomInformation room)
    {
        foreach (var existingRoom in dungeon)
        {
            if (DistanceBetweenTwoRooms(existingRoom.Value, room).x < ScaleBetweenRooms(existingRoom.Value, room).x + minDistanceBetweenRooms &&
                DistanceBetweenTwoRooms(existingRoom.Value, room).z < ScaleBetweenRooms(existingRoom.Value, room).z + minDistanceBetweenRooms)
                return true;
        }
        return false;
    }


    public Vector3Int DistanceBetweenTwoRooms(RoomInformation roomOne, RoomInformation roomTwo)
    {
        int distX = Mathf.Abs(roomOne.GetCenter().x - roomTwo.GetCenter().x);
        int distZ = Mathf.Abs(roomOne.GetCenter().z - roomTwo.GetCenter().z);
        return new Vector3Int(distX, 0, distZ);
    }


    public Vector3Int ScaleBetweenRooms(RoomInformation roomOne, RoomInformation roomTwo)
    {
        int scaleX = roomOne.xScale + roomTwo.xScale;
        int scaleZ = roomOne.zScale + roomTwo.zScale;
        return new Vector3Int(scaleX, 1, scaleZ);
    }


    public void ConnectRooms(RoomInformation roomOne, RoomInformation roomTwo)
    {
        Vector3Int roomOneCenter = roomOne.GetCenter();
        Vector3Int roomTwoCenter = roomTwo.GetCenter();

        float xMin = roomOneCenter.x < roomTwoCenter.x ? roomOneCenter.x + roomOne.xScale * 0.5f : roomOneCenter.x - roomOne.xScale * 0.5f;
        float xMax = roomTwoCenter.x;
        float xCenter = (xMin + xMax) * 0.5f;

        float xScale = Mathf.Abs(xMax - xMin);

        float zMin = roomTwoCenter.z < roomOneCenter.z ? roomTwoCenter.z + roomTwo.zScale * 0.5f : roomTwoCenter.z - roomTwo.zScale * 0.5f;
        float zMax = roomOneCenter.z;

        float zScale = Mathf.Abs(zMax - zMin);
        float zCenter = (zMin + zMax) * 0.5f;

        FloorPrefab.transform.localScale = new Vector3(xScale, 1, Random.Range(minHallWidth, maxHallWidth));
        Instantiate(FloorPrefab, new Vector3(xCenter, 0, zMin), Quaternion.identity, transform);

        FloorPrefab.transform.localScale = new Vector3(Random.Range(minHallWidth, maxHallWidth), 1, zScale);
        Instantiate(FloorPrefab, new Vector3(xMin, 0, zCenter), Quaternion.identity, transform);

        Debug.Log(xMin + ", " + zMin + "; " + xMax + ", " + zMax + ", " + xCenter + ", " + zCenter);
    }


    public void AddRoom(RoomInformation room)
    {
        dungeon.Add(room.GetCenter(), room);
    }
}



public class RoomInformation
{
    public int xMin, xMax, zMin, zMax, xScale, zScale;

    public RoomInformation(int xMin, int xMax, int zMin, int zMax, int xScale, int zScale)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.zMin = zMin;
        this.zMax = zMax;
        this.xScale = xScale;
        this.zScale = zScale;
    }


    public Vector3Int GetCenter()
    {
        return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(xMin, xMax, 0.5f)), 0, Mathf.RoundToInt(Mathf.Lerp(zMin, zMax, 0.5f)));
    }
}