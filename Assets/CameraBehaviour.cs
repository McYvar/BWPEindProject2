using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject objectToFollow;
    public GameObject cam;
    public GameObject actualCamera;
    [SerializeField] GameObject higherCamera;
    public bool camFollow = true;
    Vector3 camVelocity = Vector3.zero;
    public float camSmoothSpeed, camSpeed;
    public Vector3 camPosition;
    Vector2 grid;

    private void Start()
    {
        grid = FindObjectOfType<TileBasedDungeonGeneration.TileBasedDungeonGeneration>().GetGrid();
        cam.transform.position = new Vector3(grid.x / 2, 30, grid.y / 2);
    }

    void LateUpdate()
    {
        if (camFollow)
        {
            camPosition = Vector3.SmoothDamp(cam.transform.position, objectToFollow.transform.position, ref camVelocity, camSmoothSpeed * Time.deltaTime);
            cam.transform.position = camPosition;
            cam.transform.Rotate(0, PlayerInput.rightJoy.x * camSpeed * Time.deltaTime, 0);
            if (PlayerInput.dPad.x != 0)
            {
                cam.transform.Rotate(0, PlayerInput.dPad.x * 90, 0);
                PlayerInput.dPad = new Vector2(0, 0);
            }
        }

        if (GameStates.allEnemiesMoveAtTheSameTimeStatic && GameStates.isRunning)
        {
            higherCamera.transform.position = new Vector3(objectToFollow.transform.position.x, higherCamera.transform.position.y, objectToFollow.transform.position.z);
            objectToFollow = higherCamera;
        }
    }
}
