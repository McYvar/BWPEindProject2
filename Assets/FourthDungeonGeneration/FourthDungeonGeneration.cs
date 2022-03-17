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
        room.CreateRoomWall(room.GetRoomLowerLeft(), room.GetRoomUpperLeft(), false, 3f);
        room.CreateRoomWall(room.GetRoomUpperLeft(), room.GetRoomUpperRight(), false, 1f);
        room.CreateRoomWall(room.GetRoomLowerRight(), room.GetRoomUpperRight(), false, 3f);
        room.CreateRoomWall(room.GetRoomLowerLeft(), room.GetRoomLowerRight(), true, 3f);
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

        CreateObjectInstructions(roomObject ,vertices, triangles);
    }

    public void CreateRoomWall(Vector3 pointA, Vector3 pointB, bool hasHall, float wallHeigt)
    {
        Vector3[] vertices = new Vector3[8];
        Vector3[] verticesIfHall = new Vector3[8];
        string name;

        if (pointA.z == pointB.z)  // Horizontal wall
        {
            if (pointA.z > GetRoomCenter().z) // Upper wall
            {
                Debug.Log("Upper wall");
                if (hasHall) 
                {
                    Vector3[] tempVertLeftWall = new Vector3[] { pointA + Vector3.back,                            pointA,                            GetRoomCenterUp() + Vector3.left + Vector3.back,                            GetRoomCenterUp() + Vector3.left,
                                                                 pointA + Vector3.back + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), GetRoomCenterUp() + Vector3.left + Vector3.back + (Vector3.up * wallHeigt), GetRoomCenterUp() + Vector3.left + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertRightWall = new Vector3[] { GetRoomCenterUp() + Vector3.right + Vector3.back,                            GetRoomCenterUp() + Vector3.right,                            pointB + Vector3.back,                            pointB,
                                                                  GetRoomCenterUp() + Vector3.right + Vector3.back + (Vector3.up * wallHeigt), GetRoomCenterUp() + Vector3.right + (Vector3.up * wallHeigt), pointB + Vector3.back + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt)} ;
                    vertices = tempVertLeftWall;
                    verticesIfHall = tempVertRightWall;
                }
                else
                {
                    Vector3[] tempVert = new Vector3[] { pointA + Vector3.back,                            pointA,                            pointB + Vector3.back,                            pointB,
                                                         pointA + Vector3.back + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), pointB + Vector3.back + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                    verticesIfHall = null;
                }
                name = "Upper";
            }
            else // Lower wall
            {
                Debug.Log("Lower wall");
                if (hasHall)
                {
                    Vector3[] tempVertLeftWall = new Vector3[] { pointA,                            pointA + Vector3.forward,                            GetRoomCenterDown() + Vector3.left,                            GetRoomCenterDown() + Vector3.left + Vector3.forward,
                                                                 pointA + (Vector3.up * wallHeigt), pointA + Vector3.forward + (Vector3.up * wallHeigt), GetRoomCenterDown() + Vector3.left + (Vector3.up * wallHeigt), GetRoomCenterDown() + Vector3.left + Vector3.forward + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertRightWall = new Vector3[] { GetRoomCenterDown() + Vector3.right,                            GetRoomCenterDown() + Vector3.right + Vector3.forward,                            pointB,                            pointB + Vector3.forward,
                                                                  GetRoomCenterDown() + Vector3.right + (Vector3.up * wallHeigt), GetRoomCenterDown() + Vector3.right + Vector3.forward + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt), pointB + Vector3.forward + (Vector3.up * wallHeigt)};
                    vertices = tempVertLeftWall;
                    verticesIfHall = tempVertRightWall;
                }
                else
                {
                    Vector3[] tempVert = new Vector3[] { pointA,                            pointA + Vector3.forward,                            pointB,                            pointB + Vector3.forward,
                                                         pointA + (Vector3.up * wallHeigt), pointA + Vector3.forward + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt), pointB + Vector3.forward + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                    verticesIfHall = null;
                }
                name = "Lower";
            }
        }
        else // Vertical wall
        {
            if (pointA.x < GetRoomCenter().x) // Left wall
            {
                Debug.Log("Left wall");
                if (hasHall)
                {
                    Vector3[] tempVertLowerWall = new Vector3[] { pointA,                            GetRoomCenterLeft() + Vector3.back,                            pointA + Vector3.right,                            GetRoomCenterLeft() + Vector3.back + Vector3.right,
                                                                  pointA + (Vector3.up * wallHeigt), GetRoomCenterLeft() + Vector3.back + (Vector3.up * wallHeigt), pointA + Vector3.right + (Vector3.up * wallHeigt), GetRoomCenterLeft() + Vector3.back + Vector3.right + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertUpperWall = new Vector3[] { GetRoomCenterLeft() + Vector3.forward,                            pointB + Vector3.right,            GetRoomCenterLeft() + Vector3.forward + Vector3.right,                            pointB,            
                                                                  GetRoomCenterLeft() + Vector3.forward + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt), GetRoomCenterLeft() + Vector3.forward + Vector3.right + (Vector3.up * wallHeigt), pointB + Vector3.right + (Vector3.up * wallHeigt)};
                    vertices = tempVertLowerWall;
                    verticesIfHall = tempVertUpperWall;
                }
                else
                {
                    Vector3[] tempVert = new Vector3[] { pointA,                            pointB,                            pointA + Vector3.right,                            pointB + Vector3.right,
                                                         pointA + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt), pointA + Vector3.right + (Vector3.up * wallHeigt), pointB + Vector3.right + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                    verticesIfHall = null;
                }
                name = "Left";
            }
            else // Right wall
            {
                Debug.Log("Right wall");
                if (hasHall)
                {
                    Vector3[] tempVertLowerWall = new Vector3[] { pointA - Vector3.left,                            GetRoomCenterRight() + Vector3.back + Vector3.left,                            pointA,                            GetRoomCenterRight() + Vector3.back,
                                                                  pointA - Vector3.left + (Vector3.up * wallHeigt), GetRoomCenterRight() + Vector3.back + Vector3.left + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), GetRoomCenterRight() + Vector3.back + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertUpperWall = new Vector3[] { GetRoomCenterRight() + Vector3.forward + Vector3.left,                            pointB + Vector3.left,                            GetRoomCenterRight() + Vector3.forward,                            pointB,
                                                                  GetRoomCenterRight() + Vector3.forward + Vector3.left + (Vector3.up * wallHeigt), pointB + Vector3.left + (Vector3.up * wallHeigt), GetRoomCenterRight() + Vector3.forward + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt) };
                    vertices = tempVertLowerWall;
                    verticesIfHall = tempVertUpperWall;
                }
                else
                {
                    Vector3[] tempVert = new Vector3[] { pointA + Vector3.left,                            pointB + Vector3.left,                            pointA,                            pointB,
                                                         pointA + Vector3.left + (Vector3.up * wallHeigt), pointB + Vector3.left + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                    verticesIfHall = null;
                }
                name = "Right";
            }
        }

        GameObject obj = new GameObject(name + " wall");
        obj.layer = 3;
        obj.transform.parent = roomObject.transform;
        int[] triangles = { 2, 1, 0, 3, 1, 2, 3, 5, 1, 7, 5, 3, 6, 3, 2, 7, 3, 6, 6, 0, 4, 2, 0, 6, 0, 5, 4, 1, 5, 0, 7, 4, 5, 6, 4, 7 };
        CreateObjectInstructions(obj, vertices, triangles);
        if (hasHall)
        {
            GameObject objTwo = new GameObject(name + " wall");
            objTwo.layer = 3;
            objTwo.transform.parent = roomObject.transform;
            CreateObjectInstructions(objTwo, verticesIfHall, triangles);
        }
    }

    private void CreateObjectInstructions(GameObject obj, Vector3[] vertices, int[] triangles)
    {
        MeshFilter mesh = obj.AddComponent<MeshFilter>();
        mesh.mesh.vertices = vertices;
        mesh.mesh.triangles = triangles;
        mesh.mesh.RecalculateNormals();

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        mesh.mesh.uv = uvs;

        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        mr.material = floorMat;

        BoxCollider boxCollider = obj.AddComponent<BoxCollider>();
    }
}