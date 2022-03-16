using System.Collections.Generic;
using System.Collections;
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
    [SerializeField] int maxAmountOfConnectedHalls;

    [SerializeField] Dictionary<Vector3Int, RoomInformation> dungeon = new Dictionary<Vector3Int, RoomInformation>();
    
    [SerializeField] GameObject roomFloorPrefab;
    [SerializeField] GameObject hallFloorPrefabVert;
    [SerializeField] GameObject hallFloorPrefabHori;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] GameObject floater;

    int preventInfiniteLoop;

    private void Start()
    {
        Generate();

        SummonPlayer();
    }

    public void SummonPlayer()
    {
        RoomInformation anyRoom = RandomValues(dungeon).First();
        Instantiate(playerPrefab, anyRoom.GetCenter() + new Vector3(0.5f, 5, -0.5f), Quaternion.identity);
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
        for (int i = 0; i < numRooms && preventInfiniteLoop < 100; i++)
        {
            int xMin = makeEven(Random.Range(0, gridWidth));
            int xMax = xMin + makeEven(Random.Range(roomSizeMin, roomSizeMax + 1));
            int zMin = makeEven(Random.Range(0, gridHeight));
            int zMax = zMin + makeEven(Random.Range(roomSizeMin, roomSizeMax + 1));

            int xScale = xMax - xMin;
            int zScale = zMax - zMin;

            RoomInformation room = new RoomInformation(xMin, xMax, zMin, zMax, xScale, zScale);
            if (DoesRoomExists(room)) { i--; preventInfiniteLoop++; continue; }

            AddRoom(room);

            CreateHalls(room);
        }
    }

    private void CreateHalls(RoomInformation room)
    {
        foreach (var existingRooms in dungeon)
        {
            if (existingRooms.Value == room) continue;
            if (Vector3.Distance(existingRooms.Key, room.GetCenter()) < maxDistanceToCreateHall && room.getHallsAmount() < maxAmountOfConnectedHalls)
            {
                ConnectRooms(existingRooms.Value, room);
                room.setHallsAmount(room.getHallsAmount() + 1);
            }
        }
    }

    public int makeEven(float input)
    {
        if (input % 2 == 0) return (int) input;
        else return (int) input + 1;
    }


    public bool DoesRoomExists(RoomInformation room)
    {
        foreach (var existingRoom in dungeon)
        {
            if (DistanceBetweenTwoRooms(existingRoom.Value, room).x < ScaleBetweenTwoRooms(existingRoom.Value, room).x + minDistanceBetweenRooms &&
                DistanceBetweenTwoRooms(existingRoom.Value, room).z < ScaleBetweenTwoRooms(existingRoom.Value, room).z + minDistanceBetweenRooms)
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


    public Vector3Int ScaleBetweenTwoRooms(RoomInformation roomOne, RoomInformation roomTwo)
    {
        int scaleX = roomOne.xScale + roomTwo.xScale;
        int scaleZ = roomOne.zScale + roomTwo.zScale;
        return new Vector3Int(scaleX, 1, scaleZ);
    }


    public void ConnectRooms(RoomInformation existingRoom, RoomInformation currentRoom)
    {
        Vector3Int roomOneCenter = existingRoom.GetCenter();
        Vector3Int roomTwoCenter = currentRoom.GetCenter();

        int width = makeEven(Random.Range(minHallWidth, maxHallWidth));
        float xMin = roomOneCenter.x < roomTwoCenter.x ? roomOneCenter.x + existingRoom.xScale * 0.5f : roomOneCenter.x - existingRoom.xScale * 0.5f;
        float xMax = roomOneCenter.x < roomTwoCenter.x ? roomTwoCenter.x + width * 0.5f : roomTwoCenter.x - width * 0.5f;
        float xCenter = (xMin + xMax) * 0.5f;
        float xScale = Mathf.Abs(xMax - xMin);

        float zMin = roomTwoCenter.z < roomOneCenter.z ? roomTwoCenter.z + currentRoom.zScale * 0.5f : roomTwoCenter.z - currentRoom.zScale * 0.5f;
        float zMax = roomTwoCenter.z < roomOneCenter.z ? roomOneCenter.z + width * 0.5f : roomOneCenter.z - width * 0.5f;

        float zCenter = (zMin + zMax) * 0.5f;
        float zScale = Mathf.Abs(zMax - zMin);

        if (xScale > 2)
        {
            hallFloorPrefabHori.transform.localScale = new Vector3(xScale - 0.01f, 0.99f, width - 0.01f);
            currentRoom.halls.Add(Instantiate(createNewObjWithTextureTiling(hallFloorPrefabHori), new Vector3(xCenter, 0, roomOneCenter.z), Quaternion.identity, transform));
        }

        if (zScale > 2)
        {
            hallFloorPrefabVert.transform.localScale = new Vector3(width - 0.01f, 0.99f, zScale - 0.01f);
            currentRoom.halls.Add(Instantiate(createNewObjWithTextureTiling(hallFloorPrefabVert), new Vector3(roomTwoCenter.x, 0, zCenter), Quaternion.identity, transform));
        }

        foreach (GameObject obj in currentRoom.halls)
        {
            obj.transform.parent = currentRoom.roomObj.transform;
        }
    }


    public GameObject createNewObjWithTextureTiling(GameObject obj)
    {
        GameObject temp;
        temp = obj;
        Texture tex = obj.GetComponent<Renderer>().material.mainTexture;
        temp.GetComponent<MeshRenderer>().material = null;
        temp.GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(temp.transform.localScale.x, temp.transform.localScale.z);
        temp.GetComponent<MeshRenderer>().material.mainTexture = tex;
        return temp;
    }


    public void AddRoom(RoomInformation room)
    {
        roomFloorPrefab.transform.localScale = new Vector3Int(room.xScale, 1, room.zScale);
        room.roomObj = Instantiate(roomFloorPrefab, room.GetCenter(), Quaternion.identity, transform);
        room.roomObj.name = "Room" + dungeon.Count;
        dungeon.Add(room.GetCenter(), room);
    }


    public void CreateWalls()
    {

    }
}



public class RoomInformation
{
    public int xMin, xMax, zMin, zMax, xScale, zScale;
    int hallsAmount;
    public GameObject roomObj;
    public List<GameObject> halls;

    public RoomInformation(int xMin, int xMax, int zMin, int zMax, int xScale, int zScale)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.zMin = zMin;
        this.zMax = zMax;
        this.xScale = xScale;
        this.zScale = zScale;
        hallsAmount = 0;
        roomObj = new GameObject();
        halls = new List<GameObject>();
    }


    public int getHallsAmount()
    {
        return hallsAmount;
    }


    public void setHallsAmount(int amount)
    {
        hallsAmount = amount;
    }


    public Vector3Int GetCenter()
    {
        return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(xMin, xMax, 0.5f)), 0, Mathf.RoundToInt(Mathf.Lerp(zMin, zMax, 0.5f)));
    }
}