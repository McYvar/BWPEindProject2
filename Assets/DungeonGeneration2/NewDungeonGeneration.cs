using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NewDungeonGeneration {

    public enum TileType { Floor, Wall }



    public class NewDungeonGeneration : MonoBehaviour
    {
        [SerializeField] Dictionary<Vector3Int, TileType> dungeon = new Dictionary<Vector3Int, TileType>();
        [SerializeField] List<Room> roomsList = new List<Room>();

        [SerializeField] int gridWidth = 100;
        [SerializeField] int gridHeight = 100;

        [SerializeField] int roomSizeMin = 3;
        [SerializeField] int roomSizeMax = 7;

        [SerializeField] int minDistanceBetweenRooms = 1;

        [SerializeField] int numRooms = 10;

        [SerializeField] GameObject floorPrefab;
        [SerializeField] GameObject wallPrefab;

        [SerializeField] GameObject playerPrefab;

        private void Start()
        {
            Generate();

            SpawnPlayer();
        }

        
        public void SpawnPlayer()
        {
            Instantiate(playerPrefab, roomsList[0].GetCenter() + Vector3.up, Quaternion.identity);
        }


        public void Generate()
        {
            for (int i = 0; i < numRooms; i++)
            {
                int xMin = Random.Range(0, gridWidth);
                int xMax = xMin + Random.Range(roomSizeMin, roomSizeMax + 1);
                int zMin = Random.Range(0, gridHeight);
                int zMax = zMin + Random.Range(roomSizeMin, roomSizeMax + 1);

                Room room = new Room(xMin, xMax, zMin, zMax);
                if (!DoesRoomExists(room)) AddRoomToDungeon(room);
                else i--;
            }

            for (int i = 0; i < roomsList.Count; i++)
            {
                Room room = roomsList[i];
                Room otherRoom = roomsList[(i + Random.Range(1, roomsList.Count)) % roomsList.Count];

                ConnectRooms(room, otherRoom);
            }

            AllocateWalls();

            SpawnDungeon();
        }


        public bool DoesRoomExists(Room room)
        {
            for (int x = room.xMin - minDistanceBetweenRooms; x <= room.xMax + minDistanceBetweenRooms; x++)
            {
                for (int z = room.zMin - minDistanceBetweenRooms; z <= room.zMax + minDistanceBetweenRooms; z++)
                {
                    if (dungeon.ContainsKey(new Vector3Int(x, 0, z))) return true;
                }
            }
            return false;
        }


        public void AddRoomToDungeon(Room room)
        {
            for (int x = room.xMin; x <= room.xMax; x++)
            {
                for (int z = room.zMin; z <= room.zMax; z++)
                {
                    dungeon.Add(new Vector3Int(x, 0, z), TileType.Floor);
                }
            }
            roomsList.Add(room);
        }


        public void SpawnDungeon()
        {
            foreach (var kv in dungeon)
            {
                switch (kv.Value)
                {
                    case TileType.Floor: Instantiate(floorPrefab, kv.Key, Quaternion.identity, transform); break;
                    case TileType.Wall: Instantiate(wallPrefab, kv.Key, Quaternion.identity, transform); break;
                }
            }
        }


        public void ConnectRooms(Room roomOne, Room roomTwo)
        {
            Vector3Int posOne = roomOne.GetCenter();
            Vector3Int posTwo = roomTwo.GetCenter();
            int dirX = posTwo.x > posOne.x ? 1 : -1;
            int x = 0;
            for (x = posOne.x; x != posTwo.x; x += dirX)
            {
                Vector3Int position = new Vector3Int(x, 0, posOne.z);
                if (dungeon.ContainsKey(position)) continue;
                dungeon.Add(position, TileType.Floor);
            }

            int dirZ = posTwo.z > posOne.z ? 1 : -1;
            for (int z = posOne.z; z != posTwo.z; z += dirZ)
            {
                Vector3Int position = new Vector3Int(x, 0, z);
                if (dungeon.ContainsKey(position)) continue;
                dungeon.Add(position, TileType.Floor);
            }
        }


        public void AllocateWalls()
        {
            var keys = dungeon.Keys.ToList();
            foreach(var kv in keys)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        if (Mathf.Abs(x) == Mathf.Abs(z)) continue;
                        Vector3Int newPos = kv + new Vector3Int(x, 0, z);
                        if (dungeon.ContainsKey(newPos)) continue;
                        dungeon.Add(newPos, TileType.Wall);
                    }
                }
            }
        }

    }



    public class Room
    {
        public int xMin, xMax, zMin, zMax;

        public Room(int xMin, int xMax, int zMin, int zMax)
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
}
