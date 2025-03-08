using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReactionManager : MonoBehaviour
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

    [Header("Sprite Settings")]
    public Sprite[] QuestionImages;
    public Image leftImage, RightImg;
    private float timer = 0f;
    private float interval = 1f;

    [Header("Results Settings")]
    [SerializeField] private int attemptedQuestions;
    [SerializeField] private int  CorrectAnswers;
    public GameObject ResultsPanel;
    public TextMeshProUGUI ResultsText;
    public TMP_Text attempted, correct;

    [Header("GamePlayTimer")]
    public float timeRemaining = 300f;
    private bool isTimerRunning = true;
    public TMP_Text text_timer;


    private int leftIndex, rightIndex;


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
                StartGame();
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
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            Debug.Log("Images Generated");
            RandomUpdateSprites();
       //     StartCoroutine(MoveSliders());
            timer = 0f;
        }
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
        CountDownTimerPanel.SetActive(false);

        isTimerRunning = true;
    }

    public void RandomUpdateSprites()
    {
        leftIndex = Random.Range(0, QuestionImages.Length);
        rightIndex = Random.Range(0, QuestionImages.Length);
        leftImage.sprite = QuestionImages[leftIndex];
        RightImg.sprite = QuestionImages[rightIndex];
    }
    public void EqualOrNot()
    {
        attemptedQuestions++;
        if (leftIndex == rightIndex)
        {
            Debug.Log("Equal");
            CorrectAnswers++;
        }
        else
        {
            Debug.Log("Not Equal");
        }
    }


    #region Slider Movement
 //  IEnumerator MoveSliders()
 //  {
 //      while (true)
 //      {
 //          yield return MoveSlidersOnce(1f, 0f); // Move Right & Left
 //          yield return MoveSlidersOnce(0f, 1f); // Move Left & Right
 //      }
 //  }
 //
 //  IEnumerator MoveSlidersOnce(float targetValueUp, float targetValueBottom)
 //  {
 //      float elapsedTime = 0f;
 //      float startValueUp = Upslider.value;
 //      float startValueBottom = BottomSlider.value;
 //
 //      while (elapsedTime < duration)
 //      {
 //          elapsedTime += Time.deltaTime;
 //          float t = elapsedTime / duration;
 //
 //          Upslider.value = Mathf.Lerp(startValueUp, targetValueUp, t);
 //          BottomSlider.value = Mathf.Lerp(startValueBottom, targetValueBottom, t);
 //
 //          yield return null;
 //      }
 //
 //      // Ensure values reach exactly the target
 //      Upslider.value = targetValueUp;
 //      BottomSlider.value = targetValueBottom;
 //  }

    #endregion

    #region Results
    public void ShowResults()
    {
        isTimerRunning = false;
        // Perform floating-point division
        float percentage = ((float)CorrectAnswers / attemptedQuestions) * 100f;

        Debug.Log("Correct Answers: " + CorrectAnswers + " Total Questions Attempted: " + attemptedQuestions + " Percentage: " + percentage);

        ResultsManager.Instance.UpdateStats("Reaction Challenge", 4, attemptedQuestions, CorrectAnswers);

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
