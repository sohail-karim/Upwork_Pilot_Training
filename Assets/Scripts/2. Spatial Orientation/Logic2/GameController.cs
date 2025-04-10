using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Directions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;
public class GameController : MonoBehaviour
{
 //   public RectTransform ScreenDivider;

    [Header("CountDownTimer")]
    [SerializeField] int _CountDownTimer = 5; // Count down timer before game starts..
    [SerializeField] TextMeshProUGUI _CountDownTimerText;
    [SerializeField] GameObject CountDownTimerPanel; //CountDown Timer Panel..


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


    [Space]
    public GameObject GyroCampus;
    public GameObject Arrow;
    public List<GameObject> directionPrefabs; // List of 8 direction prefabs (North, NorthEast, etc.)
    public List<GameObject> NumberGridPrefabs;
    public Transform GyroDirectionsSpawnPoint;
  //  public Slider TimerSlider;

    Direction dirPlaneTravel;
    Direction dirBeaconToPlane;
    Direction dirPlaneToBeacon;
    Direction dirRelativePlaneToBeacon; // dirPlaneToBeacon from the axis of the plane

    [SerializeField]  Direction directionGuess = Direction.Null; // the direction the plane is facing
    [SerializeField] Direction locationGuess = Direction.Null; // which square around the beacon the plane is in


    bool isCorrect = false;
    bool hasGuessed = true;
    bool stopTimer = false;

    [SerializeField] int questionsAnswered = 0;
    [SerializeField] int questionsCorrect = 0;


    [SerializeField] Button clickedButton;
    [SerializeField] GameObject buttonsparent;

    //implementation for everyquestion timer..
    //  private float timeLeft = 10f;

    void Awake()
    {
      //  Screen.orientation = ScreenOrientation.LandscapeLeft;

    //  int screenWidth = Screen.width;
    //  int screenHeight = Screen.height;
    //  Debug.Log("Screen Width: " + screenWidth + " Screen Height: " + screenHeight);
    //  ScreenDivider.sizeDelta = new Vector2(screenWidth, screenHeight);
    //  RectTransform rt = (RectTransform)ScreenDivider.GetChild(0);
    //  RectTransform rt2 = (RectTransform)ScreenDivider.GetChild(1);
    //  rt.sizeDelta = new Vector2(screenWidth, screenHeight/2);
    //  rt2.sizeDelta = new Vector2(screenWidth, screenHeight/2);

    }

    private void Start()
    {
        isTimerRunning = false;
        CountDownTimerPanel.SetActive(true);
        StartCoroutine(CountDownTimer());
        _CountDownTimer = 5;
        _CountDownTimerText.text = _CountDownTimer.ToString();
     //   TimerSlider.maxValue = timeLeft;
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
        StartNewQuestion();
        isTimerRunning = true;

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
        float percentage = ((float)questionsCorrect / questionsAnswered) * 100f;

        Debug.Log("Correct Answers: " + questionsCorrect + " Total Questions Attempted: " + questionsAnswered + " Percentage: " + percentage);

        ResultsManager.Instance.UpdateStats("Spatial Orientation", 1, questionsAnswered, questionsCorrect);

        // Format the result as a percentage
        string result = percentage.ToString("F2") + "%"; // "F2" limits to 2 decimal places
        ResultsText.text = result;
        attempted.text = questionsAnswered.ToString();
        correct.text = questionsCorrect.ToString();
        ResultsPanel.SetActive(true);
    }


    public void StartNewQuestion()
    {


        //   timeLeft = 10f;
        //   TimerSlider.maxValue = timeLeft;
        GyroDirectionsSpawnPoint.GetComponent<ButtonBorderToggle>().enabled = true;
        ClearSpawnedDirections(GyroDirectionsSpawnPoint);
        if (!hasGuessed)
        {
            Debug.Log("Need to submit a guess first");
            return;
        }

    //  if (timeLeft <= 0)
    //  {
    //      Debug.Log("Time's up!");
    //      return;
    //  }


        dirPlaneTravel = GetRandomDirection(true);
        dirBeaconToPlane = GetRandomDirection(false);
     //   buttonsparent.GetComponent<ButtonBorderToggle>().DestroyAllButtons(); // Ensure old buttons are removed first
        SpawnDirectionPoints(dirPlaneTravel.ToString());
        RotateGyrocompass();
        RotateArrow();
        
        //   buttonsparent.GetComponent<ButtonBorderToggle>().UpdateButtonsArray(); // Ensure old buttons are removed first

        directionGuess = Direction.Null;
        locationGuess = Direction.Null;


        isCorrect = false;
        hasGuessed = false;
    }

