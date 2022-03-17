using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonGenerator : MonoBehaviour
{
    public DungeonGenerationData dungeonGenerationData;
    private List<Vector2Int> dungeonRooms;
    public GameObject player;


    private void Start()
    {
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGenerationData);
        SpawnRooms(dungeonRooms);

        Instantiate(player, Vector3.one * 0.5f + Vector3.up * 5, Quaternion.identity);
    }

    private void SpawnRooms(List<Vector2Int> rooms)
    {
        RoomController.instance.LoadRoom("Start", 0, 0);
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i < rooms.Count - 1)
            {
                int r = Random.Range(0, GetBasementScenes());
                RoomController.instance.LoadRoom("Room" + r, rooms[i].x, rooms[i].y);
            }
            else RoomController.instance.LoadRoom("End", rooms[i].x, rooms[i].y);
        }
    }

    int GetBasementScenes()
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
        }

        sceneCount = 0;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].Contains("BasementRoom")) sceneCount++;
        }
        return sceneCount;
    }
}
