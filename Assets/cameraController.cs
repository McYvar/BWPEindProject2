using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public GameObject player;
    // Tiny method to make the camera always stay on the players position
    private void LateUpdate()
    {
        transform.position = player.transform.position;
    }
}
