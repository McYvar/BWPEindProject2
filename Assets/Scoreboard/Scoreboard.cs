using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    public int entryAmount;
    public static List<string> entryName;
    public static List<int> finalScore;

    public static int currentScore;

    [SerializeField] TMP_Text board;
    [SerializeField] TMP_Text onScreenScore;

    private void Start()
    {
        entryName = new List<string>();
        finalScore = new List<int>();

        entryAmount = SaveSystem.CountFileLines();
        if (entryAmount <= 0) return;
        LoadScore();
    }


    private void Update()
    {
        if (onScreenScore != null)
        {
            onScreenScore.text = "Score: " + currentScore;
            board.text = "Final score: " + currentScore;
            return;
        }

        board.text = "";
        if (entryName.Count <= 0)
        {
            board.text = "No entries yet!";
            return;
        }

        for (int i = 0; i < entryName.Count; i++)
        {
            board.text += entryName[i] + "............................................." + finalScore[i] + " points\n";
        }
    }


    public static void SaveScore(string name, int score)
    {
        entryName.Add(name);
        finalScore.Add(score);

        SaveSystem.SaveScore(name, score);
    }


    void LoadScore()
    {
        ScoreData[] data = SaveSystem.LoadScore(entryAmount);

        for (int i = 0; i < data.Length; i++)
        {
            entryName.Add(data[i].entryName);
            finalScore.Add(data[i].finalScore);
        }
    }
}



public class ScoreData
{
    public string entryName;
    public int finalScore;

    public ScoreData(string name, int score)
    {
        entryName = name;
        finalScore = score;
    }
}
