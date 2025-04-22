using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Threading;

public class RotationPatternManager : MonoBehaviour
{
    public List<Transform> leftPoints;  // List of points on the left grid
    public List<Transform> rightPoints; // List of points on the right grid

    public GameObject rightGridParent;
    [Space]
    public Button yesButton; // Yes button for checking the symmetry
    public Button noButton; // No button for checking the difference
    [Space]
    public bool rotateRightGrid = true;
    public bool changeToUpperRight = false;

   [Space]
    private List<int> scaledLeftPoints = new List<int>();  // To track the scaled left points
    private List<int> scaledRightPoints = new List<int>();

    public static RotationPatternManager instance;
    LevelManager levelManager;

    Dictionary<int, List<int>> patterns = new Dictionary<int, List<int>>
{
    { 2, new List<int> {  0, 1, 2, 3, 4, 9,10,11,12,13,14,15,20,21,22,23,24} },
    { 3, new List<int> {  0, 1, 2, 3, 4, 9,10,11,12,13,14,19,20,21,22,23,24} },
    { 4, new List<int> {  0, 5, 10,11,12,13,14,4,9,19,24} },
    { 5, new List<int> { 0, 1, 2, 3, 4, 5,10,11,12,13,14,19, 20, 21, 22, 23, 24 } },
    { 6, new List<int> { 0, 1, 2, 3, 4, 5,10,11,12,13,14,19,15, 20, 21, 22, 23, 24 } },
    { 9, new List<int> { 0, 1, 2, 3, 4, 5,10,11,12,13,14,9,19, 20, 21, 22, 23, 24 } }
};

    private bool isIdentical = true;
    [Tooltip("This is used to Store Scores of this Rotational Pattern")]
    public int patternScores;


    float displayResultTimer = 0f;
    bool isDisplayingResult = false;


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        
    }
    private void Start()
    {
        levelManager = LevelManager.instance;

        yesButton.onClick.AddListener(OnYesButtonPressed);
        noButton.onClick.AddListener(OnNoButtonPressed);

     //   CreatePattern();
    }

    public void CreatePattern()
    {
        int randomKey = patterns.Keys.ElementAt(Random.Range(0, patterns.Count));
        rotateRightGrid = false;
        changeToUpperRight = false;
        // Clear previous scaling if any
        HighlightPattern(patterns[randomKey]);
    }

    public void HighlightPattern(List<int> indicesToHighlight)
    {
        ClearPattern(); // Reset any previous highlights

        // Highlight the left grid
        foreach (int index in indicesToHighlight)
        {
            if (index >= 0 && index < leftPoints.Count)
            {
                // Highlight or scale the grid point
                leftPoints[index].transform.localScale = new Vector3(4f, 4f, 1); // Increase scale
            }
        }
        rotateRightGrid = Random.Range(0, 2) == 0;

        if (rotateRightGrid)
        {
            // Rotate the right grid by 90 degrees along the Z-axis
            if (rightGridParent != null)
            {
                Debug.Log("Rotation");
                rightGridParent.transform.rotation = Quaternion.Euler(0, 0, -90);
            }
        }
        else
        {
            // Reset rotation if no rotation is applied
            if (rightGridParent != null)
            {
                Debug.Log("No Rotation");
                rightGridParent.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        if (!rotateRightGrid)
        {
            changeToUpperRight = true;
        }
        else
        {
            changeToUpperRight = false;  
        }

       // changeToUpperRight = Random.Range(0, 2) == 0;  // 50% chance to set the start corner to UpperRight
        if (rightGridParent != null)
        {
            GridLayoutGroup gridLayout = rightGridParent.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                if (changeToUpperRight && !rotateRightGrid)
                {
                    // Change the start corner to UpperRight
                    gridLayout.startCorner = GridLayoutGroup.Corner.UpperRight;
                    Debug.Log("Inverse : " + changeToUpperRight);
                }
                else
                {
                    // Optionally reset to UpperLeft if no change is applied
                    gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
                    Debug.Log("Inverse : " + changeToUpperRight);
                }
            }
        }

        // Highlight the right grid
        foreach (int index in indicesToHighlight)
        {
            if (index >= 0 && index < rightPoints.Count)
            {
                // Highlight or scale the grid point
                rightPoints[index].transform.localScale = new Vector3(4f, 4f, 1); // Increase scale
            }
        }
    }


    void ClearPattern()
    {
        // Reset the scale of all points on both grids to normal size
        foreach (Transform point in leftPoints)
        {
            point.localScale = Vector3.one;
        }

        foreach (Transform point in rightPoints)
        {
            point.localScale = Vector3.one;
        }

        // Clear the lists tracking scaled points
        scaledLeftPoints.Clear();
        scaledRightPoints.Clear();
    }

    public void OnYesButtonPressed()
    {
        
        if (rotateRightGrid && !changeToUpperRight)
        {
            StartCoroutine(Display_Result(levelManager.img_Tick));
            patternScores++;
            levelManager.setScores(1);
            Debug.Log("Correct Ans");
        }
        else
        {
          StartCoroutine(Display_Result(levelManager.img_False));
            Debug.Log("Wrong Ans");
        }
    }

    public void OnNoButtonPressed()
    {
        if (changeToUpperRight)
        {
            patternScores++;
            levelManager.setScores(1);
            StartCoroutine(Display_Result(levelManager.img_Tick));
            Debug.Log("Correct Ans");
        }
        else
        {
           StartCoroutine(Display_Result(levelManager.img_False));
            Debug.Log("Wrong Ans");
        }

    }

    public IEnumerator Display_Result(Sprite resultImg)
    {
        levelManager.rotationResultImg.enabled = true;
        levelManager.rotationResultImg.sprite = resultImg;
//        Time.timeScale = 0f;
        levelManager.TimerCheck =  false;
        yield return new WaitForSeconds(1f);
        levelManager.TimerCheck = true;
        //  StopCoroutine(levelManager.WaitAndSwitchScreen(0));
        //    yield return null; //new WaitForSeconds(0.5f);
        levelManager.countdownTime = 0.0f;
        //  StopCoroutine("TimerScreen");
   //  Time.timeScale = 1;
        levelManager.ShowCircleScreenAgain();
    }
}
