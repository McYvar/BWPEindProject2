using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    ON_ENEMY_KILLED = 0,
}

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] EnemyInfo info;

    public string enemyName;
    private int turns;
    public bool isTurn;
    public bool isJumperEnemy;
    public float attackRange;
    public float detectRange;
    public int dealsDamage;

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
        dealsDamage = info.dealsDamage;
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
        Debug.Log("enemy took: " + amount + " damage!");
    }

}
