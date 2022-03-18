using System.Collections.Generic;
using UnityEngine;

public class GameStates : MonoBehaviour
{
    public static bool startEnemyTurn;
    public static bool isRunning;
    public static bool ableToDequeue;
    Queue<Enemy> enemyQueue;

    private void Start()
    {
        enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
    }

    private void Update()
    {
        if (startEnemyTurn)
        {
            isRunning = true;
            ableToDequeue = true;
            startEnemyTurn = false;
            enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
        }

        if (!isRunning) return;

        Debug.Log(enemyQueue.Count);

        if (ableToDequeue && enemyQueue.Count > 0)
        {
            ableToDequeue = false;
            Enemy enemy = enemyQueue.Dequeue();
            enemy.isTurn = true;
            enemyQueue.Enqueue(enemy);
        }

        if (enemyQueue.Count < 1)
        {
            TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue = enemyQueue;
            isRunning = false;
        }
    }

}
