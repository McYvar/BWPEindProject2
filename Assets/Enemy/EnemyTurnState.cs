using UnityEngine;

public class EnemyTurnState : MovementBase
{
    GameObject player;
    Enemy enemy;
    CameraBehaviour cam;
    bool doAttack;

    float timer;

    public override void OnAwake()
    {
        player = FindObjectOfType<Player>().gameObject;
        enemy = GetComponent<Enemy>();
        cam = FindObjectOfType<CameraBehaviour>();
    }


    public override void OnEnter()
    {
        timer = 0;
        turns = enemy.GetTurns();
        doAttack = false;
    }


    public override void OnExit()
    {
        GameStates.ableToDequeue = true;
    }


    public override void OnUpdate()
    {
        if (turns <= 0 && !moving)
        {
            enemy.isTurn = false;
            stateManager.SwitchState(typeof(EnemyIdleState));
        }
        if (!GameStates.allEnemiesMoveAtTheSameTimeStatic) cam.GetComponent<CameraBehaviour>().objectToFollow = gameObject;

        Move();

    }


    public override void Move()
    {
        if (timer > GameStates.timeInBetweenMovesStatic)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= enemy.attackRange && CanSeePlayer())
            {
                turns = 0;
                doAttack = true;
                player.GetComponent<IDamagable>().takeDamage(enemy.dealsDamage);
            }
            base.Move();

            if (!moving) timer = 0;
        }
        else timer += Time.deltaTime;
    }


    public override void InputCheck()
    {
        if (doAttack) return;

        canJump = enemy.isJumperEnemy;
        float length = canJump ? 2.1f : 1.1f;
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance <= turns + enemy.attackRange && CanSeePlayer())
        {
            float tempX = player.transform.position.x - transform.position.x;
            float tempY = player.transform.position.z - transform.position.z;


            if (tempX == 0) // means that player is in line with x
            {
                if (tempY > 0 && CheckUp(length)) { GoUp(); return; }
                if (tempY < 0 && CheckDown(length)) { GoDown(); return; }
            }

            if (tempY == 0) // means that player is in line with y
            {
                if (tempX > 0 && CheckRight(length)) { GoRight(); return; }
                if (tempX < 0 && CheckLeft(length)) { GoLeft(); return; }
            }

            if (tempY < tempX) // means that player is closer on y then on x
            {
                if (tempY > 0 && CheckUp(length)) { GoUp(); return; }
                if (tempY < 0 && CheckDown(length)) { GoDown(); return; }
            }

            if (tempX < tempY) // means that player is closer on x then on y
            {
                if (tempX > 0 && CheckRight(length)) { GoRight(); return; }
                if (tempX < 0 && CheckLeft(length)) { GoLeft(); return; }
            }
        }

        if (canMove) return;

        int r = Random.Range(0, 4);

        switch (r)
        {
            case 0: if (CheckUp(length)) { GoUp(); } break;
            case 1: if (CheckDown(length)) { GoDown(); } break;
            case 2: if (CheckRight(length)) { GoRight(); } break;
            case 3: if (CheckLeft(length)) { GoLeft(); } break;
        }
    }


    bool CanSeePlayer()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, player.transform.position - transform.position);
        Debug.DrawRay(ray.origin, ray.direction * enemy.detectRange, Color.yellow);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Player")) return true;
        }
        return false;
    }


    bool CheckUp(float lenght)
    {
        return ValidityCheck(transform.position, transform.forward, lenght, whatIsWall);
    }


    bool CheckDown(float lenght)
    {
        return ValidityCheck(transform.position, -transform.forward, lenght, whatIsWall);
    }


    bool CheckRight(float lenght)
    {
        return ValidityCheck(transform.position, transform.right, lenght, whatIsWall);
    }


    bool CheckLeft(float lenght)
    {
        return ValidityCheck(transform.position, -transform.right, lenght, whatIsWall);
    }
}
