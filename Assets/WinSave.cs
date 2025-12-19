using System.IO;
using UnityEngine;

public class WinSave : MonoBehaviour
{
    private static string path => Path.Combine(Application.persistentDataPath, "wins.json");

    // Save total wins
    public static void SaveWins(int totalWins)
    {
        PlayerData data = new PlayerData();
        data.AmsterdamWins = totalWins;

        string json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
        Debug.Log("Wins saved! Path: " + path);
    }

    // Load total wins
    public static int LoadWins()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            return data.AmsterdamWins;
        }
        else
        {
            return 0; // default if no save exists
        }
    }
}
