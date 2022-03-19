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

    private void Start()
    {
        enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
        allEnemiesMoveAtTheSameTimeStatic = allEnemiesMoveAtTheSameTime;
        timeInBetweenMovesStatic = timeInBetweenMoves;
        timerIsRunning = true;
    }

    private void Update()
    {
        if (startEnemyTurn)
        {
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

        if (allEnemiesMoveAtTheSameTime && !timerIsRunning)
        {
            timerIsRunning = true;
            foreach (Enemy enemy in enemyQueue)
            {
                if (enemy.GetTurns() > timer) timer = enemy.GetTurns();
            }
            timer *= timeInBetweenMoves;
        }

        if (allEnemiesMoveAtTheSameTime)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                DoEnemyTurn();
            }
        }

        if (enemyCount < 1)
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

}
