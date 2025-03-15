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

    //  [Header("Slider Settings")]
    //  public Slider Upslider;
    //  public Slider BottomSlider;
    //  public float duration = 1f;
    //  private bool movingRight = true;

    [Header("GamePlay Settings")]
    public GameObject[] segments; // Assign 7 sprites in Inspector
    public Sprite[] AllSprites;
    public Sprite ESprite;
    public Image SegmentImage;

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


    private int leftIndex, rightIndex;
    private int lastEqualIndex = -1; // Store last equal index



    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

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

        #region timer
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
        #endregion
    }

    public void StartGame()
    {
        isEqual = false;
        DotsEqual3 = false;
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
        StartGame();
        isTimerRunning = true;
    }

    void RandomGenerateLines()
    {
        if (Random.value < 0.3f)
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
    //Previous Code
    /*
    public void RandomGenerateLines()
    {
        // Ensure we have exactly 7 segments assigned
        if (segments.Length != 7)
        {
            Debug.LogError("Assign exactly 7 segment sprites in the inspector!");
            return;
        }
        if (Random.value< 0.3f)
        {
            //generate code for equal and make circle counts 3
            isEqual = true;
            GenerateIndicesforE();
            ActivateDots();
        }
        else
        {
            isEqual = false;
            ActivateDots();
            // Randomly activate 4-6 segments
            List<int> activeIndices = GetRandomActiveIndices();

            // Enable selected segments, disable the rest
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].SetActive(activeIndices.Contains(i));
            }

        }  
    }

    */

    #region regionforE

    void GenerateIndicesforE()
    {
        List<int> indices = new List<int> { 0, 1, 2, 5, 6 };

        //Setting False
        for (int i = 0; i < 7; i++)
        {
            segments[i].SetActive(false);
        }

        for (int i = 0; i < segments.Length; i++)
        {
            segments[i].SetActive(indices.Contains(i));
        }
    }
    #endregion

    private List<int> GetRandomActiveIndices()
    {
        List<int> indices = new List<int> { 0, 1, 2, 3, 4, 5, 6 };
        int activeCount = Random.Range(4, 7); // Activate 4 to 6 segments

        // Shuffle and take the first 'activeCount' indices
        indices.Shuffle();
        return indices.GetRange(0, activeCount);
    }

    public void ActivateDots()
    {
        //Deativate all dots first..
        for (int i = 0; i < 6; i++)
        {
            DotsCirclesPoints[i].SetActive(false);
        }
        int dotCount = Random.Range(3, 6);
        DotsEqual3 = dotCount == 3;
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
        if (isEqual && DotsEqual3)
        {
            Debug.Log("Correct Ans");
            CorrectAnswers++;
        }
        else
        {
            Debug.Log("Wrong Ans");
        }
        StartGame();
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
        StartGame();
    }


    #region Results
    public void ShowResults()
    {
        isTimerRunning = false;
        // Perform floating-point division
        float percentage = ((float)CorrectAnswers / attemptedQuestions) * 100f;

        Debug.Log("Correct Answers: " + CorrectAnswers + " Total Questions Attempted: " + attemptedQuestions + " Percentage: " + percentage);

        ResultsManager.Instance.UpdateStats("Concentration Challenge", 6, attemptedQuestions, CorrectAnswers);

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
