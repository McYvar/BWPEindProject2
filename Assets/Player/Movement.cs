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

    float normalSpeed = 5f;
    float speed;

    bool canMove, moving;
    public LayerMask whatIsWall, whatIsFloor;


    void Start()
    {
        currentDirection = up;
        nextPos = Vector3.forward;
        destination = transform.position;
        speed = normalSpeed;
    }


    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        if (transform.position == destination) moving = false;

        if (!moving)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                nextPos = Vector3.forward;
                currentDirection = up;
                canMove = true;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                nextPos = Vector3.right;
                currentDirection = right;
                canMove = true;
            }
            if (Input.GetKeyDown(KeyCode.S))
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
        }

        if (Vector3.Distance(destination, transform.position) <= 0.0001f && canMove)
        {
            transform.localEulerAngles = currentDirection;

            if (canMove && !Input.GetKey(KeyCode.LeftShift))
            {
                if (Valid(1f))
                {
                    destination = transform.position + nextPos;
                    moving = true;
                }
                else if (ValidUp(2))
                {
                    destination = transform.position + nextPos + Vector3.up;
                    moving = true;
                }
            }
            canMove = false;
            speed = normalSpeed;
        }
        else if (!moving)
        {
            GameObject floor = CheckFloor(1f);
            if (floor != null)
            {
                Vector3 temp = new Vector3(floor.transform.position.x + 0.5f, transform.position.y, floor.transform.position.z - 0.5f);
                destination = temp;
                speed = 1000;
            }
            else
            {
                destination = transform.position + Vector3.down;
                speed = 10;
            }
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


    bool ValidUp(float rayLength)
    {
        Ray ray = new Ray(transform.position, transform.forward + Vector3.up);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.blue);

        if (Physics.Raycast(ray, out hit, rayLength, whatIsWall))
        {
            return false;
        }
        return true;
    }

    GameObject CheckFloor(float rayLength)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);

        if (Physics.Raycast(ray, out hit, rayLength, whatIsFloor)) {
            return hit.transform.gameObject;
        }
        return null;
    }
}
