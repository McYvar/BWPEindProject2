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
    [SerializeField] int maxDistanceToCreateHall = 10;

    [SerializeField] int minHallWidth;
    [SerializeField] int maxHallWidth;

    [SerializeField] Dictionary<Vector3Int, RoomInformation> dungeon = new Dictionary<Vector3Int, RoomInformation>();

    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject playerPrefab;

    int preventInfiniteLoop;

    private void Start()
    {
        Generate();

        SummonPlayer();
    }

    public void SummonPlayer()
    {
        RoomInformation anyRoom = RandomValues(dungeon).First();
        Instantiate(playerPrefab, anyRoom.GetCenter() + Vector3.up * 5, Quaternion.identity);
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

    public void Generate()
    {
        for (int i = 0; i < numRooms && preventInfiniteLoop < 50; i++)
        {
            int xMin = Random.Range(0, gridWidth);
            int xMax = xMin + Random.Range(roomSizeMin, roomSizeMax + 1);
            int zMin = Random.Range(0, gridHeight);
            int zMax = zMin + Random.Range(roomSizeMin, roomSizeMax + 1);

            int xScale = xMax - xMin;
            int zScale = zMax - zMin;

            RoomInformation room = new RoomInformation(xMin, xMax, zMin, zMax, xScale, zScale);
            if (DoesRoomExists(room)) { i--; preventInfiniteLoop++; continue; }

            AddRoom(room);
            floorPrefab.transform.localScale = new Vector3Int(room.xScale, 1, room.zScale);
            Instantiate(floorPrefab, room.GetCenter(), Quaternion.identity, transform);

            foreach (var existingRooms in dungeon)
            {
                if (Vector3.Distance(existingRooms.Key, room.GetCenter()) < maxDistanceToCreateHall)
                    ConnectRooms(existingRooms.Value, room);
            }
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

        int width = Random.Range(minHallWidth, maxHallWidth);
        float xMin = roomOneCenter.x < roomTwoCenter.x ? roomOneCenter.x + roomOne.xScale * 0.5f : roomOneCenter.x - roomOne.xScale * 0.5f;
        float xMax = roomOneCenter.x < roomTwoCenter.x ? roomTwoCenter.x + width * 0.5f : roomTwoCenter.x - width * 0.5f;
        float xCenter = (xMin + xMax) * 0.5f;
        Debug.Log("(" + xMin + " + " + xMax + ") / 2 = " + xCenter);
        float xScale = Mathf.Abs(xMax - xMin);

        float zMin = roomTwoCenter.z < roomOneCenter.z ? roomTwoCenter.z + roomTwo.zScale * 0.5f : roomTwoCenter.z - roomTwo.zScale * 0.5f;
        float zMax = roomTwoCenter.z < roomOneCenter.z ? roomOneCenter.z + width * 0.5f : roomOneCenter.z - width * 0.5f;

        float zCenter = (zMin + zMax) * 0.5f;
        Debug.Log("(" + zMin + " + " + zMax + ") / 2 = " + zCenter);
        float zScale = Mathf.Abs(zMax - zMin);

        floorPrefab.transform.localScale = new Vector3(xScale, 1, width);
        Instantiate(floorPrefab, new Vector3(xCenter, 0, roomOneCenter.z), Quaternion.identity, transform);

        floorPrefab.transform.localScale = new Vector3(width, 1, zScale);
        Instantiate(floorPrefab, new Vector3(roomTwoCenter.x, 0, zCenter), Quaternion.identity, transform);

        //Debug.Log(xMin + ", " + zMin + "; " + xMax + ", " + zMax + ", " + xCenter + ", " + zCenter);
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