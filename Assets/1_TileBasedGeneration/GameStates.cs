using System.Collections.Generic;
using UnityEngine;

public class GameStates : MonoBehaviour
{
    public static bool startEnemyTurn;
    public static bool isRunning;
    public static bool ableToDequeue;
    [SerializeField] bool allEnemiesMoveAtTheSameTime;
    public static bool allEnemiesMoveAtTheSameTimeStatic;
    public float timeInBetweenMoves;
    public static float timeInBetweenMovesStatic;
    Queue<Enemy> enemyQueue;
    int enemyCount;
    float timer;
    bool timerIsRunning;
    bool anotherBool;

    Enemy activeEnemy;

    private void Start()
    {
        enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
        allEnemiesMoveAtTheSameTimeStatic = allEnemiesMoveAtTheSameTime;
        timeInBetweenMovesStatic = timeInBetweenMoves;
        timerIsRunning = false;
    }

    private void Update()
    {
        // Run once in the update function
        if (startEnemyTurn)
        {
            anotherBool = true;
            timerIsRunning = false;
            isRunning = true;
            ableToDequeue = true;
            startEnemyTurn = false;
            enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
            enemyCount = enemyQueue.Count;
            timer = 0;
        }

        if (!isRunning) return;

        // If enemies move one by one, they have to wait for each other to finish their moves
        if (ableToDequeue && enemyCount > 0 && !allEnemiesMoveAtTheSameTime)
        {
            DoEnemyTurn();
            enemyCount--;
        }

        // If enemies move all at the same time, then a timer is used for the enemy that has the longest turns
        SetTimeForLastEnemy();

        // Then all enemies get to move at the same time
        if (allEnemiesMoveAtTheSameTime && anotherBool)
        {
            anotherBool = false;
            ableToDequeue = false;
            foreach (Enemy enemy in enemyQueue)
            {
                enemy.isTurn = true;
            }
        }

        // Once the enemy count is set to 0, and the last/longest active enemy has finished its turn, then this piece of code stops running
        if (enemyCount < 1 && !activeEnemy.isTurn)
        {
            TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue = enemyQueue;
            isRunning = false;
        }
        else if (timer < 0 && allEnemiesMoveAtTheSameTime) enemyCount = 0;
        else if (allEnemiesMoveAtTheSameTime) timer -= Time.deltaTime;
    }


    void DoEnemyTurn()
    {
        ableToDequeue = false;
        Enemy enemy = enemyQueue.Dequeue();
        enemy.isTurn = true;
        activeEnemy = enemy;
        enemyQueue.Enqueue(enemy);
    }


    void SetTimeForLastEnemy()
    {
        if (timerIsRunning) return;
        timerIsRunning = true;
        foreach (Enemy enemy in enemyQueue)
        {
            if (enemy.GetTurns() > timer)
            {
                timer = enemy.GetTurns();
                activeEnemy = enemy;
            }
        }
        timer = (timer * timeInBetweenMoves) + 3;
    }

}
