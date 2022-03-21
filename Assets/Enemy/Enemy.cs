using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] EnemyInfo info;

    public string enemyName;
    private int turns;
    public bool isTurn;
    public bool isJumperEnemy;
    public float attackRange;
    public float detectRange;

    public int healt { get; set; }

    public FiniteStateMachine fsm;

    private void Start()
    {
        fsm = new FiniteStateMachine(typeof(EnemyIdleState), GetComponents<BaseState>());
        turns = info.turns;
        setHealth(info.health);
        enemyName = info.enemyName;
        attackRange = info.attackRange;
        isJumperEnemy = info.doesEnemyJump;
        detectRange = info.detectRange;
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
