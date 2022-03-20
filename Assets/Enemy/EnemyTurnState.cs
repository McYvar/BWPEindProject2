using UnityEngine;

public class EnemyTurnState : MovementBase
{
    Enemy enemy;
    CameraBehaviour cam;

    float timer;

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
        Move();

        if (turns <= 0)
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
            base.Move();
            if (!moving) timer = 0;
        }
        else timer += Time.deltaTime;
    }


    public override void InputCheck()
    {
        GoLeft();
    }
}
