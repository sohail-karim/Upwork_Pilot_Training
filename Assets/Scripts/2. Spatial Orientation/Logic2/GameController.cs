using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Directions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{

    public GameObject GyroCampus;
    public GameObject Arrow;
    public List<GameObject> directionPrefabs; // List of 8 direction prefabs (North, NorthEast, etc.)
    public Transform GyroDirectionsSpawnPoint;
    public Slider TimerSlider;

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

    private float timeLeft = 10f;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        TimerSlider.maxValue = timeLeft;

        StartNewQuestion();
    }


    public void StartNewQuestion()
    {
        timeLeft = 10f;
        TimerSlider.maxValue = timeLeft;
        ClearSpawnedDirections(GyroDirectionsSpawnPoint);
          
        if (!hasGuessed)
        {
            Debug.Log("Need to submit a guess first");
            return;
        }
        if (timeLeft <= 0)
        {
            Debug.Log("Time's up!");
            return;
        }


        dirPlaneTravel = GetRandomDirection(true);
        dirBeaconToPlane = GetRandomDirection(false);
        SpawnDirectionPoints(dirPlaneTravel.ToString());


        RotateGyrocompass();
        RotateArrow();

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
    }





    public void SpawnDirectionPoints(string correctDirection)
    {

        // Find the correct prefab
        GameObject correctPrefab = directionPrefabs.Find(prefab => prefab.name == correctDirection);
        if (correctPrefab == null)
        {
            Debug.LogError($"Prefab for {correctDirection} not found.");
            return;
        }

        // Select 3 additional random prefabs excluding the correct one
        List<GameObject> availablePrefabs = new List<GameObject>(directionPrefabs);
        availablePrefabs.Remove(correctPrefab);

        List<GameObject> selectedPrefabs = new List<GameObject> { correctPrefab }; // Always include the correct prefab
        while (selectedPrefabs.Count < 4)
        {
            GameObject randomPrefab = availablePrefabs[Random.Range(0, availablePrefabs.Count)];
            if (!selectedPrefabs.Contains(randomPrefab))
            {
                selectedPrefabs.Add(randomPrefab);
            }
        }

        // Shuffle the selected prefabs to ensure the correct one is not always in the same position
        for (int i = selectedPrefabs.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = selectedPrefabs[i];
            selectedPrefabs[i] = selectedPrefabs[randomIndex];
            selectedPrefabs[randomIndex] = temp;

        }

        // Spawn the prefabs at the spawn point
        foreach (GameObject prefab in selectedPrefabs)
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

        selectedPrefabs.Clear();
        availablePrefabs.Clear();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!stopTimer)
        {
            if (timeLeft <= 0)
            {
                Debug.Log("Time's up!");
                hasGuessed = true;
                stopTimer = true;
                StartCoroutine(HighLightCorrectAns(dirPlaneTravel.ToString(), dirBeaconToPlane.ToString()));
                return;
            }
            timeLeft -= Time.deltaTime;
            TimerSlider.value = timeLeft;
        }
       


    }

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
    }

    public void GuessLocation(string dir) {
        Direction direction = GetDirectionFromString(dir);
        if (directionGuess == Direction.Null) { //need a direction guess first (for placing plane sprite)
            Debug.Log("Guess the direction first");
            return;
        }
        locationGuess = direction;
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
        if(timeLeft <= 0) {
            return;
        }

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
            StartCoroutine(HighLightCorrectAns(dirPlaneTravel.ToString(), dirBeaconToPlane.ToString()));
            isCorrect = false;
        }
        questionsAnswered++;
        

    }


    IEnumerator HighLightCorrectAns(string dirPlaneTravel, string dirbeaconToPlane)
    {
      //  answerText.text = "Incorrect!" + "\nCorrect Direction: " + dirPlaneTravel + "\nCorrect Location: " + dirbeaconToPlane;
        string finalName = dirPlaneTravel + "(Clone)";
        GameObject.Find(finalName).GetComponent<ButtonBorderToggle>().EnableImage();
        GameObject.Find(dirbeaconToPlane).GetComponentInChildren<Button>().colors = UpdateButtonNormalColor(Color.green);

        yield return new   WaitForSeconds(1);

        GameObject.Find(dirbeaconToPlane).GetComponentInChildren<Button>().colors = UpdateButtonNormalColor(Color.white);
        hasGuessed = true;
        StartNewQuestion();
        stopTimer = false;
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




}
