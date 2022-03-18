using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] EnemyInfo info;

    private int turns;

    public int healt { get; set; }

    FiniteStateMachine fsm;

    private void Start()
    {
        fsm = new FiniteStateMachine(typeof(EnemyIdleState), GetComponents<BaseState>());
        turns = info.turns;
        setHealth(info.health);
    }


    public int GetTurns()
    {
        return turns;
    }
    

    private void Update()
    {
        fsm.OnUpdate();
    }


    public void setHealth(int amount)
    {
        healt = amount;
    }

    public void takeDamage(int amount)
    {
        healt -= amount;
    }
}
