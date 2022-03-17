using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rooms
{
    Vector3 xMinZMin;
    Vector3 xMinZMax;
    Vector3 xMaxZMin;
    Vector3 xMaxZMax;
    int name;
    Material floorMat;
    bool seeTroughMode;

    GameObject roomObject;

    public List<Vector3> doorLocations = new List<Vector3>();

    public Rooms(Vector3 xMinZMin, Vector3 xMinZMax, Vector3 xMaxZMin, Vector3 xMaxZMax, int name, Material floorMat, bool seeTroughMode)
    {
        this.xMinZMin = xMinZMin;
        this.xMinZMax = xMinZMax;
        this.xMaxZMin = xMaxZMin;
        this.xMaxZMax = xMaxZMax;
        this.name = name;
        this.floorMat = floorMat;
        this.seeTroughMode = seeTroughMode;
    }

    public void AddDoor(Vector3 location)
    {
        doorLocations.Add(location);
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


        Vector3[] vertices = { GetRoomLowerLeft(), GetRoomUpperLeft(), GetRoomLowerRight(), GetRoomUpperRight() };
        int[] triangles = { 0, 1, 2, 2, 1, 3 };

        CreateObjectInstructions(roomObject, vertices, triangles);
    }


    Vector3? CheckDoorPosition(List<Vector3> doorLocations, Vector3 pointA, Vector3 pointB)
    {
        foreach (var location in doorLocations)
        {
            if (location.x == pointA.x && location.x == pointB.x)
            {
                return location;
            }
            else if (location.z == pointA.z && location.z == pointB.z)
            {
                return location;
            }
        }
        return null;
    }


    public void CreateRoomWall(Vector3 pointA, Vector3 pointB, float wallHeigt)
    {
        Vector3[] vertices = new Vector3[8];
        Vector3[] verticesIfHall = new Vector3[8];
        string name;

        List<int> tempTriangles = new List<int>();
        List<int> tempTrianglesIfHall = new List<int>();

        if (pointA.z == pointB.z)  // Horizontal wall
        {
            if (pointA.z > GetRoomCenter().z) // Upper wall
            {
                Debug.Log("Upper wall");
                Vector3? door = CheckDoorPosition(doorLocations, pointA, pointB);
                if (door != null)
                {
                    Vector3[] tempVertLeftWall = new Vector3[] { pointA + Vector3.back,                            pointA,                            (Vector3) door + Vector3.left + Vector3.back,                            (Vector3) door + Vector3.left,
                                                                 pointA + Vector3.back + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), (Vector3) door + Vector3.left + Vector3.back + (Vector3.up * wallHeigt), (Vector3) door + Vector3.left + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertRightWall = new Vector3[] { (Vector3) door + Vector3.right + Vector3.back,                            (Vector3) door + Vector3.right,                            pointB + Vector3.back,                            pointB,
                                                                  (Vector3) door + Vector3.right + Vector3.back + (Vector3.up * wallHeigt), (Vector3) door + Vector3.right + (Vector3.up * wallHeigt), pointB + Vector3.back + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt)};
                    vertices = tempVertLeftWall;
                    verticesIfHall = tempVertRightWall;
                    if (seeTroughMode) tempTriangles = new int[] { 6, 0, 4, 2, 0, 6, 6, 3, 2, 7, 3, 6 }.ToList(); // COVERAGE OF SELECT EDGES
                    if (seeTroughMode) tempTrianglesIfHall = new int[] { 6, 0, 4, 2, 0, 6, 0, 5, 4, 1, 5, 0 }.ToList(); // COVERAGE OF SELECT EDGES
                }
                else
                {
                    Vector3[] tempVert = new Vector3[] { pointA + Vector3.back,                            pointA,                            pointB + Vector3.back,                            pointB,
                                                         pointA + Vector3.back + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), pointB + Vector3.back + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                    verticesIfHall = null;
                    if (seeTroughMode) tempTriangles = new int[] { 6, 0, 4, 2, 0, 6 }.ToList(); // COVERAGE OF SELECT EDGES
                }
                name = "Upper";
            }
            else // Lower wall
            {
                Debug.Log("Lower wall");
                Vector3? door = CheckDoorPosition(doorLocations, pointA, pointB);
                if (door != null)
                {
                    Vector3[] tempVertLeftWall = new Vector3[] { pointA,                            pointA + Vector3.forward,                            (Vector3) door + Vector3.left,                            (Vector3) door + Vector3.left + Vector3.forward,
                                                                 pointA + (Vector3.up * wallHeigt), pointA + Vector3.forward + (Vector3.up * wallHeigt), (Vector3) door + Vector3.left + (Vector3.up * wallHeigt), (Vector3) door + Vector3.left + Vector3.forward + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertRightWall = new Vector3[] { (Vector3) door + Vector3.right,                            (Vector3) door + Vector3.right + Vector3.forward,                            pointB,                            pointB + Vector3.forward,
                                                                  (Vector3) door + Vector3.right + (Vector3.up * wallHeigt), (Vector3) door + Vector3.right + Vector3.forward + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt), pointB + Vector3.forward + (Vector3.up * wallHeigt)};
                    vertices = tempVertLeftWall;
                    verticesIfHall = tempVertRightWall;
                    if (seeTroughMode) tempTriangles = new int[] { 3, 5, 1, 7, 5, 3, 6, 3, 2, 7, 3, 6 }.ToList(); // COVERAGE OF SELECT EDGES
                    if (seeTroughMode) tempTrianglesIfHall = new int[] { 3, 5, 1, 7, 5, 3, 0, 5, 4, 1, 5, 0 }.ToList(); // COVERAGE OF SELECT EDGES
                }
                else
                {
                    Vector3[] tempVert = new Vector3[] { pointA,                            pointA + Vector3.forward,                            pointB,                            pointB + Vector3.forward,
                                                         pointA + (Vector3.up * wallHeigt), pointA + Vector3.forward + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt), pointB + Vector3.forward + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                    verticesIfHall = null;
                    if (seeTroughMode) tempTriangles = new int[] { 3, 5, 1, 7, 5, 3 }.ToList(); // COVERAGE OF SELECT EDGES
                }
                name = "Lower";
            }
        }
        else // Vertical wall
        {
            if (pointA.x < GetRoomCenter().x) // Left wall
            {
                Debug.Log("Left wall");
                Vector3? door = CheckDoorPosition(doorLocations, pointA, pointB);
                if (door != null)
                {
                    Vector3[] tempVertLowerWall = new Vector3[] { pointA,                            (Vector3) door + Vector3.back,                            pointA + Vector3.right,                            (Vector3) door + Vector3.back + Vector3.right,
                                                                  pointA + (Vector3.up * wallHeigt), (Vector3) door + Vector3.back + (Vector3.up * wallHeigt), pointA + Vector3.right + (Vector3.up * wallHeigt), (Vector3) door + Vector3.back + Vector3.right + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertUpperWall = new Vector3[] { (Vector3) door + Vector3.forward,                            pointB,                            (Vector3) door + Vector3.forward + Vector3.right,                            pointB + Vector3.right,
                                                                  (Vector3) door + Vector3.forward + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt), (Vector3) door + Vector3.forward + Vector3.right + (Vector3.up * wallHeigt), pointB + Vector3.right + (Vector3.up * wallHeigt)};
                    vertices = tempVertLowerWall;
                    verticesIfHall = tempVertUpperWall;
                    if (seeTroughMode) tempTriangles = new int[] { 6, 3, 2, 7, 3, 6, 3, 5, 1, 7, 5, 3 }.ToList(); // COVERAGE OF SELECT EDGES
                    if (seeTroughMode) tempTrianglesIfHall = new int[] { 6, 3, 2, 7, 3, 6, 6, 0, 4, 2, 0, 6 }.ToList(); // COVERAGE OF SELECT EDGES
                }
                else
                {
                    Vector3[] tempVert = new Vector3[] { pointA,                            pointB,                            pointA + Vector3.right,                            pointB + Vector3.right,
                                                         pointA + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt), pointA + Vector3.right + (Vector3.up * wallHeigt), pointB + Vector3.right + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                    verticesIfHall = null;
                    if (seeTroughMode) tempTriangles = new int[] { 6, 3, 2, 7, 3, 6 }.ToList(); // COVERAGE OF SELECT EDGES
                }
                name = "Left";
            }
            else // Right wall
            {
                Debug.Log("Right wall");
                Vector3? door = CheckDoorPosition(doorLocations, pointA, pointB);
                if (door != null)
                {
                    Vector3[] tempVertLowerWall = new Vector3[] { pointA + Vector3.left,                            (Vector3) door + Vector3.back + Vector3.left,                            pointA,                            (Vector3) door + Vector3.back,
                                                                  pointA + Vector3.left + (Vector3.up * wallHeigt), (Vector3) door + Vector3.back + Vector3.left + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), (Vector3) door + Vector3.back + (Vector3.up * wallHeigt) };

                    Vector3[] tempVertUpperWall = new Vector3[] { (Vector3) door + Vector3.forward + Vector3.left,                            pointB + Vector3.left,                            (Vector3) door + Vector3.forward,                            pointB,
                                                                  (Vector3) door + Vector3.forward + Vector3.left + (Vector3.up * wallHeigt), pointB + Vector3.left + (Vector3.up * wallHeigt), (Vector3) door + Vector3.forward + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt) };
                    vertices = tempVertLowerWall;
                    verticesIfHall = tempVertUpperWall;
                    if (seeTroughMode) tempTriangles = new int[] { 0, 5, 4, 1, 5, 0, 3, 5, 1, 7, 5, 3 }.ToList(); // COVERAGE OF SELECT EDGES
                    if (seeTroughMode) tempTrianglesIfHall = new int[] { 0, 5, 4, 1, 5, 0, 6, 0, 4, 2, 0, 6 }.ToList(); // COVERAGE OF SELECT EDGES
                }
                else
                {
                    Vector3[] tempVert = new Vector3[] { pointA + Vector3.left,                            pointB + Vector3.left,                            pointA,                            pointB,
                                                         pointA + Vector3.left + (Vector3.up * wallHeigt), pointB + Vector3.left + (Vector3.up * wallHeigt), pointA + (Vector3.up * wallHeigt), pointB + (Vector3.up * wallHeigt) };
                    vertices = tempVert;
                    verticesIfHall = null;
                    if (seeTroughMode) tempTriangles = new int[] { 0, 5, 4, 1, 5, 0 }.ToList(); // COVERAGE OF SELECT EDGES
                }
                name = "Right";
            }
        }

        GameObject obj = new GameObject(name + " wall");
        obj.layer = 3;
        obj.transform.parent = roomObject.transform;

        if (!seeTroughMode) tempTriangles = new int[] { 3, 5, 1, 7, 5, 3, 6, 3, 2, 7, 3, 6, 6, 0, 4, 2, 0, 6, 0, 5, 4, 1, 5, 0, 7, 4, 5, 6, 4, 7 }.ToList(); // COVERAGE OF ALL EDGES (exept bottom edge)
        int[] triangles = new int[tempTriangles.Count];
        for (int i = 0; i < tempTriangles.Count; i++)
        {
            triangles[i] = tempTriangles[i];
        }


        CreateObjectInstructions(obj, vertices, triangles);
        Vector3? doors = CheckDoorPosition(doorLocations, pointA, pointB);
        if (doors != null)
        {
            GameObject objTwo = new GameObject(name + " wall");
            objTwo.layer = 3;
            objTwo.transform.parent = roomObject.transform;

            if (!seeTroughMode) tempTrianglesIfHall = new int[] { 3, 5, 1, 7, 5, 3, 6, 3, 2, 7, 3, 6, 6, 0, 4, 2, 0, 6, 0, 5, 4, 1, 5, 0, 7, 4, 5, 6, 4, 7 }.ToList(); // COVERAGE OF ALL EDGES (exept bottom edge)
            int[] trianglesIfHall = new int[tempTrianglesIfHall.Count];
            for (int i = 0; i < tempTrianglesIfHall.Count; i++)
            {
                trianglesIfHall[i] = tempTrianglesIfHall[i];
            }

            CreateObjectInstructions(objTwo, verticesIfHall, trianglesIfHall);
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