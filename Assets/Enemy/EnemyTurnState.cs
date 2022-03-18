using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnState : BaseState
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
        for (int i = 0; i < enemy.GetTurns(); i++)
        {
            Debug.Log("running: " + i);
        }
    }
}
