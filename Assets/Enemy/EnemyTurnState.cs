using UnityEngine;

public class EnemyTurnState : BaseState
{
    Enemy enemy;
    CameraBehaviour cam;

    float timer;
    int turns;

    public override void OnAwake()
    {
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
        if (timer > GameStates.timeInBetweenMovesStatic && Vector3.Distance(cam.camPosition, transform.position) < 2f)
        {
            Move();
            timer = 0;
            turns--;
        }
        else timer += Time.deltaTime;
        if (turns <= 0)
        {
            enemy.isTurn = false;
            stateManager.SwitchState(typeof(EnemyIdleState));
        }
        if (!GameStates.allEnemiesMoveAtTheSameTimeStatic) cam.GetComponent<CameraBehaviour>().objectToFollow = gameObject;
    }


    public void Move()
    {

    }
}