    public void ClearSpawnedDirections(Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is not assigned.");
            return;
        }

        // Loop through all child objects and destroy them
        foreach (Transform child in spawnPoint)
        {
            Destroy(child.gameObject);
        }
        //Destroy all buttons of the Aeroplanes spawned..
    }





    public void SpawnDirectionPoints(string correctDirection)
    {
        
   //   // Find the correct prefab
   //   GameObject correctPrefab = directionPrefabs.Find(prefab => prefab.name == correctDirection);
   //   if (correctPrefab == null)
   //   {
   //       Debug.LogError($"Prefab for {correctDirection} not found.");
   //       return;
   //   }
   //
   //   // Select 3 additional random prefabs excluding the correct one
   //   List<GameObject> availablePrefabs = new List<GameObject>(directionPrefabs);
   //   availablePrefabs.Remove(correctPrefab);
   //
   //   List<GameObject> selectedPrefabs = new List<GameObject> { correctPrefab }; // Always include the correct prefab
   //   while (selectedPrefabs.Count < 4)
   //   {
   //       GameObject randomPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
   //       if (!selectedPrefabs.Contains(randomPrefab))
   //       {
   //           selectedPrefabs.Add(randomPrefab);
   //       }
   //   }

        // Spawn the prefabs at the spawn point
        foreach (GameObject prefab in directionPrefabs)
        {
            GameObject instance = Instantiate(prefab, GyroDirectionsSpawnPoint.position, Quaternion.identity, GyroDirectionsSpawnPoint);
            Button button = instance.GetComponent<Button>();
            if (button != null)
            {
                string directionName = prefab.name; // Capture the prefab's name
                button.onClick.AddListener(() => GuessDirection(directionName));
            }
            else
            {
                Debug.LogWarning($"Prefab {prefab.name} does not have a Button component.");
            }
        }
     //   selectedPrefabs.Clear();
      //  availablePrefabs.Clear();
        buttonsparent.SetActive(true);
    }

    // Update is called once per frame
