using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBobling : MonoBehaviour
{
    public float rotationSpeed;
    public float bopHeight;
    public float bounceSpeed;

    private void Update()
    {
        // Makes a tiny animation based on rotation and position
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        transform.Translate(Vector3.up * Mathf.Cos((Time.time * bounceSpeed)) * bopHeight * Time.deltaTime);
    }
}
