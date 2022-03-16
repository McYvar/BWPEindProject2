using System.Collections.Generic;
using UnityEngine;

public class FourthDungeonGeneration : MonoBehaviour
{
    Dictionary<int, GameObject> dungeon = new Dictionary<int, GameObject>();
    [SerializeField] Material mat;

    private void Start()
    {
        Rooms room = new Rooms(new Vector3(0, 0, 0), new Vector3(0, 0, 15), new Vector3(15, 0, 0), new Vector3(15, 0, 15), dungeon.Count, mat);
        room.CreateRoomTile();
    }


    void AddRoom(GameObject obj)
    {
        dungeon.Add(dungeon.Count, obj);
        Debug.Log(dungeon[dungeon.Count - 1] + " has been added!");
    }
}



public class Rooms
{
    Vector3 xMinZMin;
    Vector3 xMinZMax;
    Vector3 xMaxZMin;
    Vector3 xMaxZMax;
    int name;
    Material floorMat;

    GameObject roomObject;

    public Rooms(Vector3 xMinZMin, Vector3 xMinZMax, Vector3 xMaxZMin, Vector3 xMaxZMax, int name, Material floorMat)
    {
        this.xMinZMin = xMinZMin;
        this.xMinZMax = xMinZMax;
        this.xMaxZMin = xMaxZMin;
        this.xMaxZMax = xMaxZMax;
        this.name = name;
        this.floorMat = floorMat;
    }


    public Vector3 GetRoomCenter()
    {
        return new Vector3((xMinZMin.x + xMaxZMax.x) * 0.5f, 0, (xMinZMin.z + xMaxZMax.z) * 0.5f);
    }


    public Vector3 GetRoomCenterLeft()
    {
        return new Vector3((xMinZMin.x + xMinZMax.x) / 2, 0, (xMinZMin.z + xMinZMax.z) / 2);
    }


    public Vector3 GetRoomCenterUp()
    {
        return new Vector3((xMinZMax.x + xMaxZMax.x) / 2, 0, (xMinZMax.z + xMaxZMax.z) / 2);
    }


    public Vector3 GetRoomCenterRight()
    {
        return new Vector3((xMaxZMin.x + xMaxZMax.x) / 2, 0, (xMaxZMin.z + xMaxZMax.z) / 2);
    }


    public Vector3 GetRoomCenterDown()
    {
        return new Vector3((xMinZMin.x + xMaxZMin.x) / 2, 0, (xMinZMin.z + xMaxZMin.z) / 2);
    }


    public Vector3 GetRoomLowerLeft()
    {
        return xMinZMin;
    }


    public Vector3 GetRoomUpperLeft()
    {
        return xMinZMax;
    }


    public Vector3 GetRoomUpperRight()
    {
        return xMaxZMax;
    }


    public Vector3 GetRoomLowerRight()
    {
        return xMaxZMin;
    }


    public void CreateRoomTile()
    {
        roomObject = new GameObject("RoomTile(" + name + ")");
        roomObject.layer = 6;


        Vector3[] vertices = new Vector3[] { GetRoomLowerLeft(), GetRoomUpperLeft(), GetRoomLowerRight(), GetRoomUpperRight() };
        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };

        CreateObjectInstructions(vertices, triangles);
    }

    public void CreateRoomWall(Vector3 pointA, Vector3 pointB, bool hasHall, float wallHeigt)
    {
        Vector3[] vertices;
        Vector3[] verticesIfHall;

        if (pointA.x == pointB.x)  // Horizontal wall
        {
            if (pointA.y > GetRoomCenter().y) // Upper wall
            {
                if (hasHall) 
                {
                    Vector3[] tempVertLeftWall = { pointA + Vector3.back,                            pointA,                            GetRoomCenterUp() + Vector3.left + Vector3.back,                            GetRoomCenterUp() + Vector3.left,
                                                   pointA + Vector3.back + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), GetRoomCenterUp() + Vector3.left + Vector3.back + (Vector3.up * wallHeigt), GetRoomCenterUp() + Vector3.left + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertRightWall = { GetRoomCenterUp() + Vector3.right + Vector3.back,                            GetRoomCenterUp() + Vector3.right,                            pointB + Vector3.back,                            pointB,
                                                    GetRoomCenterUp() + Vector3.right + Vector3.back + (Vector3.up * wallHeigt), GetRoomCenterUp() + Vector3.right + (Vector3.up * wallHeigt), pointB + Vector3.back + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt)} ;
                }
                else
                {
                    Vector3[] tempVert = { pointA + Vector3.back,                            pointA,                            pointB + Vector3.back,                            pointB,
                                           pointA + Vector3.back + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), pointB + Vector3.back + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                }
            }
            else // Lower wall
            {

            }
        }
        else // Vertical wall
        {
            if (pointA.x < GetRoomCenter().x) // Left wall
            {

            }
            else // Right wall
            {

            }
        }

        int[] triangles;

    }

    private void CreateObjectInstructions(Vector3[] vertices, int[] triangles)
    {
        MeshFilter mesh = roomObject.AddComponent<MeshFilter>();
        mesh.mesh.vertices = vertices;
        mesh.mesh.triangles = triangles;
        mesh.mesh.RecalculateNormals();

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        mesh.mesh.uv = uvs;

        MeshRenderer mr = roomObject.AddComponent<MeshRenderer>();
        mr.material = floorMat;

        BoxCollider boxCollider = roomObject.AddComponent<BoxCollider>();
    }
}