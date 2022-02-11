using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    Vector3 nextPos, destination;

    float speed = 5f;

    bool canMove;
    public LayerMask whatIsWall;

    // Start is called before the first frame update
    void Start()
    {
        currentDirection = up;
        nextPos = Vector3.forward;
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.W))
        {
            nextPos = Vector3.forward;
            currentDirection = up;
            canMove = true;
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            nextPos = Vector3.right;
            currentDirection = right;
            canMove = true;
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            nextPos = Vector3.back;
            currentDirection = down;
            canMove = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            nextPos = Vector3.left;
            currentDirection = left;
            canMove = true;
        }

        if (Vector3.Distance(destination, transform.position) <= 0.0001f)
        {
            transform.localEulerAngles = currentDirection;

            if (canMove && !Input.GetKey(KeyCode.LeftShift))
            {
                if (Valid(1f))
                {
                    destination = transform.position + nextPos;
                }
            }
            canMove = false;
        }
    }

    bool Valid(float rayLength)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (Physics.Raycast(ray, out hit, rayLength, whatIsWall))
        {
            return false;
        }
        return true;
    }

}
