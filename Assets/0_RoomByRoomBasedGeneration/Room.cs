using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int width;

    public int height;
    public int x;
    public int y;

    private void Start()
    {
        if (RoomController.instance == null)
        {
            Debug.Log("Load room in different scene");
            return;
        }

        RoomController.instance.RegisterRoom(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 0, height));
    }

    public Vector3 GetRoomCenter()
    {
        return new Vector3(x * width, 0, y * height);
    }
}
