using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public GameObject cam;
    Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    Vector3 nextPos, destination;

    float normalSpeed = 5f;
    float speed;

    bool canMove, moving, canJump, directionChange, falling;
    public LayerMask whatIsWall, whatIsFloor;

    void LateUpdate()
    {
        cam.transform.position = transform.position;
    }

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
        else falling = true;

        GameObject floor = CheckFloor(1.1f);
        if (floor != null) floor.GetComponent<Glow>().timer = 1f;

        if (!moving)
        {
            if (Input.GetKey(KeyCode.W))
            {
                nextPos = Vector3.forward;
                currentDirection = up;
                canMove = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                nextPos = Vector3.right;
                currentDirection = right;
                canMove = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                nextPos = Vector3.back;
                currentDirection = down;
                canMove = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                nextPos = Vector3.left;
                currentDirection = left;
                canMove = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                canJump = true;
                canMove = true;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                canMove = true;
                directionChange = true;
                currentDirection += left;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                canMove = true;
                directionChange = true;
                currentDirection += right;
            }
        }

        if (canMove && !falling)
        {
            transform.localEulerAngles = currentDirection;

            if (!directionChange)
            {
                if (canJump && Valid(2.1f))
                {
                    destination = transform.position + (2 * nextPos);
                    canJump = false;
                    moving = true;
                    speed = 2 * normalSpeed;
                }
                else if (ValidDown(1.1f))
                {
                    destination = transform.position + nextPos + Vector3.down;
                    moving = true;
                    speed = normalSpeed;
                }
                else if (Valid(1.1f))
                {
                    destination = transform.position + nextPos;
                    moving = true;
                    speed = normalSpeed;
                }
                else if (ValidUp(2.1f))
                {
                    destination = transform.position + nextPos + Vector3.up;
                    moving = true;
                    speed = normalSpeed;
                }
            }
            canMove = false;
            canJump = false;
            directionChange = false;
            nextPos = transform.forward;
        }
        else if (!moving)
        {
            if (floor != null)
            {
                Vector3 temp = new Vector3(floor.transform.position.x + 0.5f, floor.transform.position.y + 2f, floor.transform.position.z - 0.5f);
                destination = temp;
                speed = Mathf.Infinity;
                falling = false;
            }
            else
            {
                destination = transform.position + Vector3.down;
                speed = 30;
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

    bool ValidDown(float rayLength)
    {
        Ray ray = new Ray(transform.position + transform.forward + Vector3.down, Vector3.down);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.black);

        if (Physics.Raycast(ray, out hit, rayLength, whatIsFloor))
        {
            return true;
        }
        return false;
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
