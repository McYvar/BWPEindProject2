using System.Collections;
using UnityEngine;

public class PlayerTurnState : MovementBase
{
    GameObject cam;
    public int playerHasTurns;
    float deadZone = 0.7f;
    
    public override void OnAwake()
    {
        currentDirection = up;
        nextPos = transform.forward;
        destination = transform.position;
        speed = normalSpeed;
        spawn = transform.position;

        cam = FindObjectOfType<CameraBehaviour>().gameObject;
    }


    public override void OnEnter()
    {
        turns = playerHasTurns;
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

        if (PlayerInput.westPressed) speed = 2 * normalSpeed;
        else speed = normalSpeed;
        base.InputCheck();
    }

}
