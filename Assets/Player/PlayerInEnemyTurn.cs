using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInEnemyTurn : BaseState
{
    public override void OnAwake()
    {
    }


    public override void OnEnter()
    {
        GameStates.startEnemyTurn = true;
        Debug.Log("entered other state");
    }


    public override void OnExit()
    {
    }


    public override void OnUpdate()
    {
        if (!GameStates.isRunning) stateManager.SwitchState(typeof(PlayerTurnState));
    }
}
