using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject objectToFollow;
    public GameObject cam;
    public bool camFollow = true;
    Vector3 camVelocity = Vector3.zero;
    public float camSmoothSpeed, camSpeed;

    void LateUpdate()
    {
        if (camFollow)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, objectToFollow.transform.position, ref camVelocity, camSmoothSpeed * Time.deltaTime);
            cam.transform.Rotate(0, PlayerInput.rightJoy.x * camSpeed * Time.deltaTime, 0);
            if (PlayerInput.dPad.x != 0)
            {
                cam.transform.Rotate(0, PlayerInput.dPad.x * 90, 0);
                PlayerInput.dPad = new Vector2(0, 0);
            }
        }
    }
}
