using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class SaveSystem
{
    public static void SaveScore(string name, int score)
    {
        string path = Application.persistentDataPath + "/Scoreboard.score";
        
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(name + "½" + score);
        writer.Close();
        writer.Dispose();
    }

    public static ScoreData[] LoadScore(int entryAmount)
    {
        string path = Application.persistentDataPath + "/Scoreboard.score";
        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);

            ScoreData[] data = new ScoreData[entryAmount];

            for (int i = 0; i < entryAmount; i++)
            {
                string temp = reader.ReadLine();
                string[] values = temp.Split('½');

                string name = values[0];
                int score = int.Parse(values[1]);
                data[i] = new ScoreData(values[0], int.Parse(values[1]));
            }

            reader.Close();
            reader.Dispose();

            return data;
        }
        else
        {
            Debug.LogError("File does not exist!");
            return null;
        }
    }

    public static int CountFileLines()
    {
        string path = Application.persistentDataPath + "/Scoreboard.score";
        using (StreamReader r = new StreamReader(path))
        {
            int i = 0;
            while (r.ReadLine() != null)
            {
                i++;
            }
            return i;
        }
    }
}
