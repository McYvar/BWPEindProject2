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
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                nextPos = Vector3.forward;
                currentDirection = up;
                canMove = true;
            }
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                nextPos = Vector3.right;
                currentDirection = right;
                canMove = true;
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                nextPos = Vector3.back;
                currentDirection = down;
                canMove = true;
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
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
                // Jump two spaces forward
                if (canJump && CheckForward())
                {
                    destination = transform.position + (2 * nextPos);
                    canJump = false;
                    moving = true;
                    speed = 2 * normalSpeed;
                    Debug.Log("Jump two spaces forward");
                }
                // Jump two spaces forward over one space
                else if (canJump && CheckForwardUp())
                {
                    destination = transform.position + (2 * nextPos) + Vector3.up;
                    canJump = false;
                    moving = true;
                    speed = normalSpeed * 1.5f;
                    Debug.Log("Jump two spaces forward over one space");
                }
                // Jump one space forward and two up
                else if (canJump && CheckForwardDoubleUp())
                {
                    destination = transform.position + nextPos + (2 * Vector3.up);
                    canJump = false;
                    moving = true;
                    speed = normalSpeed;
                    Debug.Log("Jump one space forward and two up");
                }
                // Move one space forward and down one
                else if (CheckForwardDown() && CheckForward())
                {
                    destination = transform.position + nextPos + Vector3.down;
                    moving = true;
                    speed = normalSpeed;
                    Debug.Log("Move one space forward and down one");
                }
                // Move one space forward
                else if (CheckForward())
                {
                    destination = transform.position + nextPos;
                    moving = true;
                    speed = normalSpeed;
                    Debug.Log("Move one space forward");
                }
                // Move one space forward and one up
                else if (CheckForwardUp())
                {
                    destination = transform.position + nextPos + Vector3.up;
                    moving = true;
                    speed = normalSpeed;
                    Debug.Log("Move one space forward and one up");
                }
            }

            if (Input.GetKey(KeyCode.LeftShift)) speed = 2 * normalSpeed;
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
            Debug.Log(CheckForward());
            Debug.Log(CheckForwardUp());
            Debug.Log(CheckForwardDoubleUp());
            Debug.Log(CheckForwardDown());
        }
    }


    bool ValidityCheck(Vector3 startingPosition, Vector3 direction, float length, LayerMask mask)
    {
        Ray ray = new Ray(startingPosition, direction);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (Physics.Raycast(ray, out hit, length, mask))
        {
            return true;
        }
        return false;
    }


    bool CheckForward()
    {
        return ValidityCheck(transform.position, transform.forward, 1.1f, whatIsWall);
    }


    bool CheckForwardUp()
    {
        return ValidityCheck(transform.position + transform.up, transform.forward, 1.1f, whatIsWall);
    }


    bool CheckForwardDoubleUp()
    {
        return ValidityCheck(transform.position + (transform.up * 2), transform.forward, 1.1f, whatIsWall);
    }


    bool CheckForwardDown()
    {
        return ValidityCheck(transform.position + transform.forward, -transform.up, 1.1f, whatIsFloor);
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
