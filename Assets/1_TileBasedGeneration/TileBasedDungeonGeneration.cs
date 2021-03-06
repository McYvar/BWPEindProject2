using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TileBasedDungeonGeneration {

    public enum TileType { Floor, Wall }

    public class TileBasedDungeonGeneration : MonoBehaviour
    {
        [SerializeField] Dictionary<Vector3Int, TileType> dungeon = new Dictionary<Vector3Int, TileType>();
        List<Room> roomsList = new List<Room>();

        public int gridWidth = 100;
        public int gridHeight = 100;
        public Vector2 GetGrid() { return new Vector2(gridWidth, gridHeight); }

        [SerializeField] int roomSizeMin = 3;
        [SerializeField] int roomSizeMax = 7;

        [SerializeField] int minDistanceBetweenRooms = 1;

        [SerializeField] int numRooms = 10;

        [SerializeField] GameObject floorPrefab;
        [SerializeField] GameObject borderPrefab;
        [SerializeField] GameObject lightPrefab;

        [SerializeField] GameObject floorMapout;
        [SerializeField] GameObject borderMapout;
        [SerializeField] List<GameObject> objectsToCombine;

        [SerializeField] GameObject playerPrefab;

        [SerializeField] GameObject[] enemies;
        [SerializeField] int maxEnemiesPerRoom;
        public static Queue<Enemy> enemyQueue;

        [SerializeField] GameObject[] itemsPrefabs;
        [SerializeField] GameObject[] spellsPrefabs;
        [Range(0, 100)] [SerializeField] float itemSpawnChance;

        MenuSystem menuSystem;

        int preventInfiniteLoop = 5000;

        private void Start()
        {
            Time.timeScale = 1;

            enemyQueue = new Queue<Enemy>();

            Generate();

            foreach (GameObject obj in objectsToCombine) CombineMeshes(obj);

            SpawnObjects();

            SpawnPlayer();

            menuSystem = GetComponent<MenuSystem>();

            Invoke("WaitForLoad", 1);
        }

        private void OnEnable()
        {
            EventManager<Vector3>.AddListener(EventType.ON_ENEMY_DEATH_SPAWN_ITEM, SpawnItems);
            EventManager<Vector3>.AddListener(EventType.ON_ENEMY_DEATH_SPAWN_SPELL, SpawnSpells);
        }

        private void OnDisable()
        {
            EventManager<Vector3>.RemoveListener(EventType.ON_ENEMY_DEATH_SPAWN_ITEM, SpawnItems);
            EventManager<Vector3>.RemoveListener(EventType.ON_ENEMY_DEATH_SPAWN_SPELL, SpawnSpells);
        }

        void WaitForLoad()
        {
            menuSystem.enabled = true;
        }


        public void Generate()
        {
            for (int i = 0; i < numRooms && preventInfiniteLoop > 0; i++)
            {
                int xMin = Random.Range(0, gridWidth);
                int xMax = xMin + Random.Range(roomSizeMin, roomSizeMax + 1);
                int zMin = Random.Range(0, gridHeight);
                int zMax = zMin + Random.Range(roomSizeMin, roomSizeMax + 1);

                Room room = new Room(xMin, xMax, zMin, zMax);
                if (!DoesRoomExists(room)) AddRoomToDungeon(room);
                else { i--; preventInfiniteLoop--; }
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
        

        void SpawnObjects()
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                Room room = roomsList[i];
                SpawnEnemies(room);

                //SpawnLights(room);

                SpawnItems(room.GetRandomLocation());
            }
        }


        void SpawnItems(Vector3 location)
        {
            int r = Random.Range(0, itemsPrefabs.Length);
            Instantiate(itemsPrefabs[r], location + Vector3.up * 0.51f, itemsPrefabs[r].transform.rotation, transform);
        }


        void SpawnSpells(Vector3 location)
        {
            int r = Random.Range(0, spellsPrefabs.Length);
            Instantiate(spellsPrefabs[r], location + Vector3.up * 0.51f, spellsPrefabs[r].transform.rotation, transform);
        }


        void SpawnLights(Room room)
        {
            Instantiate(lightPrefab, room.GetCenter() + transform.up * 30, lightPrefab.transform.rotation, transform);
        }


        void SpawnEnemies(Room room)
        {
            int amount = Random.Range(1, maxEnemiesPerRoom + 1);
            for (int i = 0; i < amount; i++)
            {
                GameObject enemy = Instantiate(enemies[Random.Range(0, enemies.Count())], room.GetRandomLocation() + Vector3.up * 5, Quaternion.identity, transform);
                enemy.name = "Enemy" + enemyQueue.Count;
                enemyQueue.Enqueue(enemy.GetComponent<Enemy>());
            }
        }


        void SpawnPlayer()
        {
            GameObject playerHolder = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            GameObject player = FindObjectOfType<Player>().gameObject;
            player.transform.position = roomsList[0].GetRandomLocation() + Vector3.up * 100;
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
                    case TileType.Floor:
                        Instantiate(floorPrefab, kv.Key, Quaternion.identity, floorMapout.transform);
                        break;
                    case TileType.Wall: Instantiate(borderPrefab, kv.Key + Vector3.up, Quaternion.identity, borderMapout.transform); break;
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


        public void CombineMeshes(GameObject obj)
        {
            //Temporarily set position to zero to make matrix math easier
            Vector3 position = obj.transform.position;
            obj.transform.position = Vector3.zero;

            //Get all mesh filters and combine
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 1;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }

            obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
            obj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
            obj.transform.gameObject.SetActive(true);

            //Return to original position
            obj.transform.position = position;

            obj.AddComponent<MeshCollider>();
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


        public Vector3Int GetRandomLocation()
        {
            return new Vector3Int(Random.Range(xMin, xMax), 0, Random.Range(zMin, zMax));
        }
    }
}
