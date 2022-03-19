using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBase : BaseState
{
    public bool canMove, moving, canJump, directionChange, falling, dead, firstFloorTouch;
    public float normalSpeed;
    protected Vector3 nextPos, destination;
    protected int turns;
    protected Vector3 spawn;
    protected float fallDistance, deadTimer;
    
    protected Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    protected float maxFallDistance = 10, speed;
    public LayerMask whatIsWall, whatIsFloor;

    public virtual void Move()
    {
        Debug.Log(transform.position + "; " + destination);
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        if (transform.position == destination) moving = false;
        else falling = true;

        InputCheck();

        GameObject floor = CheckFloor(1.1f);
        if (canMove && !falling && !dead)
        {
            transform.localEulerAngles = currentDirection;
            nextPos = transform.forward;

            if (!directionChange)
            {
                if (!CheckForwardDownWall())
                {
                    turns--;
                    // Jump two spaces forward
                    if (canJump && CheckForward(2.1f))
                    {
                        destination = transform.position + (2 * nextPos);
                        canJump = false;
                        moving = true;
                        speed = 2f * normalSpeed;
                    }
                    // Jump two spaces forward over one space
                    else if (canJump && CheckForwardUp(2.1f))
                    {
                        destination = transform.position + (2 * nextPos) + Vector3.up;
                        canJump = false;
                        moving = true;
                        speed = normalSpeed * 2f;
                    }
                    // Jump two spaces forward and two up (only if there is something in between)
                    else if (canJump && CheckForwardUp(1.1f) && CheckForwardDoubleUp(2.1f))
                    {
                        destination = transform.position + (2 * nextPos) + (2 * Vector3.up);
                        canJump = false;
                        moving = true;
                        speed = normalSpeed * 2f;
                    }
                    // Jump one space forward and two up
                    else if (canJump && CheckForwardDoubleUp(1.1f) && CheckRoof())
                    {
                        destination = transform.position + nextPos + (2 * Vector3.up);
                        canJump = false;
                        moving = true;
                        speed = normalSpeed;
                    }
                    // Move one space forward and down one
                    else if (!CheckForwardDownForGround() && CheckForward(1.1f) && CheckForwardDownForSpace())
                    {
                        destination = transform.position + nextPos + Vector3.down;
                        moving = true;
                        speed = normalSpeed;
                    }
                    // Move one space forward
                    else if (CheckForward(1.1f))
                    {
                        destination = transform.position + nextPos;
                        moving = true;
                        speed = normalSpeed;
                    }
                    // Move one space forward and one up
                    else if (CheckForwardUp(1.1f))
                    {
                        destination = transform.position + nextPos + Vector3.up;
                        moving = true;
                        speed = normalSpeed;
                    }
                }
            }

            canMove = false;
            canJump = false;
            directionChange = false;
        }
        else if (!moving)
        {
            if (floor != null)
            {
                Vector3 temp;
                if (floor.CompareTag("Floater"))
                {
                    temp = new Vector3(floor.transform.position.x + 0.5f, floor.transform.position.y + 1f, floor.transform.position.z - 0.5f);
                    destination = temp;
                }
                //else temp = new Vector3(transform.position.x, floor.transform.position.y + 1f, transform.position.z);
                fallDistance = 0;
                speed = Mathf.Infinity;
                falling = false;
                firstFloorTouch = true;
            }
            else
            {
                destination = transform.position + Vector3.down;
                speed = 30;

                float currentFallDistance = CheckFallDistance();
                if (currentFallDistance > fallDistance) fallDistance = currentFallDistance;
                if (fallDistance > maxFallDistance && firstFloorTouch)
                {
                    DeadAndRespawn();
                }
            }
        }
        if (dead) DeadAndRespawn();
    }

    public virtual void InputCheck()
    {

    }

    public virtual void DeadAndRespawn()
    {
        if (!dead) deadTimer = 3;
        if (deadTimer < 0)
        {
            if (dead) transform.position = spawn;
            dead = false;
            firstFloorTouch = false;
        }
        else
        {
            dead = true;
            deadTimer -= Time.deltaTime;
        }
    }


    #region Raycast checks
    bool ValidityCheck(Vector3 startingPosition, Vector3 direction, float rayLength, LayerMask mask)
    {
        Ray ray = new Ray(startingPosition, direction);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

        if (Physics.Raycast(ray, out hit, rayLength, mask))
        {
            return false;
        }
        return true;
    }

    bool CheckForwardDownWall()
    {
        return ValidityCheck(transform.position + transform.forward + transform.up * 10, -transform.up, 30, whatIsFloor);
    }


    bool CheckForward(float length)
    {
        return ValidityCheck(transform.position, transform.forward, length, whatIsWall);
    }


    bool CheckForwardUp(float length)
    {
        return ValidityCheck(transform.position + transform.up * 1f, transform.forward, length, whatIsWall);
    }


    bool CheckForwardDoubleUp(float length)
    {
        return ValidityCheck(transform.position + transform.up * 2f, transform.forward, length, whatIsWall);
    }


    bool CheckForwardDownForGround()
    {
        return ValidityCheck(transform.position + transform.forward - transform.up, -transform.up, 0.6f, whatIsFloor);
    }

    bool CheckForwardDownForSpace()
    {
        return ValidityCheck(transform.position + transform.forward, -transform.up, 1.1f, whatIsFloor);
    }


    bool CheckRoof()
    {
        return ValidityCheck(transform.position, transform.up, 2.1f, whatIsWall);
    }


    float CheckFallDistance()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position - transform.up * 0.4f, -transform.up);
        if (Physics.Raycast(ray, out hit, 1000, whatIsFloor, QueryTriggerInteraction.UseGlobal))
        {
            return Vector3.Distance(hit.point, transform.position);
        }
        return 1000;
    }


    GameObject CheckFloor(float rayLength)
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.yellow);

        if (Physics.Raycast(ray, out hit, rayLength, whatIsFloor))
        {
            return hit.transform.gameObject;
        }
        return null;
    }
    #endregion
}
