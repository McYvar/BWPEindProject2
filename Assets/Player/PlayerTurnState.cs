using UnityEngine;

public class PlayerTurnState : MovementBase
{
    CameraBehaviour cam;
    public int playerHasTurns;
    float deadZone = 0.7f;

    public override void OnAwake()
    {
        currentDirection = up;
        nextPos = transform.forward;
        speed = normalSpeed;
        spawn = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        destination = spawn;

        cam = FindObjectOfType<CameraBehaviour>();
    }


    public override void OnEnter()
    {
        turns = playerHasTurns;
        canMove = false;
    }


    public override void OnExit()
    {

    }


    public override void OnUpdate()
    {
        Move();
        if (turns <= 0 && !moving) stateManager.SwitchState(typeof(PlayerInEnemyTurn));

        cam.GetComponent<CameraBehaviour>().objectToFollow = gameObject;
    }


    void CameraCheck()
    {
        if ((cam.cam.transform.localEulerAngles.y > 0 && cam.cam.transform.localEulerAngles.y <= 45) || (cam.cam.transform.localEulerAngles.y > 315 && cam.cam.transform.localEulerAngles.y <= 360))
        {
            up = Vector3Int.zero;
            right = new Vector3Int(0, 90, 0);
            down = new Vector3Int(0, 180, 0);
            left = new Vector3Int(0, 270, 0);
        }
        else if (cam.cam.transform.localEulerAngles.y > 45 && cam.cam.transform.localEulerAngles.y <= 135)
        {
            left = Vector3Int.zero;
            up = new Vector3Int(0, 90, 0);
            right = new Vector3Int(0, 180, 0);
            down = new Vector3Int(0, 270, 0);
        }
        else if (cam.cam.transform.localEulerAngles.y > 135 && cam.cam.transform.localEulerAngles.y <= 225)
        {
            down = Vector3Int.zero;
            left = new Vector3Int(0, 90, 0);
            up = new Vector3Int(0, 180, 0);
            right = new Vector3Int(0, 270, 0);
        }
        else if (cam.cam.transform.localEulerAngles.y > 225 && cam.cam.transform.localEulerAngles.y <= 315)
        {
            right = Vector3Int.zero;
            down = new Vector3Int(0, 90, 0);
            left = new Vector3Int(0, 180, 0);
            up = new Vector3Int(0, 270, 0);
        }
    }


    public override void Move()
    {
        base.Move();
    }
    
    public override void InputCheck()
    {
        if (!moving && !dead)
        {
            if (PlayerInput.leftJoy.y > deadZone)
            {
                CameraCheck();
                GoUp();
            }
            if (PlayerInput.leftJoy.x > deadZone)
            {
                CameraCheck();
                GoRight();
            }
            if (PlayerInput.leftJoy.y < -deadZone)
            {
                CameraCheck();
                GoDown();
            }
            if (PlayerInput.leftJoy.x < -deadZone)
            {
                CameraCheck();
                GoLeft();
            }
            if (PlayerInput.southPressed)
            {
                PlayerInput.southPressed = false;
                CameraCheck();
                DoJump();
            }
            if (PlayerInput.leftShoulderPressed)
            {
                PlayerInput.leftShoulderPressed = false;
                CameraCheck();
                RotateLeft();
            }
            if (PlayerInput.rightShoulderPressed)
            {
                PlayerInput.rightShoulderPressed = false;
                CameraCheck();
                RotateRight();
            }
        }

        if (PlayerInput.westPressed) speed = 2 * normalSpeed;
        else speed = normalSpeed;
    }


    public override void Deadbehaviour()
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

}
