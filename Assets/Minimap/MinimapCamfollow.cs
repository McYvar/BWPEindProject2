using UnityEngine;

public class MinimapCamfollow : MonoBehaviour
{
    [SerializeField] bool followPlayer;

    TileBasedDungeonGeneration.TileBasedDungeonGeneration generation;
    GameObject player;
    bool foundPlayer;

    private void Awake()
    {
        generation = FindObjectOfType<TileBasedDungeonGeneration.TileBasedDungeonGeneration>();
    }

    private void LateUpdate()
    {
        if (!foundPlayer)
        {
            player = FindObjectOfType<Player>().gameObject;
            if (player == null) return;
            else foundPlayer = true;
        }

        if (followPlayer)
        {
            transform.position = new Vector3(player.transform.position.x, 20, player.transform.position.z);
        }
        else
        {
            float x = generation.gridWidth/2;
            float z = generation.gridHeight/2;
            float size = Vector3.Distance(Vector3.zero, new Vector3(x, 0, z));
            gameObject.GetComponent<Camera>().orthographicSize = size + 3;
            transform.position = new Vector3(x, 100, z);
        }

    }
}
