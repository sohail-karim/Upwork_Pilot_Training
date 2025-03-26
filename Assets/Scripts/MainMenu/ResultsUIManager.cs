using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultsUIManager : MonoBehaviour
{
    public Transform statsContainer;  // Parent object for stats items
    public GameObject statsPrefab;    // Prefab for displaying each game stat
    public Sprite[] gameIcons;         // Icons for each game

    private void Start()
    {

    }
    private void OnEnable()
    {
        DisplayStats();
    }

    public void DisplayStats()
    {
        // Clear old stats
        foreach (Transform child in statsContainer)
        {
            Destroy(child.gameObject);
        }

        // Get latest stats
        List<GameStats> recentStats = ResultsManager.Instance.GetRecentStats();
        Debug.Log(recentStats.Count);
        foreach (GameStats stat in recentStats)
        {
            GameObject statItem = Instantiate(statsPrefab, statsContainer);
            // Debugging to ensure prefab has correct child names
            foreach (Transform child in statItem.transform)
            {
                Debug.Log($"Child Name: {child.name}");
            }
            StatItemUI statUI = statItem.GetComponent<StatItemUI>();
            statUI.gameNameText.text = stat.gameName;
            statUI.dateTimeText.text = stat.dateTimePlayed;
            statUI.attemptedText.text = $"Attempted: {stat.attempted}";
            statUI.correctText.text = $"Correct: {stat.correct}";
            statUI.incorrectText.text = $"Incorrect: {stat.incorrect}";
            statUI.percentageText.text = $"{(int)stat.percentage}%";
            // Load Icon
            statUI.iconImage.sprite = gameIcons[stat.iconPath];
        }
    }
}
