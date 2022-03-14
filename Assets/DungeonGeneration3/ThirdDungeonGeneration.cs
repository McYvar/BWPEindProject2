using UnityEngine;
using System.Collections.Generic;

public class ThirdDungeonGeneration : MonoBehaviour
{
    RoomInformation room;

    [SerializeField] int gridWidth = 100;
    [SerializeField] int gridHeight = 100;

    [SerializeField] int roomSizeMin = 3;
    [SerializeField] int roomSizeMax = 7;

    [SerializeField] int numRooms = 10;

    [SerializeField] int minDistanceBetweenRooms = 1;

    [SerializeField] Dictionary<Vector3Int, Room> dungeon = new Dictionary<Vector3Int, Room>();

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
            int zScale = zMin - zMax;

            RoomInformation room = new RoomInformation(xMin, xMax, zMin, zMax);
            if (DoesRoomExists(room)) i--;

            FloorPrefab.transform.localScale = new Vector3(xScale, 1, zScale);
            Instantiate(FloorPrefab, room.GetCenter(), Quaternion.identity, transform);
        }
    }


    public bool DoesRoomExists(RoomInformation room)
    {
        if (dungeon.ContainsKey(room.GetCenter())) return true;
        return false;
    }
}



public class RoomInformation
{
    public int xMin, xMax, zMin, zMax;

    public RoomInformation(int xMin, int xMax, int zMin, int zMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.zMin = zMin;
        this.zMax = zMax;
    }


    public Vector3Int GetCenter()
    {
        return new Vector3Int(Mathf.RoundToInt(Mathf.Lerp(xMin, xMax, 0.5f)), 0, Mathf.RoundToInt(Mathf.Lerp(zMin, zMax, 0.5f)));
    }
}

