using System.Collections;
using UnityEngine;

public class PlayerTurnState : BaseState
{
    #region varaiables and such

    [SerializeField] Player player;
    Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    Vector3 nextPos, destination;

    public float normalSpeed;
    float deadZone = 0.7f;
    float speed, fallDistance, deadTimer;
    public float maxFallDistance;

    bool canMove, moving, canJump, directionChange, falling, dead, firstFloorTouch;
    public LayerMask whatIsWall, whatIsFloor;

    Vector3 spawn;
    #endregion

    public override void OnAwake()
    {
        currentDirection = up;
        nextPos = transform.forward;
        destination = transform.position;
        speed = normalSpeed;
        spawn = transform.position;
        GameObject floor = CheckFloor(1.1f);
        transform.position = new Vector3(floor.transform.position.x + 0.5f, floor.transform.position.y + 1.5f, floor.transform.position.z - 0.5f);
    }


    public override void OnEnter()
    {

    }


    public override void OnExit()
    {

    }


    public override void OnUpdate()
    {
        Move();
    }


    public override void OnFixedUpdate()
    {

    }


    #region Movement
    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
        if (transform.position == destination) moving = false;
        else falling = true;

        GameObject floor = CheckFloor(1.1f);
        //if (floor != null) floor.GetComponent<Glow>().timer = 1f;
        if (!moving && !dead)
        {
            if (PlayerInput.leftJoy.y > deadZone)
            {
                CameraCheck();
                currentDirection = up;
                canMove = true;
            }
            if (PlayerInput.leftJoy.x > deadZone)
            {
                CameraCheck();
                currentDirection = right;
                canMove = true;
            }
            if (PlayerInput.leftJoy.y < -deadZone)
            {
                CameraCheck();
                currentDirection = down;
                canMove = true;
            }
            if (PlayerInput.leftJoy.x < -deadZone)
            {
                CameraCheck();
                currentDirection = left;
                canMove = true;
            }
            if (PlayerInput.southPressed)
            {
                PlayerInput.southPressed = false;
                CameraCheck();
                canJump = true;
                canMove = true;
            }
            if (PlayerInput.leftShoulderPressed)
            {
                PlayerInput.leftShoulderPressed = false;
                CameraCheck();
                canMove = true;
                directionChange = true;
                currentDirection += new Vector3(0, -90, 0);
            }
            if (PlayerInput.rightShoulderPressed)
            {
                PlayerInput.rightShoulderPressed = false;
                CameraCheck();
                canMove = true;
                directionChange = true;
                currentDirection += new Vector3(0, 90, 0);
            }
        }

        if (canMove && !falling && !dead)
        {
            transform.position = new Vector3(floor.transform.position.x + 0.5f, floor.transform.position.y + 1.5f, floor.transform.position.z - 0.5f);
            transform.localEulerAngles = currentDirection;
            nextPos = transform.forward;

            if (!directionChange)
            {
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

            if (PlayerInput.westPressed) speed = 2 * normalSpeed;
            canMove = false;
            canJump = false;
            directionChange = false;
        }
        else if (!moving)
        {
            if (floor != null)
            {
                Vector3 temp;
                if (floor.CompareTag("Floater")) temp = new Vector3(floor.transform.position.x + 0.5f, floor.transform.position.y + 1.5f, floor.transform.position.z - 0.5f);
                else temp = new Vector3(transform.position.x, floor.transform.position.y + 1.5f, transform.position.z);
                fallDistance = 0;
                destination = temp;
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

    void DeadAndRespawn()
    {
        if (!dead) deadTimer = 3;
        if (deadTimer < 0)
        {
            if (dead) transform.position = spawn;
            dead = false;
            player.camFollow = true;
            firstFloorTouch = false;
            // Remove lifes here
        }
        else
        {
            dead = true;
            player.camFollow = false;
            deadTimer -= Time.deltaTime;
        }
    }

    void CameraCheck()
    {
        if ((player.cam.transform.localEulerAngles.y > 0 && player.cam.transform.localEulerAngles.y <= 45) || (player.cam.transform.localEulerAngles.y > 315 && player.cam.transform.localEulerAngles.y <= 360))
        {
            up = Vector3.zero;
            right = new Vector3(0, 90, 0);
            down = new Vector3(0, 180, 0);
            left = new Vector3(0, 270, 0);
        }
        else if (player.cam.transform.localEulerAngles.y > 45 && player.cam.transform.localEulerAngles.y <= 135)
        {
            left = Vector3.zero;
            up = new Vector3(0, 90, 0);
            right = new Vector3(0, 180, 0);
            down = new Vector3(0, 270, 0);
        }
        else if (player.cam.transform.localEulerAngles.y > 135 && player.cam.transform.localEulerAngles.y <= 225)
        {
            down = Vector3.zero;
            left = new Vector3(0, 90, 0);
            up = new Vector3(0, 180, 0);
            right = new Vector3(0, 270, 0);
        }
        else if (player.cam.transform.localEulerAngles.y > 225 && player.cam.transform.localEulerAngles.y <= 315)
        {
            right = Vector3.zero;
            down = new Vector3(0, 90, 0);
            left = new Vector3(0, 180, 0);
            up = new Vector3(0, 270, 0);
        }
    }
    #endregion


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


    bool CheckForward(float length)
    {
        return ValidityCheck(transform.position + transform.up * 0.5f, transform.forward, length, whatIsWall);
    }


    bool CheckForwardUp(float length)
    {
        return ValidityCheck(transform.position + transform.up * 1.5f, transform.forward, length, whatIsWall);
    }


    bool CheckForwardDoubleUp(float length)
    {
        return ValidityCheck(transform.position + transform.up * 2.5f, transform.forward, length, whatIsWall);
    }


    bool CheckForwardDownForGround()
    {
        return ValidityCheck(transform.position + transform.forward - 1.0f * transform.up, -transform.up, 0.6f, whatIsFloor);
    }

    bool CheckForwardDownForSpace()
    {
        return ValidityCheck(transform.position + transform.forward + transform.up * 0.5f, -transform.up, 1.1f, whatIsFloor);
    }


    bool CheckRoof()
    {
        return ValidityCheck(transform.position + transform.up * 0.5f, transform.up, 2.1f, whatIsWall);
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
        Ray ray = new Ray(transform.position + transform.up * 0.5f, -transform.up);
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
