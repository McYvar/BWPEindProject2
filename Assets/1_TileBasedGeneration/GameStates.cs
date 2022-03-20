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
    float timer = 0;
    bool timerIsRunning;

    Enemy enemyWithMostTurns;

    private void Start()
    {
        enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
        allEnemiesMoveAtTheSameTimeStatic = allEnemiesMoveAtTheSameTime;
        timeInBetweenMovesStatic = timeInBetweenMoves;
        timerIsRunning = false;
    }

    private void Update()
    {
        if (startEnemyTurn)
        {
            timerIsRunning = false;
            isRunning = true;
            ableToDequeue = true;
            startEnemyTurn = false;
            enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
            enemyCount = enemyQueue.Count;
            timer = 1;
        }

        if (!isRunning) return;

        if (ableToDequeue && enemyCount > 0 && !allEnemiesMoveAtTheSameTime)
        {
            DoEnemyTurn();
            enemyCount--;
        }

        Timer();

        if (allEnemiesMoveAtTheSameTime)
        {
            foreach (Enemy enemy in enemyQueue)
            {
                enemy.isTurn = true;
            }
        }
        Debug.Log(enemyWithMostTurns);

        if (enemyCount < 1 && !enemyWithMostTurns.isTurn)
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
        enemyQueue.Enqueue(enemy);
    }


    void Timer()
    {
        if (timerIsRunning) return;
        timerIsRunning = true;
        foreach (Enemy enemy in enemyQueue)
        {
            Debug.Log(enemy.enemyName);
            if (enemy.GetTurns() > timer)
            {
                timer = enemy.GetTurns();
                enemyWithMostTurns = enemy;
            }
        }
        timer = (timer * timeInBetweenMoves) + 3;
    }

}
