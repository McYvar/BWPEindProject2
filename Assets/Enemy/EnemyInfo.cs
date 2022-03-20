using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Enemy information")]
public class EnemyInfo : ScriptableObject
{
    public string enemyName;
    public int turns;
    public int health;
    public bool doesEnemyJump;
    public float attackRange;
}
