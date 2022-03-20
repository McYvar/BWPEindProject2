using UnityEngine;

public class EnemyTurnState : MovementBase
{
    GameObject player;
    Enemy enemy;
    CameraBehaviour cam;

    float timer;
    [SerializeField] LayerMask whatIsPlayer;

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
    }


    public override void OnExit()
    {
        GameStates.ableToDequeue = true;
    }


    public override void OnUpdate()
    {
        Move();

        if (turns <= 0 && !moving)
        {
            enemy.isTurn = false;
            stateManager.SwitchState(typeof(EnemyIdleState));
        }
        if (!GameStates.allEnemiesMoveAtTheSameTimeStatic) cam.GetComponent<CameraBehaviour>().objectToFollow = gameObject;
    }


    public override void Move()
    {
        if (timer > GameStates.timeInBetweenMovesStatic)
        {
            if (Vector3.Distance(transform.position, player.transform.position) < enemy.attackRange)
            {
                // do attack
                turns--;
            }
            else
            {
                //base.Move();
            }

            if (!moving) timer = 0;
        }
        else timer += Time.deltaTime;
    }


    public override void InputCheck()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < turns + enemy.attackRange && CanSeePlayer())
        {
            float tempX = player.transform.position.x - transform.position.x;
            float tempY = player.transform.position.z - transform.position.z;

            if (tempX == 0)
            {
                if (tempY > 0) GoUp();
                if (tempY < 0) GoDown();
            }

            if (tempY == 0)
            {
                if (tempX > 0) GoRight();
                if (tempX < 0) GoLeft();
            }

            if (tempX < tempY && CheckForward()) // means that player is closer on x then on y
            {
                
            }
        }

        canJump = enemy.isJumperEnemy;
    }


    bool CanSeePlayer()
    {
        Ray ray = new Ray(transform.position, player.transform.position - transform.position);
        Debug.DrawRay(ray.origin, ray.direction * enemy.attackRange, Color.yellow);
        if (Physics.Raycast(ray, enemy.attackRange, whatIsPlayer, QueryTriggerInteraction.UseGlobal))
        {
            return true;
        }
        return false;
    }
}
