using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons;
    public TextMeshProUGUI resultText;
    public BallManager ballManager;

    [Header("CountDownTimer")]
    [SerializeField] int _CountDownTimer = 5; // Count down timer before game starts..
    [SerializeField] TextMeshProUGUI _CountDownTimerText;
    [SerializeField] GameObject _StartPanel; //CountDown Timer Panel..

    [Header("GamePlayTimer")]
    public float timeRemaining = 300f;
    private bool isTimerRunning = true;
    public TMP_Text text_timer;

    [Header("Results")]
    public int CorrectAns;
    public int QuestiosnAttempted;
    public GameObject ResultsPanel;
    public TextMeshProUGUI ResultsText;
    public TMP_Text attempted, correct;


    private static ButtonManager _instance;

    public static ButtonManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ButtonManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        CorrectAns = 0; QuestiosnAttempted = 0; // Intialize the Correct Answers and Questions Attempted to Zero

        _StartPanel.SetActive(true);
        ResultsPanel.SetActive(false);
        StartCoroutine(CountDownTimer());
        _CountDownTimer = 5;
        _CountDownTimerText.text = _CountDownTimer.ToString();
        ballManager.gameObject.SetActive(false);
        isTimerRunning = false;


    }


    private void Update()
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
                ShowResults();  //Show Results when timer ends..
                Debug.Log("Timer has ended.");
            }
        }
    }
    //This is ingame timer function for 5 Min.
    void UpdateTimerDisplay()
    {
        // Convert seconds into minutes and seconds format
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Update the Text component
        text_timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void ShowResults()
    {
        isTimerRunning = false;
        // Perform floating-point division
        float percentage = ((float)CorrectAns / QuestiosnAttempted) * 100f;

        Debug.Log("Correct Answers: " + CorrectAns + " Total Questions Attempted: " + QuestiosnAttempted + " Percentage: " + percentage);

        // Format the result as a percentage
        string result = percentage.ToString("F2") + "%"; // "F2" limits to 2 decimal places
        ResultsText.text = result;
        attempted.text = QuestiosnAttempted.ToString();
        correct.text = CorrectAns.ToString(); 
        ResultsPanel.SetActive(true);
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
        ballManager.getCurrentDifficultyLevel();
        _StartPanel.SetActive(false);
        ballManager.SpawnBalls();
        isTimerRunning = true;
        SetupLevel();
    }

    void SetupLevel()
    {
        // Get the correct number of balls from the BallManager
        int correctAnswer = ballManager.GetBallCount();
        SetUniqueButtonNumbers(correctAnswer);
    }

    void SetUniqueButtonNumbers(int correctAnswer)
    {
        List<int> uniqueNumbers = new List<int> { correctAnswer };

        while (uniqueNumbers.Count < buttons.Length)
        {
            int randomNum = Random.Range(8, 14);
            if (!uniqueNumbers.Contains(randomNum))
            {
                uniqueNumbers.Add(randomNum);
            }
        }

        uniqueNumbers.Sort();
        /*
                for (int i = 0; i < uniqueNumbers.Count; i++)
                {
                    int temp = uniqueNumbers[i];
                    int randomIndex = Random.Range(i, uniqueNumbers.Count);
                    uniqueNumbers[i] = uniqueNumbers[randomIndex];
                    uniqueNumbers[randomIndex] = temp;
                }
        */
        Color buttonColor;
        // Use ColorUtility to parse the hex color code
        ColorUtility.TryParseHtmlString("#2F8DC3", out buttonColor);

        for (int i = 0; i < buttons.Length; i++)
        {
            int number = uniqueNumbers[i];
            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = number.ToString();
            int buttonNumber = number;
            buttons[i].GetComponent<Image>().color = buttonColor; // 2F8DC3 button colors
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonNumber, correctAnswer));
        }
    }

    void OnButtonClick(int selectedNumber, int correctAnswer)
    {
        QuestiosnAttempted++;
        Button clickedButton = Array.Find(buttons, button => button.GetComponentInChildren<TextMeshProUGUI>().text == selectedNumber.ToString());
        Image buttonImage = clickedButton.GetComponent<Image>();  // Get the Image component of the button
        if (selectedNumber == correctAnswer)
        {
            IncreaseScore();
            resultText.color = Color.green;
            resultText.text = "Correct response";
            buttonImage.color = Color.green;  // Change the button color to green
            StartCoroutine(RestartGameAfterDelay());
        }
        else
        {
            resultText.color = Color.red;
            resultText.text = "Wrong response. Correct Ans is " + correctAnswer;
            buttonImage.color = Color.red;  // Change the button color to red
            StartCoroutine(RestartGameAfterDelay());
        }
    }
    private void IncreaseScore()
    {
        CorrectAns++;
    }

    IEnumerator RestartGameAfterDelay()
    {

        StopAllBalls(); // Stop ball movement
        yield return new WaitForSeconds(2);   // Wait for 2 seconds
        ballManager.increaseDifficulty();     // Increase the difficulty level
        resultText.text = "";                 // Clear result text
        ClearBalls();   // Clear existing balls
        ballManager.SpawnBalls();             // Spawn new balls
        SetupLevel();                         // Reset buttons with new values

   //     BallMovement.instance.StartAllBalls(); // Restart ball movement
    }

    public void StopAllBalls()
    {
        BallMovement[] balls = FindObjectsOfType<BallMovement>();

        // Disable the BallMovement script on all balls to stop their movement
        foreach (var ball in balls)
        {
            ball.enabled = false; // This stops the FixedUpdate and movement logic
        }
    }

    public void StartAllBalls()
    {
        BallMovement[] balls = FindObjectsOfType<BallMovement>();

        // Disable the BallMovement script on all balls to stop their movement
        foreach (var ball in balls)
        {
            ball.enabled = true; // This stops the FixedUpdate and movement logic
        }
    }

    public void ClearBalls()
    {
        BallMovement[] balls = FindObjectsOfType<BallMovement>();
        foreach (BallMovement ball in balls)
        {
            Destroy(ball.gameObject);
        }
    }


    public void StartChallenege()
    {
        ballManager.gameObject.SetActive(true);
    }

    public void Restart(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

}
