using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : BaseState
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
        if (enemy.isTurn) stateManager.SwitchState(typeof(EnemyTurnState));
    }
}
