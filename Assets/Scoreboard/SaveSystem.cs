using System.IO;
using UnityEngine;

public class SaveSystem
{
    public static void SaveScore(string name, int score)
    {
        string path = Application.persistentDataPath + "/Scoreboard.score";

        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(name + "?" + score);
        writer.Close();
        writer.Dispose();
    }

    public static ScoreData[] LoadScore(int entryAmount)
    {
        string path = Application.persistentDataPath + "/Scoreboard.score";
        if (!File.Exists(path)) return null;

        StreamReader reader = new StreamReader(path);

        ScoreData[] data = new ScoreData[entryAmount];

        for (int i = 0; i < entryAmount; i++)
        {
            string temp = reader.ReadLine();
            string[] values = temp.Split('?');

            string name = !string.IsNullOrWhiteSpace(values[0]) && values[0].Length > 5
                            ? values[0].Substring(0, 6)
                            : values[0];
            int score = int.Parse(values[1]);
            data[i] = new ScoreData(name, score);
        }

        reader.Close();
        reader.Dispose();

        return data;
    }

    public static int CountFileLines()
    {
        string path = Application.persistentDataPath + "/Scoreboard.score";
        if (!File.Exists(path)) return 0;

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