// void FixedUpdate()
// {
//
//     if (!stopTimer)
//     {
//         if (timeLeft <= 0)
//         {
//             Debug.Log("Time's up!");
//             hasGuessed = true;
//             stopTimer = true;
//             StartCoroutine(HighLightCorrectAns(dirPlaneTravel.ToString(), dirBeaconToPlane.ToString()));
//             return;
//         }
//         timeLeft -= Time.deltaTime;
//         TimerSlider.value = timeLeft;
//     }
//    
//
//
// }

    private void RotateGyrocompass() {
        int angle = dirPlaneTravel.GetAngle();
        GyroCampus.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void RotateArrow() {

        dirPlaneToBeacon = dirBeaconToPlane.GetReverse();

        dirRelativePlaneToBeacon = dirPlaneToBeacon.RotateDirection(-dirPlaneTravel.GetAngle());
        int angle = dirRelativePlaneToBeacon.GetAngle();
        Arrow.transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

 
    private Direction GetRandomDirection(bool isPlane) {
        if (isPlane)
        {
            return (Direction)(UnityEngine.Random.Range(0, 4) * 90);

        }
        else {

            return (Direction)(UnityEngine.Random.Range(0, 8) * 45);
        }
    }




    public void GuessDirection(string dir) {
        Direction direction = GetDirectionFromString(dir);
        locationGuess = Direction.Null; // location can only be set after a guess, so reset when a new guess is made
        directionGuess = direction;

        if (clickedButton != null)
        {
            ResetColor();
        }
    }

    public void GuessLocation(string dir) {
        Direction direction = GetDirectionFromString(dir);
        if (directionGuess == Direction.Null) { //need a direction guess first (for placing plane sprite)
            Debug.Log("Guess the direction first");
            return;
        }

        //used to get the button pressed so we can highlight it yellow.      
    //    btn.colors = UpdateButtonNormalColor(Color.yellow); 

        locationGuess = direction;

     Invoke("SubmitGuess", 0.5f);
    }

    public void ButtonClicked(Button btn)
    {
        clickedButton = btn;
        if (directionGuess != Direction.Null)
        {
         //   clickedButton.image.color = Color.yellow;
        } 
    }

    private Direction GetDirectionFromString(string dir) {
        switch (dir) {
            case "North":
                return Direction.North;
            case "NorthWest":
                return Direction.NorthWest;
            case "East":
                return Direction.East;
            case "NorthEast":
                return Direction.NorthEast;
            case "South":
                return Direction.South;
            case "SouthEast":
                return Direction.SouthEast;
            case "SouthWest":
                return Direction.SouthWest;
            case "West":
                return Direction.West;
            default:
                Debug.Log("Invalid direction");
                return Direction.Null;
        }
    }

    public void SubmitGuess() {
   //   if(timeLeft <= 0) {
   //       return;
   //   }
        if(directionGuess == Direction.Null || locationGuess == Direction.Null) {
            Debug.Log("Need to guess both direction and location");
            return;
        }
        if(directionGuess == dirPlaneTravel && locationGuess == dirBeaconToPlane) {
            questionsCorrect++;
            stopTimer = true;
          StartCoroutine(HighLightCorrectAns(dirPlaneTravel.ToString(), dirBeaconToPlane.ToString()));
            Debug.Log("Correct!");
            isCorrect = true;
        }
        else {
            Debug.Log("Incorrect");
            stopTimer = true;
            StartCoroutine(HighLightWrongAns(directionGuess.ToString(), locationGuess.ToString()));
            StartCoroutine(HighLightCorrectAns(dirPlaneTravel.ToString(), dirBeaconToPlane.ToString()));
            isCorrect = false;
        }
        questionsAnswered++;
    }


    IEnumerator HighLightCorrectAns(string dirPlaneTravel, string dirbeaconToPlane)
    {
      //  answerText.text = "Incorrect!" + "\nCorrect Direction: " + dirPlaneTravel + "\nCorrect Location: " + dirbeaconToPlane;
        string finalName = dirPlaneTravel + "(Clone)";
        GameObject.Find(finalName).transform.GetChild(0).GetComponent<Image>().enabled = true;
        GameObject.Find(finalName).transform.GetChild(0).GetComponent<Image>().color = Color.green;
        GameObject.Find(dirbeaconToPlane).GetComponentInChildren<Image>().color = Color.green;

        yield return new   WaitForSeconds(1);

        GameObject.Find(dirbeaconToPlane).GetComponentInChildren<Image>().color = Color.white;
        hasGuessed = true;
        if (clickedButton != null)
        {
            ResetColor();
        }
        buttonsparent.SetActive(false);
        StartNewQuestion();
        stopTimer = false;
    }

    IEnumerator HighLightWrongAns(string dirPlaneTravel, string dirbeaconToPlane)
    {
        //  answerText.text = "Incorrect!" + "\nCorrect Direction: " + dirPlaneTravel + "\nCorrect Location: " + dirbeaconToPlane;
        string finalName = dirPlaneTravel + "(Clone)";
        GameObject.Find(finalName).transform.GetChild(0).GetComponent<Image>().enabled = true;
        GameObject.Find(finalName).transform.GetChild(0).GetComponent<Image>().color = Color.red;
        GameObject.Find(dirbeaconToPlane).GetComponentInChildren<Image>().color = Color.red;
        yield return null;
    }

    void ResetColor()
    {
        foreach(GameObject obj in NumberGridPrefabs)
        {
          //  obj.GetComponent<Button>().colors = UpdateButtonNormalColor(Color.yellow);
            obj.GetComponent<Image>().color = Color.white;
        }
    }

    private ColorBlock UpdateButtonNormalColor(Color newNormalColor)
    {
        ColorBlock colorBlock = new ColorBlock();
        colorBlock = ColorBlock.defaultColorBlock; // Start with default colors
        colorBlock.normalColor = newNormalColor;   // Update the normal color
        return colorBlock;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }

    public void AgainPlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }




}
