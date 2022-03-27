using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStates : MonoBehaviour
{
    public static bool startEnemyTurn;
    bool startPlayerTurn;
    public static bool enemyRunning;
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
    bool playerHasWon;

    Enemy activeEnemy;

    // text related stuff
    [SerializeField] TMP_Text TurnDisplay;

    private void Start()
    {
        playerHasWon = false;
        enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
        allEnemiesMoveAtTheSameTimeStatic = allEnemiesMoveAtTheSameTime;
        timeInBetweenMovesStatic = timeInBetweenMoves;
        timerIsRunning = false;
        startPlayerTurn = true;
        enemyRunning = false;
        anotherBool = false;
    }

    private void Update()
    {
        if (enemyQueue.Count <= 0 && !playerHasWon)
        {
            playerHasWon = true;
            EventManager.InvokeEvent(EventType.ON_PLAYER_WIN);
            return;
        }

        // Run once in the update function
        if (startPlayerTurn)
        {
            startPlayerTurn = false;
            StartCoroutine(PlayerTurnDisplay());
        }

        if (startEnemyTurn)
        {
            StartCoroutine(EnemyTurnDisplay());
            anotherBool = true;
            timerIsRunning = false;
            enemyRunning = true;
            ableToDequeue = true;
            startEnemyTurn = false;
            enemyQueue = TileBasedDungeonGeneration.TileBasedDungeonGeneration.enemyQueue;
            enemyCount = enemyQueue.Count;
            timer = 0;
        }

        if (!enemyRunning) return;

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
            enemyRunning = false;
            startPlayerTurn = true;
        }
        else if (timer < 0 && allEnemiesMoveAtTheSameTime) enemyCount = 0;
        else if (allEnemiesMoveAtTheSameTime) timer -= Time.deltaTime;
    }


    void DoEnemyTurn()
    {
        ableToDequeue = false;
        Enemy enemy = enemyQueue.Dequeue();
        if (enemy.isDead) return;
        enemy.isTurn = true;
        if (enemy.GetTurns() > timer && allEnemiesMoveAtTheSameTimeStatic)
        {
            timer = enemy.GetTurns();
            activeEnemy = enemy;
        }
        else if (!allEnemiesMoveAtTheSameTimeStatic) activeEnemy = enemy;
        enemyQueue.Enqueue(enemy);
    }


    void SetTimeForLastEnemy()
    {
        if (timerIsRunning) return;
        timerIsRunning = true;
        int count = enemyQueue.Count;
        for (int i = 0; i < count; i++)
        {
            DoEnemyTurn();
        }
        timer = (timer * timeInBetweenMoves) + 1;
    }


    IEnumerator EnemyTurnDisplay()
    {
        TurnDisplay.enabled = true;
        TurnDisplay.text = "Enemy Turn";
        yield return new WaitForSeconds(1);
        TurnDisplay.enabled = false;
    }


    IEnumerator PlayerTurnDisplay()
    {
        TurnDisplay.enabled = true;
        TurnDisplay.text = "PLayer Turn";
        yield return new WaitForSeconds(1);
        TurnDisplay.enabled = false;
    }
}
