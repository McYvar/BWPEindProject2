using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : MovementBase
{
    Enemy enemy;
    public override void OnAwake()
    {
        enemy = GetComponent<Enemy>();
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

        if (enemy.isTurn) stateManager.SwitchState(typeof(EnemyTurnState));
    }


    public override void Move()
    {
        base.Move();
    }
}
