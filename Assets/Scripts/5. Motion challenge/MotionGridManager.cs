using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MotionGridManager : MonoBehaviour
{
    public LevelsDifficulty[] levelsDifficulty;
    public int CurrentLevel;
    public int LevelDifficulty = 1; //Level Difficulty (1 - 10) Default difficulty is 1
    public int DifficultyCount; //Its responsible after how many current difficulty the
                         //next difficulty will be loaded

    [Space]
    int grid4levels;

    [Space]
    public int MovesDone = 0;
    public TextMeshProUGUI text_MoveDone;
    public int PuzzleSolved= 0;

    [Space]
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI timer_Text;
    public float MaxTime = 300f;
    public float timeRemaining = 300f;
    private bool isTimerRunning = true;
    public Slider slider_timer;

    [Header("Custom Colors")]
    public Color[] ObstaclesColors;
    [HideInInspector] public List<int> ColorsUsedList = new List<int>();


    [Header("Custom levelPlay")]
    public bool PlaycustomLevel = false;
    public bool isEditor = false;
    public int CustomLevel = 1;
    public int TotalLevels;
    [SerializeField]private int lastlevelPlayed;

    [Header("Custom MovementSettings")]
    public Slider lerpSpeed;
    public TextMeshProUGUI lerpSpeedText;

    public Slider MoveSpeed;
    public TextMeshProUGUI MoveSpeedText;

    //variables for storing levels played and remaining levels

    [SerializeField] private List<int> availableLevels;
    private HashSet<int> playedLevels;



    public static MotionGridManager Instance { get; private set; }
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        InitializeLevelData();
        UnityEngine.Debug.unityLogger.filterLogType = LogType.Log;
        //Above two fields are just for checking the level difficulty
        if (isEditor)
        {
            lerpSpeed.value = 15;
            MoveSpeed.value = 15;
        }
        else
        {
            lerpSpeed.value = 10;
            MoveSpeed.value = 5;
        }
        
        isTimerRunning = false;
        slider_timer.maxValue = MaxTime;
        CurrentLevel = 1;


       
    }

    void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Decrease time by delta time
              //  slider_timer.value = timeRemaining;
            //    UpdateTimerDisplay();
            }
            else
            {
                isTimerRunning = false;
                timeRemaining = 0;
            //    UpdateTimerDisplay();
                GameOver();
                Debug.Log("Timer has ended.");
            }
        }
    }

    public void GameOver()
    {
        playedLevels.Clear();
        SceneManager.LoadScene("5. Motion challenge");
        Destroy(gameObject);
    //    StartCoroutine(StartNewGame());
    }
    public void LevelSkip()
    {
        if(LevelDifficulty >1)
            LevelDifficulty--;   
        Debug.Log("Skip Level");
        CurrentLevel++;
        StartMotionGame();
    }

    public void UpdatemovesDone()
    {
        MovesDone++;
        text_MoveDone.text = MovesDone.ToString();
    }

    IEnumerator StartNewGame()
    {

        Debug.Log("Solved Puzzled " + PuzzleSolved);
        yield return new WaitForSeconds(1f);
        StartMotionGame();


        yield return null;
    }

    void UpdateTimerDisplay()
    {
        // Convert seconds into minutes and seconds format
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Update the Text component
        timer_Text.text = string.Format("{0:00}:{1:00}",minutes, seconds);
    }

     public  void StartMotionGame()
     {
        
        ColorsUsedList.Clear();
        if (PlaycustomLevel)
        {
            grid4levels = CustomLevel;
            SceneManager.LoadScene("Level" + grid4levels);
            return;
        }

        MovesDone = 0;
        //    text_MoveDone.text = MovesDone.ToString();
        /*    if (PlayerlevelsList.Count == TotalLevels-1)
            {
                Debug.Log("All levels are completed");
                PlayerlevelsList.Clear();
            }
        */

        //------------------------------------------------------------------------//

        if (availableLevels.Count > 0)
        {
            LoadRandomLevel();
        }
        else
        {
            Debug.Log("All levels have been played!");
            // Handle all levels played logic, like restarting or showing a message.
        }
    }

    void InitializeLevelData()
    {
        int totalLevels = TotalLevels; // Set the total number of levels.
        availableLevels = new List<int>();
        playedLevels = new HashSet<int>();

        // Populate available levels
        for (int i = 1; i < totalLevels; i++)
        {
            availableLevels.Add(i);
        }
    }

    void LoadRandomLevel()
    {
        bool levelFound = false;
        int randomIndex = -1;

        // Loop to find a level matching the desired difficulty
        for (int i = 0; i < availableLevels.Count; i++)
        {
            randomIndex = UnityEngine.Random.Range(1, availableLevels.Count);

            // Check if the selected level matches the difficulty
            if (levelsDifficulty[randomIndex].LevelDifficulty == LevelDifficulty)
            {
                levelFound = true;
                availableLevels.Remove(levelsDifficulty[randomIndex].LevelNo);
                break;
            }
        }

        if (levelFound)
        {
            int leveltoLoad = levelsDifficulty[randomIndex].LevelNo;
            // Mark level as played
            playedLevels.Add(leveltoLoad);
            
            Debug.Log("Difficulty Level " + LevelDifficulty);
            Debug.Log("Loading level: " + leveltoLoad);
            if(lastlevelPlayed == leveltoLoad)
            {
                Debug.LogError("Level already played");
                LoadRandomLevel();
            }
            else
            {
                lastlevelPlayed = leveltoLoad;
                // Load the selected level
                isTimerRunning = true;
                SceneManager.LoadScene("Level" + leveltoLoad);
            }
            
        }
        else
        {
            Debug.LogWarning("No eligible levels found matching the difficulty and not already played.");
        }
    }

    public void UpdateLerpSpeed()
    {
        lerpSpeedText.text = "Lerp Speed : "+ lerpSpeed.value.ToString();
        MoveSpeedText.text = "Max Speed : " + MoveSpeed.value.ToString();
        PlayerPrefs.SetFloat("LerpSpeed", lerpSpeed.value);
        PlayerPrefs.SetFloat("MaxSpeed", MoveSpeed.value);
    } 
}


[System.Serializable]
public class LevelsDifficulty
{
    public int LevelNo;
    public int LevelDifficulty;
    public int movestocompleteLevel;
}
