using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    #region varaiables and such
    public GameObject cam;
    bool camFollow = true;
    Vector3 camVelocity = Vector3.zero;
    public float camSmoothSpeed, camSpeed;

    Vector3 up = Vector3.zero,
        right = new Vector3(0, 90, 0),
        down = new Vector3(0, 180, 0),
        left = new Vector3(0, 270, 0),
        currentDirection = Vector3.zero;

    Vector3 nextPos, destination;

    float normalSpeed = 5f;
    float speed, fallDistance, deadTimer;
    public float maxFallDistance;

    bool canMove, moving, canJump, directionChange, falling, dead, firstFloorTouch, mouseCheat;
    public LayerMask whatIsWall, whatIsFloor;

    Vector3 spawn;
    #endregion


    void Start()
    {
        currentDirection = up;
        nextPos = transform.forward;
        destination = transform.position;
        speed = normalSpeed;
        spawn = transform.position;
    }


    void LateUpdate()
    {
        if (camFollow)
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, transform.position, ref camVelocity, camSmoothSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.Z)) cam.transform.Rotate(0, 1 * camSpeed * Time.deltaTime, 0);
            if (Input.GetKey(KeyCode.X)) cam.transform.Rotate(0, -1 * camSpeed * Time.deltaTime, 0);
            if (mouseCheat)
            {
                cam.transform.Rotate(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
                cam.transform.rotation = Quaternion.Euler(cam.transform.localEulerAngles.x, cam.transform.localEulerAngles.y, 0);
            }
        }
    }


    void Update()
    {
        Move();

        MouseCheatCheck();
    }

    void MouseCheatCheck()
    {
        if (Input.GetKey(KeyCode.Y) && Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.Alpha3) && Input.GetKey(KeyCode.T))
        {
            mouseCheat = true;
        }
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
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                CameraCheck();
                currentDirection = up;
                canMove = true;
            }
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                CameraCheck();
                currentDirection = right;
                canMove = true;
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                CameraCheck();
                currentDirection = down;
                canMove = true;
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                CameraCheck();
                currentDirection = left;
                canMove = true;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                CameraCheck();
                canJump = true;
                canMove = true;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CameraCheck();
                canMove = true;
                directionChange = true;
                currentDirection += new Vector3(0, -90, 0);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                CameraCheck();
                canMove = true;
                directionChange = true;
                currentDirection += new Vector3(0, 90, 0);
            }
        }

        if (canMove && !falling && !dead)
        {
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

            if (Input.GetKey(KeyCode.LeftShift)) speed = 2 * normalSpeed;
            canMove = false;
            canJump = false;
            directionChange = false;
        }
        else if (!moving)
        {
            if (floor != null)
            {
                fallDistance = 0;
                //Vector3 temp = new Vector3(floor.transform.position.x + 0.5f, floor.transform.position.y + 2f, floor.transform.position.z - 0.5f);
                Vector3 temp = new Vector3(transform.position.x, floor.transform.position.y + 1.5f, transform.position.z);
                destination = temp;
                speed = Mathf.Infinity;
                falling = false;
                firstFloorTouch = true;
            }
            else
            {
                destination = transform.position + Vector3.down;
                speed = 30;

                if (CheckFallDistance() > fallDistance) fallDistance = CheckFallDistance();
                if (fallDistance > maxFallDistance && firstFloorTouch)
                {
                    DeadAndRespawn();
                }
            }
        }
    }

    void DeadAndRespawn()
    {
        if (!dead) deadTimer = 3;

        if (deadTimer < 0)
        {
            if (dead) transform.position = spawn;
            dead = false;
            camFollow = true;
            firstFloorTouch = false;
            // Remove life here
        }
        else
        {
            dead = true;
            camFollow = false;
            deadTimer -= Time.deltaTime;
        }
    }

    void CameraCheck()
    {
        if ((cam.transform.localEulerAngles.y > 0 && cam.transform.localEulerAngles.y <= 45) || (cam.transform.localEulerAngles.y > 315 && cam.transform.localEulerAngles.y <= 360))
        {
            up = Vector3.zero;
            right = new Vector3(0, 90, 0);
            down = new Vector3(0, 180, 0);
            left = new Vector3(0, 270, 0);
        }
        else if (cam.transform.localEulerAngles.y > 45 && cam.transform.localEulerAngles.y <= 135)
        {
            left = Vector3.zero;
            up = new Vector3(0, 90, 0);
            right = new Vector3(0, 180, 0);
            down = new Vector3(0, 270, 0);
        }
        else if (cam.transform.localEulerAngles.y > 135 && cam.transform.localEulerAngles.y <= 225)
        {
            down = Vector3.zero;
            left = new Vector3(0, 90, 0);
            up = new Vector3(0, 180, 0);
            right = new Vector3(0, 270, 0);
        }
        else if (cam.transform.localEulerAngles.y > 225 && cam.transform.localEulerAngles.y <= 315)
        {
            right = Vector3.zero;
            down = new Vector3(0, 90, 0);
            left = new Vector3(0, 180, 0);
            up = new Vector3(0, 270, 0);
        }
    }
    #endregion


    #region Raycast checks
    bool ValidityCheck(Vector3 startingPosition, Vector3 direction, float length, LayerMask mask)
    {
        Ray ray = new Ray(startingPosition, direction);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (Physics.Raycast(ray, out hit, length, mask))
        {
            return false;
        }
        return true;
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
        return ValidityCheck(transform.position + (transform.up * 2), transform.forward, length, whatIsWall);
    }


    bool CheckForwardDownForGround()
    {
        return ValidityCheck(transform.position + transform.forward - (1.5f * transform.up), -transform.up, 0.6f, whatIsFloor);
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
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, whatIsFloor))
        {
            return Vector3.Distance(ray.origin, hit.transform.position);
        }
        return 1000;
    }


    GameObject CheckFloor(float rayLength)
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction, Color.yellow);

        if (Physics.Raycast(ray, out hit, rayLength, whatIsFloor))
        {
            return hit.transform.gameObject;
        }
        return null;
    }
    #endregion

    IEnumerator DeadAndRespawn(float time)
    {
        Debug.Log(dead);
        yield return new WaitForSeconds(time);
        dead = false;
        camFollow = true;
        Debug.Log(dead);
    }
}
