using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeductiveGameManager : MonoBehaviour
{
    public TextMeshProUGUI timer_Text;
    private float timeRemaining = 300f;
    private bool isTimerRunning = true;

    public GridLayoutGroup grid;
    public GridLayoutGroup AnsGrid;
    public GridLayoutGroup OptionsAnsGrids;

    [HideInInspector]   public int CorrectAns;
    [HideInInspector]   public int QuestiosnAttempted;
    
    LevelLoader4 levelLoader4;
    LevelLoader5 levelLoader5;

    List<int> PlayerlevelsList =  new List<int>();

    public static DeductiveGameManager instance;

    
    int grid4levels ;
    int grid5levels ;


    int LevelPlayed;

    public GameObject ResultsPanel;
    public TextMeshProUGUI ResultsText;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        CorrectAns = 0; QuestiosnAttempted = 0; // Intialize the Correct Answers and Questions Attempted to Zero
        levelLoader4 = LevelLoader4.instance;
        levelLoader5 = LevelLoader5.instance;
        PlayerlevelsList.Clear();
        ResultsPanel.SetActive(false);
        GameStart();
        //  StartCoroutine(StartNewGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Decrease time by delta time
                UpdateTimerDisplay();
            }
            else
            {
                isTimerRunning = false;
                timeRemaining = 0;
                UpdateTimerDisplay();
                ShowResults();
                Debug.Log("Timer has ended.");
            }
        }
    }

    void GameStart()
    { 
        if (LevelPlayed<3)
        {
            LevelPlayed++;
            grid.GetComponent<GridLayoutGroup>().constraintCount = 4;
            AnsGrid.GetComponent<GridLayoutGroup>().constraintCount = 4;
            OptionsAnsGrids.GetComponent<GridLayoutGroup>().constraintCount = 5;

            grid4levels = Random.Range(1, 33);  // select a random level of 4*4 Grid
            while (PlayerlevelsList.Contains(grid4levels))
            {
                grid4levels = Random.Range(1, 33);  // select a random level of 4*4 Grid
            }
            Debug.Log("Running 4*4 Grid and Level No: " + grid4levels);

            PlayerlevelsList.Add(grid4levels);
            levelLoader4.LoadLevelData(grid4levels);

        }
        else
        {
            LevelPlayed++;
            grid.GetComponent<GridLayoutGroup>().constraintCount = 5;
            AnsGrid.GetComponent<GridLayoutGroup>().constraintCount = 5;
            OptionsAnsGrids.GetComponent<GridLayoutGroup>().constraintCount = 6;
            grid5levels = Random.Range(33, 68);  // select a random level of 5*5 Grid

            while (PlayerlevelsList.Contains(grid5levels))
            {
                grid5levels = Random.Range(33, 68);   // select a random level of 4*4 Grid
            }
            PlayerlevelsList.Add(grid5levels);

            Debug.Log("Running 5*5 Grid and Level No: " + grid5levels);
            levelLoader5.LoadLevelData(grid5levels);

            if(LevelPlayed>8)
                LevelPlayed = 0;
        }
    }

    void UpdateTimerDisplay()
    {
        // Convert seconds into minutes and seconds format
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Update the Text component
        timer_Text.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void GameOver()
    {
        StartCoroutine(StartNewGame());
    }


    IEnumerator StartNewGame()
    {
        yield return new WaitForSeconds(1f);
        GameStart();
        yield return null;
    }

    public void ShowResults()
    {
        // Perform floating-point division
        float percentage = ((float)CorrectAns / QuestiosnAttempted) * 100f;

        Debug.Log("Correct Answers: " + CorrectAns + " Total Questions Attempted: " + QuestiosnAttempted + " Percentage: " + percentage);

        // Format the result as a percentage
        string result = percentage.ToString() + "%"; // "F2" limits to 2 decimal places
        ResultsText.text = result;
        ResultsPanel.SetActive(true);
    }

    public void AgainStartGame()
    {
        ResultsPanel.SetActive(false);
        timeRemaining = 300f;
        isTimerRunning = true;
        StartCoroutine(StartNewGame());
    }
    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }

}
