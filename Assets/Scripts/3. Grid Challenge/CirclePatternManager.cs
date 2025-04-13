using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CirclePatternManager : MonoBehaviour
{
    public GameObject circlePrefab;  // The circle prefab to spawn
    public GameObject circleScreen;  // The parent object (CircleScreen) in the Canvas

    public Button SubmitButton;
    public Image resultImg;
    bool isCorrectSequence;
    [HideInInspector]
    public int counter = 0;

    private List<GameObject> spawnedCircles = new List<GameObject>();  // To track the spawned circle GameObjects
    private List<GameObject> highlightedCircles = new List<GameObject>(); // To store the sequence of highlighted circles
    private List<GameObject> clickedCircles = new List<GameObject>(); // To track clicked circles
    private bool isLevelCompleted = false; // Flag to track if the level is completed or failed

    public static CirclePatternManager instance;

    public int CirclesScores = 0;

    LevelManager levelManager;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void Start()
    {
        levelManager = LevelManager.instance;
        instance = this;
    }

    public void SpawnCircles(int gridsize, float minDistance, float time)
    {
        DeleteAllCircles();
        RectTransform circleScreenRect = circleScreen.GetComponent<RectTransform>();
        float screenWidth = circleScreenRect.rect.width;
        float screenHeight = circleScreenRect.rect.height;

        //  float screenWidth = Screen.width;
        //   float screenHeight = Screen.height;


        Debug.Log(screenWidth + " width " + screenHeight + " height");
        float circleRadius = circlePrefab.GetComponent<RectTransform>().rect.width / 2;
        Debug.Log(" Circle Radius " + circleRadius);
        float availableWidth = screenWidth - (2 * circleRadius);
        float availableHeight = screenHeight - (2 * circleRadius);

        Debug.Log(availableWidth + " Available width " + availableHeight + " Available height");


        List<Vector3> attemptedPositions = new List<Vector3>();

        for (int i = 0; i < gridsize; i++)
        {
            bool positionValid = false;
            Vector3 spawnPosition = Vector3.zero;

            while (!positionValid)
            {
                float randomX = Random.Range(-availableWidth / 2, availableWidth / 2);
                float randomY = Random.Range(-availableHeight / 2, availableHeight / 2);
                spawnPosition = new Vector3(randomX, randomY, 0) + circleScreenRect.position;

                positionValid = true;
                foreach (Vector3 pos in attemptedPositions)
                {
                    if (Vector3.Distance(spawnPosition, pos) < minDistance)
                    {
                        positionValid = false;
                        break;
                    }
                }
            }

            GameObject circle = Instantiate(circlePrefab, spawnPosition, Quaternion.identity, circleScreen.transform);
            circle.GetComponent<RectTransform>().localScale = Vector3.one;
            attemptedPositions.Add(spawnPosition);
            spawnedCircles.Add(circle);
            circle.GetComponent<Button>().onClick.AddListener(() => OnCircleClicked(circle));
            circle.GetComponent<Button>().enabled = false;
        }
        StartCoroutine(HighlightCirclesSequence(time));
    }

    void DeleteAllCircles()
    {
        foreach (GameObject circle in spawnedCircles)
        {
            Destroy(circle);
        }
        spawnedCircles.Clear();
    }
    public IEnumerator HighlightCirclesSequence(float time)
    {
        GameObject circle = null;

        // Keep finding a random circle until it's not already highlighted
        do
        {
            int randomIndex = Random.Range(0, spawnedCircles.Count);
            circle = spawnedCircles[randomIndex];
        } while (highlightedCircles.Contains(circle)); // Repeat until a unique circle is found

        // Highlight the circle by changing its color
        circle.GetComponent<Image>().color = new Color(86f / 255f, 86f / 255f, 86f / 255f);
        highlightedCircles.Add(circle);  // Add to the highlighted list

        yield return new WaitForSeconds(time);  // Wait for the specified time

        // Reset the circle's color to white after time
        circle.GetComponent<Image>().color = Color.white;

        // Optionally reset all circles to white if needed
        TurnAllCirclesWhite();
    }

    public void AgainHighlight(float time)
    {
        StartCoroutine(HighlightCirclesSequence(time));
    }

    void TurnAllCirclesWhite()
    {
        foreach (var circle in spawnedCircles)
        {
            circle.GetComponent<Image>().color = Color.white;
        }
    }

    void OnCircleClicked(GameObject clickedCircle)
    {

        if (!isLevelCompleted)
        {
            clickedCircles.Add(clickedCircle);
            clickedCircle.GetComponent<Image>().color = new Color(86f / 255f, 86f / 255f, 86f / 255f);
            clickedCircle.GetComponentInChildren<TextMeshProUGUI>().text = counter.ToString();

            if (clickedCircles.Count == highlightedCircles.Count)
            {
                isCorrectSequence = true;

                for (int i = 0; i < clickedCircles.Count; i++)
                {
                    if (clickedCircles[i] != highlightedCircles[i])
                    {
                        isCorrectSequence = false;

                    }
                    else
                    {
                        CirclesScores++;
                    }
                }

                SubmitButton.gameObject.SetActive(true);
                TrunOffCircleClicks();
            }
        }
    }

    //When All circles are clicked they should not be able to click any more circles. 
    void TrunOffCircleClicks()
    {
        foreach (var circle in spawnedCircles)
        {
            circle.GetComponent<Button>().enabled = false;
        }
    }

    public void SubmitButtonClicked()
    {
        if (isCorrectSequence)
        {
            Debug.Log("Level completed successfully!");
            StartCoroutine(DisplayResult(levelManager.img_Tick));
            isLevelCompleted = true;
            counter = 0;
            int currentScore = CirclesScores;
            levelManager.OnLevelComplete(currentScore);
        }
        else
        {
            Debug.Log("Level failed.");
            StartCoroutine(DisplayResult(levelManager.img_False));
            isLevelCompleted = true;
            counter = 0;
            int currentScore = CirclesScores;
            levelManager.OnLevelFailed(currentScore);

        }
        SubmitButton.gameObject.SetActive(false);
        levelManager.circle_Grid_Rotation.SetActive(false);
        levelManager.Addition_Substraction.SetActive(false);
        clickedCircles.Clear();
        highlightedCircles.Clear();
        CirclesScores = 0;
    }

    public IEnumerator DisplayResult(Sprite AnsSprite)
    {
        resultImg.enabled = true;
        resultImg.sprite = AnsSprite;
        yield return new WaitForSeconds(1f);

        StopAllCoroutines();
        resultImg.sprite = null;

    }

    public void StartUserInputSequence()
    {
        SubmitButton.gameObject.SetActive(false);

        foreach (GameObject btn in spawnedCircles)
        {
            btn.GetComponent<Button>().enabled = true;
        }
        Debug.Log("Starting user input sequence...");
        clickedCircles.Clear();
        isLevelCompleted = false;

        foreach (var circle in spawnedCircles)
        {
            circle.GetComponent<Image>().color = Color.white; // Reset all circle colors
            Button button = circle.GetComponent<Button>();
            button.interactable = true; // Ensure all circles are clickable
        }
    }
}