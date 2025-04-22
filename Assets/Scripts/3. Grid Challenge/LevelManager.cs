using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject circleScreen; // Reference to CircleScreen
    public GameObject gridScreen;   // Reference to GridScreen
    public GameObject RotationScreen;  //Reference to RotationScreen
    public GameObject AdditionScreen;  // Reference to AdditionScreen
    public GameObject SubstractionScreen;  // Reference to SubstractionScreen

    [Space]
    public GameObject circle_Grid_Rotation;
    public GameObject Addition_Substraction;

    [Header("CountDownTimer")]
    [SerializeField] int _CountDownTimer = 5; // Count down timer before game starts..
    [SerializeField] TextMeshProUGUI _CountDownTimerText;
    [SerializeField] GameObject _StartPanel; //CountDown Timer Panel..



    [Space]
    public Slider timer_Slider;
    public TextMeshProUGUI timer_Text;
    public TextMeshProUGUI Level_txt;

    CirclePatternManager CircleGameManager;           // Reference to CirclePatternManager script
    PatternGameManager patternManager; // Reference to PatternGameManager script
    RotationPatternManager rotationPatternManager;
    AdditionGridManager additionScreen;
    SubstractionGridManager substractionScreen;

    [Space]
    public Image gridResultImg;
    public Image rotationResultImg;
    public Image AdditionResultImg;
    public Image SubstractionResultImg;


    public Sprite img_Tick;
    public Sprite img_False;
    public float countdownTime;


    public bool TimerCheck = true;


    public int totalLevels; // Total number of levels in the game
    [SerializeField]
    private int currentLevel; // Tracks the current level

    private bool isCircleScreenActive = true; // Toggles between CircleScreen and GridScreen
    private bool isCalledFromGridScreen = false;

    bool isResultScreen = false;
    public int timesCircleScreenShown = 0; // Tracks number of CircleScreens shown
    public int timesGridScreenShown = 0; // Tracks number of GridScren is shown

    public List<Levels> levels = new List<Levels>();

    public static LevelManager instance;
    Levels currentLevelData;


    [Space]
    public GameObject resultsPanel;
    [Header("Results Settings")]
    [SerializeField] private int attemptedQuestions;
    [SerializeField] private int CorrectAnswers;
    public GameObject  Topbar, ContinuPanel,Attempted, Correct, Percentageobj , continuebutton,mainmenubutton;
    public TextMeshProUGUI ResultsText,ContinuePanelText;

    public TextMeshProUGUI ScoresPercentage;

    [Space]
    public GameObject NoSubmitScreen;


    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        instance = this;
    }

    public void setScores(int correct)
    {
        CorrectAnswers += correct;
    }

    void Start()
    {

        CircleGameManager = CirclePatternManager.instance;
        patternManager = PatternGameManager.instance;
        rotationPatternManager = RotationPatternManager.instance;
        additionScreen = AdditionGridManager.instance;
        substractionScreen = SubstractionGridManager.instance;
        totalLevels = levels.Count;
        currentLevel = PlayerPrefs.GetInt("currentLevel", 0);
        Level_txt.text = "Level " + currentLevel.ToString();
        StartCoroutine(CountDownTimer());
        _CountDownTimer = 5;
        _CountDownTimerText.text = _CountDownTimer.ToString();

    }

    IEnumerator CountDownTimer()
    {
        while (_CountDownTimer > 0)
        {
            Debug.Log("CountDownTimer: " + _CountDownTimer);
            _CountDownTimerText.text = _CountDownTimer.ToString(); // Update UI first
            yield return new WaitForSeconds(1); // Then wait
            _CountDownTimer--;
        }
        _StartPanel.SetActive(false);


        resultsPanel.SetActive(false);
        NoSubmitScreen.SetActive(false);
        RotationScreen.SetActive(false);
        AdditionScreen.SetActive(false);
        SubstractionScreen.SetActive(false);
        StartLevel(currentLevel);

    }


    public void StartLevel(int CurrentLevelNo)
    {
        resultsPanel.SetActive(false);
        NoSubmitScreen.SetActive(false);

        int temp = PlayerPrefs.GetInt("currentLevel", 0) + 1;


        if (temp > 11)
        {
            Debug.Log("Random Level Generated");
            currentLevelData = GenerateCustomLevel(currentLevel);
        }
        else
        {
            currentLevelData = levels[currentLevel];
        }


        Level_txt.text = "Level " + temp.ToString();
        circle_Grid_Rotation.SetActive(false);
        Addition_Substraction.SetActive(false);
        gridResultImg.enabled = false;
        rotationResultImg.enabled = false;
        AdditionResultImg.enabled = false;
        SubstractionResultImg.enabled = false;

        timesCircleScreenShown++;
        isCircleScreenActive = false;
        isResultScreen = false;
        ActivateCircleScreen(currentLevelData.LevelNo);
    }

    private void ActivateCircleScreen(int CurrentLevelNo)
    {
        gridScreen.SetActive(false);
        circle_Grid_Rotation.SetActive(true);
        Addition_Substraction.SetActive(false);
        // Configure the CirclePatternManager for this level
        CircleGameManager.SpawnCircles(currentLevelData.CircleCount, currentLevelData.Distance, currentLevelData.CircleDisplayTime);
        // Wait and switch to grid screen after CircleDisplayTime
        StartCoroutine(WaitAndSwitchScreen(currentLevelData.CircleDisplayTime));
    }


    public Levels GenerateCustomLevel(int levelNumber)
    {
        // Determine CircleCount
        int circleCount = Mathf.Clamp(16, 20, 25); // Randomly choose between 16, 20, 25
        if (circleCount == 16) circleCount = 16;
        else if (circleCount == 20) circleCount = 20;
        else circleCount = 25;

        // Set Distance based on CircleCount
        float distance = circleCount == 16 ? 150f :
                         circleCount == 20 ? 130f : 120f;

        // Set CountToDisplayCirclesScreen and CounttoDisplayGridScreen based on CircleCount
        int countToDisplay = circleCount == 16 ? 3 :
                             circleCount == 20 ? 4 : 5;

        // Set random LevelType
        LevelType levelType = (LevelType)Random.Range(0, System.Enum.GetValues(typeof(LevelType)).Length);

        // Return a generated level
        return new Levels
        {
            LevelNo = levelNumber,
            CircleCount = circleCount,
            Distance = distance,
            CircleDisplayTime = 3f,
            SymmetricDisplayTime = 7f,
            CountToDisplayCirclesScreen = countToDisplay,
            CounttoDisplayGridScreen = countToDisplay,
            ResultScreenTime = 5,
            GridSize = 0,
            levelType = levelType
        };
    }


    public IEnumerator TimerScreen(float delay)
    {
        timer_Slider.maxValue = delay;
        countdownTime = delay;
        timer_Slider.value = countdownTime;  // Set initial slider value
        int seconds = Mathf.CeilToInt(countdownTime);
        timer_Text.text = $"00:{seconds:00}";

        //      timer_Text.text = "00:" + Mathf.CeilToInt(countdownTime).ToString("00");  // Ensure two-digit formatting


        while (countdownTime > 0 && TimerCheck)
        {

            // Decrease the countdown time (this controls the speed of the countdown)
            countdownTime -= Time.deltaTime;

            // Update the slider with the remaining time
            timer_Slider.value = countdownTime;

            // Round the remaining time and update the text (only updating at whole seconds)
            //    timer_Text.text = "00:0" + Mathf.CeilToInt(countdownTime).ToString("00");  // Update the text to show the rounded time
            seconds = Mathf.CeilToInt(countdownTime);
            timer_Text.text = $"00:{seconds:00}";

            yield return null;


        }


        yield return new WaitUntil(() => TimerCheck);


        // Once the countdown ends, update the UI text to reflect zero
        timer_Text.text = "00:00";  // Optional: Set the text to "00:00" when the timer reaches zero

        if (isCalledFromGridScreen)
        {
            // Call the next function after grid screen timer ends
            Debug.Log("This function is called when the timer is : " + countdownTime.ToString());
            countdownTime = 0;
            substractionScreen.ClearLines();
            ShowCircleScreenAgain();
        }

        if (isResultScreen)
        {
            //   CircleGameManager.DisplayResult(img_False);
            //   Debug.Log("No Ans is submitted in Time : ");
            //   NoSubmitScreen.SetActive(true);
            //   countdownTime = 0;

            OnNoresultSubmitted();
        }


    }

    public void OnNoresultSubmitted() { 
    
        CircleGameManager.SubmitButtonClicked();

    }
    private void ActivateGridScreen(float gridTime)
    {
        gridResultImg.enabled = false;
        rotationResultImg.enabled = false;
        AdditionResultImg.enabled = false;
        SubstractionResultImg.enabled = false;
        isCalledFromGridScreen = true;
        StartCoroutine(TimerScreen(gridTime));

        isCircleScreenActive = true;
        gridScreen.SetActive(true);
        // circleScreen.SetActive(false);
        // Configure the PatternGameManager for this level
        switch (currentLevelData.levelType)
        {
            case LevelType.Symmetric:
                circle_Grid_Rotation.SetActive(true);
                Addition_Substraction.SetActive(false);
                patternManager.CreatePattern();
                Debug.Log("Symmetric Questions Should be working");

                break;

            case LevelType.Rotations:
                circle_Grid_Rotation.SetActive(true);
                Addition_Substraction.SetActive(false);
                RotationScreen.SetActive(true);
                rotationPatternManager.CreatePattern();
                Debug.Log("Rotation Questions Should be working");
                break;

            case LevelType.Additions:
                circle_Grid_Rotation.SetActive(false);
                Addition_Substraction.SetActive(true);
                AdditionScreen.SetActive(true);
                additionScreen.StartChallenge();
                Debug.Log("Additions Questions Should be working");
                break;
            case LevelType.Substraction:
                circle_Grid_Rotation.SetActive(false);
                Addition_Substraction.SetActive(true);
                SubstractionScreen.SetActive(true);
                substractionScreen.StartChallenge();
                Debug.Log("Substraction Questions Should be working");
                break;

            default:
                Debug.LogError("Unknown Level Type!");
                break;
        }
    }


    // Switches between screens after a delay
    public IEnumerator WaitAndSwitchScreen(float delay)
    {
        yield return StartCoroutine(TimerScreen(delay));

        yield return new WaitForSeconds(0);
        // Increment the number of CircleScreens shown
        Debug.Log("Circle Screen " + timesCircleScreenShown + " = " + currentLevelData.CountToDisplayCirclesScreen);
        if (timesCircleScreenShown <= currentLevelData.CountToDisplayCirclesScreen)
        {
            timesGridScreenShown++;

            ActivateGridScreen(currentLevelData.SymmetricDisplayTime);
            // Start user input sequence after CircleScreen has been displayed enough times
        }
        else
        {
            gridScreen.SetActive(false);
            isResultScreen = true;
            StartCoroutine(TimerScreen(currentLevelData.ResultScreenTime));
            CircleGameManager.StartUserInputSequence();
        }
    }
    public void ShowCircleScreenAgain()
    {
        isCalledFromGridScreen = false;
        timesCircleScreenShown++;
        isCircleScreenActive = false;

        circle_Grid_Rotation.SetActive(true);
        Addition_Substraction.SetActive(false);


        gridScreen.SetActive(false);
        RotationScreen.SetActive(false);
        AdditionScreen.SetActive(false);
        SubstractionScreen.SetActive(false);
        if (timesCircleScreenShown > currentLevelData.CountToDisplayCirclesScreen)
        {
            StartCoroutine(WaitAndSwitchScreen(0f));
        }
        else
        {
            CircleGameManager.AgainHighlight(currentLevelData.CircleDisplayTime);
            StartCoroutine(WaitAndSwitchScreen(currentLevelData.CircleDisplayTime));
        }
    }


    private int GetCircleCountForLevel(int CurrentLevelNo)
    {
        return levels[CurrentLevelNo].CircleCount; // Example: Gradually increases up to 10 circles
    }

    private float GetMinimumDistanceForLevel(int CurrentLevelNo)
    {
        return levels[CurrentLevelNo].Distance; // Example: Reduces distance as levels progress
    }

    // Handles level completion
    public void OnLevelComplete(int Scores)
    {
        StopAllCoroutines();
        int totalScores = 0;
        int Percentage = 0;
        timesCircleScreenShown = 0;
        timesGridScreenShown = 0;


        switch (currentLevelData.levelType)
        {
            case LevelType.Symmetric:
                totalScores = Scores + patternManager.patternScores;
                CorrectAnswers += Scores;
                Percentage = (int)((float)totalScores / (currentLevelData.CountToDisplayCirclesScreen + currentLevelData.CounttoDisplayGridScreen) * 100);

                break;

            case LevelType.Rotations:
                totalScores = Scores + rotationPatternManager.patternScores;
                CorrectAnswers += Scores;
                Percentage = (int)((float)totalScores / (currentLevelData.CountToDisplayCirclesScreen + currentLevelData.CounttoDisplayGridScreen) * 100);
                Debug.Log("Rotation Questions Should be working");
                break;

            case LevelType.Additions:
                totalScores = Scores + additionScreen.patternScores;
                CorrectAnswers += Scores;
                Percentage = (int)((float)totalScores / (currentLevelData.CountToDisplayCirclesScreen + currentLevelData.CounttoDisplayGridScreen) * 100);
                Debug.Log("Additions Questions Should be working");
                break;
            case LevelType.Substraction:
                totalScores = Scores + substractionScreen.patternScores;
                CorrectAnswers += Scores;
                Percentage = (int)((float)totalScores / (currentLevelData.CountToDisplayCirclesScreen + currentLevelData.CounttoDisplayGridScreen) * 100);
                Debug.Log("Substraction Questions Should be working");
                break;

            default:
                Debug.LogError("Unknown Level Type!");
                break;
        }
        ScoresPercentage.text = Percentage.ToString() + "%";
        circle_Grid_Rotation.SetActive(false);
        Addition_Substraction.SetActive(false);
     //   resultsPanel.SetActive(true);

       
        Debug.Log($"Level {currentLevel} Completed!");
        currentLevel++;
        PlayerPrefs.SetInt("currentLevel", currentLevel);


        // Resetting the values to again start from 0
        patternManager.patternScores = 0;
        rotationPatternManager.patternScores = 0;
        additionScreen.patternScores = 0;
        substractionScreen.patternScores = 0;
        CircleGameManager.CirclesScores = 0;
        // Toggle between CircleScreen and GridScreen for the next level
        isCircleScreenActive = !isCircleScreenActive;
        if (currentLevel < totalLevels)
        {   //Disable Results UI
            ShowResults(false);
        }
        else
        {
            ShowResults(true);
        }

    }

    public void ContinueButton()
    {
        timesCircleScreenShown = 0;
        timesGridScreenShown = 0;
        StartLevel(currentLevel);
    }

    //Countdown timer in between levels like 3,2,1
    IEnumerator StartCountdown()
    {
       
        int count = 3;
        while (count > 0)
        {
            ContinuePanelText.text = "Moving to Next Level in " + count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        resultsPanel.SetActive(false);
        ContinueButton(); // Your game start function
    }

    // Handles level failure
    public void OnLevelFailed(int Scores)
    {
        StopAllCoroutines();
        int totalScores = 0;
        int Percentage = 0;
        timesCircleScreenShown = 0;
        timesGridScreenShown = 0;


        switch (currentLevelData.levelType)
        {
            case LevelType.Symmetric:
                totalScores = Scores + patternManager.patternScores;
                CorrectAnswers += Scores;
                Percentage = (int)((float)totalScores / (currentLevelData.CountToDisplayCirclesScreen + currentLevelData.CounttoDisplayGridScreen) * 100);

                break;

            case LevelType.Rotations:
                totalScores = Scores + rotationPatternManager.patternScores;
                CorrectAnswers += Scores;
                Percentage = (int)((float)totalScores / (currentLevelData.CountToDisplayCirclesScreen + currentLevelData.CounttoDisplayGridScreen) * 100);
                Debug.Log("Rotation Questions Should be working");
                break;

            case LevelType.Additions:
                totalScores = Scores + additionScreen.patternScores;
                CorrectAnswers += Scores;
                Percentage = (int)((float)totalScores / (currentLevelData.CountToDisplayCirclesScreen + currentLevelData.CounttoDisplayGridScreen) * 100);
                Debug.Log("Additions Questions Should be working");
                break;
            case LevelType.Substraction:
                totalScores = Scores + substractionScreen.patternScores;
                CorrectAnswers += Scores;
                Percentage = (int)((float)totalScores / (currentLevelData.CountToDisplayCirclesScreen + currentLevelData.CounttoDisplayGridScreen) * 100);
                Debug.Log("Substraction Questions Should be working");
                break;

            default:
                Debug.LogError("Unknown Level Type!");
                break;
        }
        ScoresPercentage.text = Percentage.ToString() + "%";
        Debug.Log("Percentage " + Percentage.ToString() + "%");
       
      //  resultsPanel.SetActive(true);
        //   patternManager.patternScores = 0;
        //   CircleGameManager.CirclesScores = 0;
        currentLevel++;
        PlayerPrefs.SetInt("currentLevel", currentLevel);

        // Resetting the values to again start from 0
        patternManager.patternScores = 0;
        rotationPatternManager.patternScores = 0;
        additionScreen.patternScores = 0;
        CircleGameManager.CirclesScores = 0;
        substractionScreen.patternScores = 0;
        // Restart the same level

        if (currentLevel == totalLevels-1)
        {   //Disable Results UI
            ShowResults(false);
        }
        else
        {
            ShowResults(true);
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowResults(bool showresult)
    {
        resultsPanel.SetActive(true);

        if (showresult) {
            PlayerPrefs.SetInt("currentLevel", 0);
            Topbar.SetActive(true);
            ContinuPanel.SetActive(false);
         //   Attempted.SetActive(true);
         //   Correct.SetActive(true);
            Percentageobj.SetActive(true);
            continuebutton.SetActive(true);
            mainmenubutton.SetActive(true);
            // Perform floating-point division
            float percentage = ((float)CorrectAnswers / 60) * 100f;

            Debug.Log("Correct Answers: " + CorrectAnswers + " Total Questions Attempted: " + 96 + " Percentage: " + percentage);

          //  Percentageobj.transform.GetChild(1).GetComponent<Text>().text = percentage.ToString();
            ResultsManager.Instance.UpdateStats("Grid challenge", 5, 96, CorrectAnswers);

            CorrectAnswers += CorrectAnswers;
            attemptedQuestions += attemptedQuestions;

            // Format the result as a percentage
            string result = percentage.ToString("F2") + "%"; // "F2" limits to 2 decimal places
            ResultsText.text = result;
            //   Attempted.transform.GetChild(1).GetComponent<Text>().text = attemptedQuestions.ToString();
            //   Correct.transform.GetChild(1).GetComponent<Text>().text = CorrectAnswers.ToString();
            CorrectAnswers = 0;
        }
        else
        {
            ContinuPanel.SetActive(true);
            Correct.SetActive(true);
            Percentageobj.SetActive(true);
            Topbar.SetActive(false);
            continuebutton.SetActive(false);
            mainmenubutton.SetActive(false);
            StartCoroutine(StartCountdown());
        }
    }


}

public enum LevelType
{
    Symmetric,
    Rotations,
    Additions,
    Substraction
}

[System.Serializable]
public class Levels
{
    public int LevelNo;
    public int CircleCount;
    public float Distance;
    public float CircleDisplayTime;
    public float SymmetricDisplayTime;
    public int CountToDisplayCirclesScreen;
    public int CounttoDisplayGridScreen;
    public int ResultScreenTime;
    [Space]
    public int GridSize;

    [Header("Level Type")]
    public LevelType levelType; // Only one level type can be selected
}