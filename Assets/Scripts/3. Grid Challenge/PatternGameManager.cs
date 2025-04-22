using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class PatternGameManager : MonoBehaviour
{
    public List<Transform> leftPoints;  // List of points on the left grid
    public List<Transform> rightPoints; // List of points on the right grid


    public Button yesButton; // Yes button for checking the symmetry
    public Button noButton; // No button for checking the difference

    private List<int> scaledLeftPoints = new List<int>();  // To track the scaled left points
    private List<int> scaledRightPoints = new List<int>(); // To track the scaled right points
    private bool isSymmetrical = true; // To track whether the pattern is symmetrical or not

    public static PatternGameManager instance;
    LevelManager levelManager;

    public int patternScores;
    void Start()
    {
        levelManager = LevelManager.instance;
        instance = this;

        
        // Add listeners for Yes and No buttons
        yesButton.onClick.AddListener(OnYesButtonPressed);
        noButton.onClick.AddListener(OnNoButtonPressed);

        // Create the pattern on start
       // CreatePattern();
    }

    public void CreatePattern()
    {
        // Clear previous scaling if any
        ClearPattern();

        // Step 1: Randomly select 8-13 points to scale on the left grid
        int numberOfPointsToScale = Random.Range(8, 14);  // Random number between 8 and 13
        scaledLeftPoints.Clear(); // Clear any previous selections

        for (int i = 0; i < numberOfPointsToScale; i++)
        {
            int randomIndex = Random.Range(0, leftPoints.Count); // Randomly pick a point from the left
            if (!scaledLeftPoints.Contains(randomIndex)) // Ensure no duplicates
            {
                scaledLeftPoints.Add(randomIndex);
                leftPoints[randomIndex].localScale = new Vector3(4f, 4f, 1f);  // Scale the selected point
            }
        }

        // Step 2: Decide if the pattern should be symmetrical or asymmetrical
        isSymmetrical = Random.Range(0, 2) == 0;  // 50% chance for symmetry or asymmetry

        scaledRightPoints.Clear(); // Clear any previous mirrored points

        // Step 3: Mirror all scaled points from left to right
        foreach (int index in scaledLeftPoints)
        {
            rightPoints[index].localScale = new Vector3(4f, 4f, 1f);  // Copy the scale to the right side
            scaledRightPoints.Add(index);  // Track the mirrored right points
        }

        // Step 4: Adjust for asymmetry if needed
        if (!isSymmetrical)
        {
            // Randomly select 3-4 points from the already scaled right points to unscale
            int pointsToUnscale = Random.Range(3, 5);
            HashSet<int> unscaledIndices = new HashSet<int>();

            while (unscaledIndices.Count < pointsToUnscale)
            {
                int randomIndex = scaledRightPoints[Random.Range(0, scaledRightPoints.Count)];
                if (!unscaledIndices.Contains(randomIndex))
                {
                    rightPoints[randomIndex].localScale = Vector3.one; // Reset scale
                    unscaledIndices.Add(randomIndex);
                    scaledRightPoints.Remove(randomIndex); // Remove from scaled points
                }
            }

            // Randomly select 3-4 unscaled points on the right to scale, keeping the count the same
            int pointsToScale = pointsToUnscale;
            while (pointsToScale > 0)
            {
                int randomIndex = Random.Range(0, rightPoints.Count);
                if (!scaledRightPoints.Contains(randomIndex) && !unscaledIndices.Contains(randomIndex))
                {
                    rightPoints[randomIndex].localScale = new Vector3(4f, 4f, 1f);  // Set scale
                    scaledRightPoints.Add(randomIndex);  // Track the newly scaled point
                    pointsToScale--;
                }
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
        bool isSymmetricalCheck = true;

        if (scaledLeftPoints.Count == scaledRightPoints.Count)
        {
            foreach (int index in scaledLeftPoints)
            {
                if (leftPoints[index].localScale != rightPoints[index].localScale)
                {
                    isSymmetricalCheck = false;
                    break;
                }
            }
        }
        else
        {
            isSymmetricalCheck = false;
        }

        if (isSymmetrical && isSymmetricalCheck)
        {
            //  Debug.Log("Success: The pattern is symmetrical!");
            StartCoroutine(Display_Result(levelManager.img_Tick));
            patternScores++;
            levelManager.setScores(1);


        }
        else
        {
            //  Debug.Log("Failure: The pattern is not symmetrical.");
            StartCoroutine(Display_Result(levelManager.img_False));
        }

        // Call AgainShowScreens to reload the CircleScreen
    }

    public void OnNoButtonPressed()
    {
        bool isAsymmetricalCheck = false;

        if (scaledLeftPoints.Count != scaledRightPoints.Count)
        {
            isAsymmetricalCheck = true;
        }
        else
        {
            foreach (int index in scaledLeftPoints)
            {
                if (leftPoints[index].localScale != rightPoints[index].localScale)
                {
                    isAsymmetricalCheck = true;
                    break;
                }
            }
        }

        if (!isSymmetrical && isAsymmetricalCheck)
        {
            patternScores++;
            levelManager.setScores(1);
            //  Debug.Log("Correct: The pattern is asymmetrical.");
            StartCoroutine(Display_Result(levelManager.img_Tick));

        }
        else
        {
            //Debug.Log("Failure: The pattern is symmetrical.");
            StartCoroutine(Display_Result(levelManager.img_False));

        }

        // Call AgainShowScreens to reload the CircleScreen

    }


    public IEnumerator Display_Result(Sprite resultImg)
    {
        levelManager.gridResultImg.enabled = true;
        levelManager.gridResultImg.sprite = resultImg;

        levelManager.TimerCheck = false;
        yield return new WaitForSeconds(1f);
        levelManager.TimerCheck = true;

        levelManager.countdownTime = 0.0f;
        //  StopCoroutine("TimerScreen");
        levelManager.ShowCircleScreenAgain();
    }


}
