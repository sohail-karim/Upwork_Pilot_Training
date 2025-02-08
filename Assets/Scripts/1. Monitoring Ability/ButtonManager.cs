using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons;
    public TextMeshProUGUI resultText;
    public BallManager ballManager;


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
        ballManager.gameObject.SetActive(false);
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

        for (int i = 0; i < uniqueNumbers.Count; i++)
        {
            int temp = uniqueNumbers[i];
            int randomIndex = Random.Range(i, uniqueNumbers.Count);
            uniqueNumbers[i] = uniqueNumbers[randomIndex];
            uniqueNumbers[randomIndex] = temp;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int number = uniqueNumbers[i];
            TextMeshProUGUI buttonText = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = number.ToString();
            int buttonNumber = number;
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonNumber, correctAnswer));
        }
    }

    void OnButtonClick(int selectedNumber, int correctAnswer)
    {
        if (selectedNumber == correctAnswer)
        {
            resultText.color = Color.green;
            resultText.text = "Correct response";
            StartCoroutine(RestartGameAfterDelay());
        }
        else
        {
            resultText.color = Color.red;
            resultText.text = "Wrong response. Correct Ans is " + correctAnswer;
            StartCoroutine(RestartGameAfterDelay());
        }
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

}
