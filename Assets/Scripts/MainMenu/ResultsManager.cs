using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameStats
{
    public string gameName;
    public int iconPath;
    public string dateTimePlayed;
    public int attempted;
    public int correct;
    public int incorrect;
    public float percentage;
}

public class ResultsManager : MonoBehaviour
{
    private static ResultsManager instance;
    [SerializeField] private List<GameStats> gameStatsList = new List<GameStats>();
    private const string PlayerPrefsKey = "GameStats";

    public static ResultsManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadStats();
    }

    private void LoadStats()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            gameStatsList = JsonUtility.FromJson<GameStatsList>(json)?.statsList ?? new List<GameStats>();
        }
    }

    private void SaveStats()
    {
        GameStatsList wrapper = new GameStatsList { statsList = gameStatsList };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    public void UpdateStats(string gameName, int iconPath, int attempted, int correct)
    {
        GameStats newStat = new GameStats
        {
            gameName = gameName,
            iconPath = iconPath,
            dateTimePlayed = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            attempted = attempted,
            correct = correct,
            incorrect = attempted - correct,
            percentage = attempted > 0 ? (correct / (float)attempted) * 100f : 0f
        };

        gameStatsList.Insert(0, newStat);
        if (gameStatsList.Count > 50)
        {
            gameStatsList.RemoveAt(50);
        }

        SaveStats();
    }

    public List<GameStats> GetRecentStats()
    {
        return gameStatsList;
    }
}

[System.Serializable]
public class GameStatsList
{
    public List<GameStats> statsList = new List<GameStats>();
}
