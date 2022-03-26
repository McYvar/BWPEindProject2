using UnityEngine;

public abstract class MovementBase : BaseState
{
    protected bool canMove, moving, canJump, directionChange, falling, dead, firstFloorTouch;
    protected Vector3 nextPos, destination;
    protected int turns;
    protected Vector3Int spawn;
    protected float fallDistance, deadTimer;
    protected float maxFallDistance = 10, speed;

    protected Vector3Int up = Vector3Int.zero,
        right = new Vector3Int(0, 90, 0),
        down = new Vector3Int(0, 180, 0),
        left = new Vector3Int(0, 270, 0),
        currentDirection = Vector3Int.zero;

    public float normalSpeed;
    public LayerMask whatIsWall, whatIsFloor, whatIsBorder;

    public virtual void Move()
    {
        canMove = false;
        canJump = false;
        directionChange = false;

        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        if (transform.position == destination)
        {
            if (moving)
            {
                turns--;
                moving = false;
            }
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
                // Enemy has no override for other check so these lines only apply to player
                GameObject enemy = otherCheck();
                if (enemy != null)
                {
                    enemy.GetComponent<IDamagable>().takeDamage(Player.currentDamage);
                    moving = true;
                    return;
                }

                if (!checkForBorder(1f)) return;

                if (canJump && !checkForBorder(2f)) return;

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
                    destination = transform.position + (2 * nextPos) + transform.up;
                    canJump = false;
                    moving = true;
                    speed = normalSpeed * 2f;
                }
                // Jump two spaces forward and two up (only if there is something in between)
                else if (canJump && !CheckForward(0.6f) && CheckForwardUp(1.1f) && CheckForwardDoubleUp(1.6f))
                {
                    destination = transform.position + (2 * nextPos) + (2 * transform.up);
                    canJump = false;
                    moving = true;
                    speed = normalSpeed * 2f;
                }
                // Jump one space forward and two up
                else if (canJump && !CheckForward(0.6f) && !CheckForwardUp(0.6f) && CheckForwardDoubleUp(0.6f) && CheckRoof())
                {
                    destination = transform.position + nextPos + (2 * transform.up);
                    canJump = false;
                    moving = true;
                    speed = normalSpeed;
                }
                // Move one space forward and down one
                else if (!CheckForwardDownForGround() && CheckForward(0.6f) && CheckForwardDownForSpace())
                {
                    destination = transform.position + nextPos + -transform.up;
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
                    destination = transform.position + nextPos + transform.up;
                    moving = true;
                    speed = normalSpeed;
                }
            }
        }
        else if (!moving)
        {
            if (floor != null)
            {
                destination = new Vector3(transform.position.x, floor.transform.position.y + 1, transform.position.z);
                fallDistance = 0;
                speed = Mathf.Infinity;
                falling = false;
                firstFloorTouch = true;
            }
            else
            {
                destination = transform.position + -transform.up;
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
        destination = Vector3Int.RoundToInt(destination);
    }


    public virtual void InputCheck()
    {
    }


    public virtual void Deadbehaviour()
    {
    }


    public void GoUp()
    {
        currentDirection = up;
        canMove = true;
    }


    public void GoRight()
    {
        currentDirection = right;
        canMove = true;
    }


    public void GoDown()
    {
        currentDirection = down;
        canMove = true;
    }


    public void GoLeft()
    {
        currentDirection = left;
        canMove = true;
    }


    public void DoJump()
    {
        canJump = true;
        canMove = true;
    }


    public void RotateLeft()
    {
        canMove = true;
        directionChange = true;
        currentDirection += new Vector3Int(0, -90, 0);
    }


    public void RotateRight()
    {
        canMove = true;
        directionChange = true;
        currentDirection += new Vector3Int(0, 90, 0);
    }


    #region Raycast checks
    protected bool ValidityCheck(Vector3 startingPosition, Vector3 direction, float rayLength, LayerMask mask)
    {
        Ray ray = new Ray(startingPosition, direction);

        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red);

        if (Physics.Raycast(ray, rayLength, mask, QueryTriggerInteraction.UseGlobal))
        {
            return false;
        }
        return true;
    }

    bool checkForBorder(float tilesForward)
    {
        return ValidityCheck(transform.position + transform.forward * tilesForward + transform.up * 100, -transform.up, Mathf.Infinity, whatIsBorder);
    }


    protected bool CheckForward(float length)
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

        if (Physics.Raycast(ray, out hit, rayLength, whatIsFloor, QueryTriggerInteraction.UseGlobal))
        {
            return hit.transform.gameObject;
        }
        return null;
    }


    public virtual GameObject otherCheck()
    {
        return null;
    }
    #endregion
}
