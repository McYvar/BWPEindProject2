using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBase : BaseState
{
    protected bool canMove, moving, canJump, directionChange, falling, dead, firstFloorTouch;
    protected Vector3 nextPos, destination;
    protected int turns;
    protected Vector3 spawn;
    protected float fallDistance, deadTimer;
    protected float maxFallDistance = 10, speed;

    protected Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    public float normalSpeed;
    public LayerMask whatIsWall, whatIsFloor, whatIsBorder;

    public virtual void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        if (transform.position == destination)
        {
            if (moving)
            {
                turns--;
                moving = false;
            }
            //destination = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        }
        else falling = true;

        InputCheck();

        GameObject floor = CheckFloor(1.1f);
        if (canMove && !falling && !dead)
        {
            transform.localEulerAngles = currentDirection;
            nextPos = transform.forward;

            if (!directionChange)
            {
                if (checkForBorder())
                {
                    // Jump two spaces forward
                    if (canJump && CheckForward(1.6f))
                    {
                        destination = transform.position + (2 * nextPos);
                        canJump = false;
                        moving = true;
                        speed = 2f * normalSpeed;
                    }
                    // Jump two spaces forward over one space / Jump two spaces forward and one up
                    else if (canJump && CheckForwardUp(1.6f))
                    {
                        destination = transform.position + (2 * nextPos) + Vector3.up;
                        canJump = false;
                        moving = true;
                        speed = normalSpeed * 2f;
                    }
                    // Jump two spaces forward and two up (only if there is something in between)
                    else if (canJump && CheckForwardUp(0.6f) && CheckForwardDoubleUp(1.6f))
                    {
                        destination = transform.position + (2 * nextPos) + (2 * Vector3.up);
                        canJump = false;
                        moving = true;
                        speed = normalSpeed * 2f;
                    }
                    // Jump one space forward and two up
                    else if (canJump && CheckForwardDoubleUp(0.6f) && CheckRoof())
                    {
                        destination = transform.position + nextPos + (2 * Vector3.up);
                        canJump = false;
                        moving = true;
                        speed = normalSpeed;
                    }
                    // Move one space forward and down one
                    else if (!CheckForwardDownForGround() && CheckForward(0.6f) && CheckForwardDownForSpace())
                    {
                        destination = transform.position + nextPos + Vector3.down;
                        moving = true;
                        speed = normalSpeed;
                    }
                    // Move one space forward
                    else if (CheckForward(0.6f))
                    {
                        destination = transform.position + nextPos;
                        moving = true;
                        speed = normalSpeed;
                    }
                    // Move one space forward and one up
                    else if (CheckForwardUp(0.6f))
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
                    temp = new Vector3(floor.transform.position.x, floor.transform.position.y + 1f, floor.transform.position.z);
                    destination = temp;
                }
                else
                {
                    temp = new Vector3(transform.position.x, floor.transform.position.y + 1f, transform.position.z);
                    destination = temp;
                }
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
                    Deadbehaviour();
                }
            }
        }
        if (dead) Deadbehaviour();
    }


    public virtual void InputCheck()
    {

    }


    public virtual void Deadbehaviour()
    {
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

    bool checkForBorder()
    {
        return ValidityCheck(transform.position + transform.forward + transform.up * 10, -transform.up, 30, whatIsBorder);
    }


    bool CheckForward(float length)
    {
        return ValidityCheck(transform.position, transform.forward, length, whatIsWall);
    }


    bool CheckForwardUp(float length)
    {
        return ValidityCheck(transform.position + transform.up, transform.forward, length, whatIsWall);
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
        Ray ray = new Ray(transform.position - transform.up, -transform.up);
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
