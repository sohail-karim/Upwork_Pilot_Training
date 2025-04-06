using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConcentrationManager : MonoBehaviour
{
    [Header("CountDownTimer")]
    [SerializeField] int _CountDownTimer; // Count down timer before game starts..
    [SerializeField] TextMeshProUGUI _CountDownTimerText;
    [SerializeField] GameObject CountDownTimerPanel; //CountDown Timer Panel..


    [Header("GamePlay Settings")]
    public Button AnsButton;
    private bool isGameRunning = false;
    public GameObject[] segments; // Assign 7 sprites in Inspector
    public Sprite[] AllSprites;
    public Sprite ESprite;
    public Image SegmentImage;
    private float timer = 0f;
    private float interval = 1f;
    private bool questionAnswered = false; // Track if the user has answered
    private float questionTimer = 0f; // Timer for question switching
    public float questionInterval = 1f; // Time between questions

    [Header("Dots")]
    public GameObject[] DotsCirclesPoints; // Assign 6 Circles in Inspector
    public bool isEqual = false;
    public bool DotsEqual3 = false;

    [Header("Results Settings")]
    [SerializeField] private int attemptedQuestions;
    [SerializeField] private int CorrectAnswers;
    public GameObject ResultsPanel;
    public TextMeshProUGUI ResultsText;
    public TMP_Text attempted, correct;

    [Header("GamePlayTimer")]
    public float timeRemaining = 300f;
    private bool isTimerRunning = true;
    public TMP_Text text_timer;

    void Start()
    {
        ResultsPanel.SetActive(false);
        StartCoroutine(CountDownTimer());
        _CountDownTimer = 5;
        _CountDownTimerText.text = _CountDownTimer.ToString();
        isTimerRunning = false;
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay();

                // If user hasn't answered, check if 1 second has passed
                if (!questionAnswered)
                {
                    questionTimer += Time.deltaTime;
                    if (questionTimer >= questionInterval)
                    {
                        LoadNewQuestion();
                    }
                }
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

    private IEnumerator ResetAndLoadNewQuestion(float delay)
    {
        yield return new WaitForSeconds(delay);

        ChangButtonColor("#113F72"); // Reset button color
        LoadNewQuestion();
    }
    private void LoadNewQuestion()
    {
        isEqual = false;
        DotsEqual3 = false;
        questionAnswered = false; // Reset answer state
        questionTimer = 0f; // Reset question timer
        RandomGenerateLines();
    }

    IEnumerator CountDownTimer()
    {
        while (_CountDownTimer > 0)
        {
            _CountDownTimerText.text = _CountDownTimer.ToString(); // Update UI first
            yield return new WaitForSeconds(1); // Then wait
            _CountDownTimer--;
        }
        CountDownTimerPanel.SetActive(false);
        isTimerRunning = true;
    }

    void RandomGenerateLines()
    {
      
        if (Random.value < 0.7f)
        {
            //generate code for equal and make circle counts 3
            isEqual = true;
            SegmentImage.sprite = ESprite;
            ActivateDots();
        }
        else
        {
            isEqual = false;
            SegmentImage.sprite = AllSprites[Random.Range(0, AllSprites.Length)];
            ActivateDots();

        }
    }

    public void ActivateDots()
    {
        // Deactivate all dots first
        for (int i = 0; i < 6; i++)
        {
            DotsCirclesPoints[i].SetActive(false);
        }

        int dotCount;

        // Ensure 70% chance of selecting exactly 3 dots
        if (Random.value < 0.5f)
        {
            dotCount = 3;
        }
        else
        {
            // 30% chance: pick randomly between 4, 5, or 6 dots
            dotCount = Random.Range(4, 7);
        }

        DotsEqual3 = (dotCount == 3);

        // Select random indices to activate
        List<int> activeIndices = Enumerable.Range(0, 6).OrderBy(x => Random.value).Take(dotCount).ToList();

        // Activate the selected dots
        foreach (int index in activeIndices)
        {
            DotsCirclesPoints[index].SetActive(true);
        }
    }

    public void Equal()
    {
        attemptedQuestions++;
        questionAnswered = true; // Mark question as answered

        if (isEqual && DotsEqual3)
        {
            Debug.Log("Correct Ans");
            CorrectAnswers++;
            ChangButtonColor("#117217"); // Green for correct
        }
        else
        {
            ChangButtonColor("#B50F0D"); // Red for wrong
            Debug.Log("Wrong Ans");
        }

        // Wait 0.5s before resetting and moving to the next question
        StartCoroutine(ResetAndLoadNewQuestion(0.5f));
    }

    private void ChangButtonColor(string HexColor)
    {
        if(ColorUtility.TryParseHtmlString(HexColor, out Color color))
        {
            Debug.Log("Color Changed");
            AnsButton.image.color = color;
        }
    }

    public void NotEqual()
    {
        attemptedQuestions++;
        if (!isEqual || !DotsEqual3)
        {
            Debug.Log("Correct Ans");
            CorrectAnswers++;
        }
        else
        {
            Debug.Log("Wrong Ans");
        }
    }


    #region Results
    public void ShowResults()
    {
        isTimerRunning = false;
        // Perform floating-point division
        float percentage = ((float)CorrectAnswers / attemptedQuestions) * 100f;

        Debug.Log("Correct Answers: " + CorrectAnswers + " Total Questions Attempted: " + attemptedQuestions + " Percentage: " + percentage);

        ResultsManager.Instance.UpdateStats("Concentration", 6, attemptedQuestions, CorrectAnswers);

        // Format the result as a percentage
        string result = percentage.ToString("F2") + "%"; // "F2" limits to 2 decimal places
        ResultsText.text = result;
        attempted.text = attemptedQuestions.ToString();
        correct.text = CorrectAnswers.ToString();
        ResultsPanel.SetActive(true);
    }

    void UpdateTimerDisplay()
    {
        // Convert seconds into minutes and seconds format
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        // Update the Text component
        text_timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    #endregion

    //used on exit and mainmenu button  
    public void ChangeLevel(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
