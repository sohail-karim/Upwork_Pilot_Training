using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SubstractionGridManager : MonoBehaviour
{
    public GameObject linePrefab; // Prefab of the line renderer
    public List<RectTransform> LeftGridPoints = new List<RectTransform>(); // List of grid points (RectTransforms)
    public List<RectTransform> RightGridPoints = new List<RectTransform>(); // List of grid points (RectTransforms)
    public List<RectTransform> subGridPoints = new List<RectTransform>(); // List of grid points (RectTransforms)
    private List<(RectTransform, RectTransform)> leftGridLines = new List<(RectTransform, RectTransform)>();
    private List<(RectTransform, RectTransform)> tempGridLines = new List<(RectTransform, RectTransform)>();
    private List<(RectTransform, RectTransform)> rightGridLines = new List<(RectTransform, RectTransform)>();
    List<(RectTransform, RectTransform)> subGridLines = new List<(RectTransform, RectTransform)>();

    //Just to manage how many total Lines were created so to later on clear them easily. 
    private List<GameObject> instantiatedLines = new List<GameObject>(); // Store all line objects



    [Space]
    public Button yesButton; // Yes button for checking the symmetry
    public Button noButton; // No button for checking the difference


    public static SubstractionGridManager instance;

    public bool AnsStatus = false;

    public int patternScores = 0;

    LevelManager levelManager;

    bool correctAnswer;
    int namesCounter = 0;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        levelManager = LevelManager.instance;
        if (LeftGridPoints.Count < 2)
        {
            Debug.LogError("Please assign at least two grid points in the Inspector.");
            return;
        }


        yesButton.onClick.AddListener(OnYesButtonPressed);
        noButton.onClick.AddListener(OnNoButtonPressed);
    }

    public void StartChallenge()
    {
        AnsStatus = false;
        ClearLines();
        bool randomBool = Random.value > 0.5f;
        StartCoroutine(DrawTwoConnectedLines(randomBool));
    }

    public void ClearLines()
    {
        // Find all GameObjects with the tag "Circle"
        GameObject[] circles = GameObject.FindGameObjectsWithTag("Finish");

        // Loop through and destroy each one
        foreach (GameObject circle in circles)
        {
            Destroy(circle);
        }


        // Destroy all instantiated line GameObjects
        foreach (GameObject line in instantiatedLines)
        {
            Destroy(line); // Destroy each line
        }

        instantiatedLines.Clear(); // Clear the GameObject list

        // Clear stored line data
        leftGridLines.Clear();
        rightGridLines.Clear();
        subGridLines.Clear();
        tempGridLines.Clear();
    }


    IEnumerator DrawTwoConnectedLines(bool RotateorNot)
    {
        yield return new WaitForEndOfFrame();

        // Randomly choose the first line
        RectTransform firstStart = GetRandomPoint();
        RectTransform firstEnd = GetRandomNeighbor(firstStart);

        // Ensure the line hasn't already been drawn
        while (IsLineDrawn(firstStart, firstEnd))
        {
            firstStart = GetRandomPoint();
            firstEnd = GetRandomNeighbor(firstStart);
        }
        CreateLine(firstStart, firstEnd);
        AddLineToDrawnLines(firstStart, firstEnd);

        // Choose the second line ensuring it connects to the first line
        RectTransform secondStart = (Random.value < 0.5f) ? firstStart : firstEnd; // Start from one of the first line's points
        RectTransform secondEnd = GetRandomNeighbor(secondStart);

        // Ensure the second line hasn't already been drawn
        while (IsLineDrawn(secondStart, secondEnd))
        {
            secondEnd = GetRandomNeighbor(secondStart);
        }
        CreateLine(secondStart, secondEnd);

        AddLineToDrawnLines(secondStart, secondEnd);
        // creating a single line for Right Grid...
        DrawRandomLineForSecondGrid();
        DrawSubtractionGridLines();
    }

    void AddLineToDrawnLines(RectTransform start, RectTransform end)
    {
        // Add the line in both directions since the order of points doesn't matter
        leftGridLines.Add((start, end));
        tempGridLines.Add((start,end));   // this is used to store generated lines in the temp directory.

        Debug.Log(" LeftGrid Lines " +  leftGridLines.Count);
    }



    bool IsLineDrawn(RectTransform start, RectTransform end)
    {
        // Check if the line in either direction has been drawn
        return leftGridLines.Contains((start, end)) || leftGridLines.Contains((end, start));
    }
    void DrawRandomLineForSecondGrid()
    {
        // Randomly decide whether to copy a line or create a new one (50% chance each)
        bool shouldCopyLine = Random.Range(0f, 1f) < 0.5f;

        if (shouldCopyLine)
        {
            AnsStatus = true;
            Debug.LogWarning("Line Copied ");
            // Copy a line from LeftGrid (using the logic we previously implemented)
            CopyLineFromLeftToRightGrid();
        }
        else
        {
            Debug.LogWarning("New Line  ");
            // Keep trying until a unique line is found
            RectTransform startPoint;
            RectTransform endPoint;
            do
            {
                // Select two random points from the second grid
                startPoint = RightGridPoints[Random.Range(0, RightGridPoints.Count)];
                endPoint = RightGridPoints[Random.Range(0, RightGridPoints.Count)];
            }
            while (startPoint == endPoint || IsLineDrawnInFirstGrid(startPoint, endPoint));
            rightGridLines.Add((startPoint, endPoint));
            // Create the unique line between the points
            CreateLine(startPoint, endPoint);
        }
    }

    void CopyLineFromLeftToRightGrid()
    {
        // Check if there are any lines drawn in LeftGrid
        if (leftGridLines.Count == 0)
        {
            Debug.LogWarning("No lines drawn in LeftGrid to replicate.");
            return; // Exit if no lines are available
        }

        Debug.Log("Left Grid Lines " + leftGridLines.Count); 

        // Randomly pick one line from the drawnLines list
        int randomLineIndex = Random.Range(0, tempGridLines.Count);
        var selectedLine = tempGridLines[randomLineIndex];

        tempGridLines.Remove(selectedLine);

        // Get the start and end points of the selected line from LeftGrid
        RectTransform startPointLeftGrid = selectedLine.Item1;
        RectTransform endPointLeftGrid = selectedLine.Item2;

        // Log the points selected from LeftGrid
        Debug.Log($"Selected line from LeftGrid - StartPoint: {startPointLeftGrid.name}, EndPoint: {endPointLeftGrid.name}");

        // Match the points to the corresponding points in the RightGrid
        int startIndex = LeftGridPoints.IndexOf(startPointLeftGrid);
        int endIndex = LeftGridPoints.IndexOf(endPointLeftGrid);

        // Get the corresponding points from RightGrid
        RectTransform startPointRightGrid = RightGridPoints[startIndex];
        RectTransform endPointRightGrid = RightGridPoints[endIndex];

        // Log the points that will be drawn on the RightGrid
        Debug.Log($"Matching points in RightGrid - StartPoint: {startPointRightGrid.name}, EndPoint: {endPointRightGrid.name}");

        // Create the line on the RightGrid using the matched points
        CreateLine(startPointRightGrid, endPointRightGrid);

        // Add the line to the secondGridLines list to track the lines in the RightGrid
        rightGridLines.Add((startPointRightGrid, endPointRightGrid));

        // Log the result of the line creation on the RightGrid
        Debug.Log($"Line created in RightGrid: StartPoint = {startPointRightGrid.name}, EndPoint = {endPointRightGrid.name}");
    }


    // Method to create a line in the RightGrid using LineRenderer

    bool IsLineDrawnInFirstGrid(RectTransform start, RectTransform end)
    {
        // Check if the line exists in the first grid's drawn lines
        return leftGridLines.Contains((start, end)) || leftGridLines.Contains((end, start));
    }

    // Adding both the grid Lines Now 

    // Method to subtract lines and update Subtraction Grid
    public void DrawSubtractionGridLines()
    {
        // Use a HashSet to avoid duplicates and mirrored lines

        Debug.LogWarning($"Left Grid Lines (Total): {leftGridLines.Count}");
        Debug.LogWarning($"Right Grid Lines (Total): {rightGridLines.Count}");

       
        // Draw the unique lines in the Subtraction Grid
        foreach (var line in tempGridLines)
        {
            int startIndex = LeftGridPoints.IndexOf(line.Item1);
            int endIndex = LeftGridPoints.IndexOf(line.Item2);

            if (startIndex != -1 && endIndex != -1)
            {
                RectTransform startPoint = subGridPoints[startIndex];
                RectTransform endPoint = subGridPoints[endIndex];

                CreateLine(startPoint, endPoint);
                Debug.Log($"Line Drawn in Subtraction Grid: Start = {startPoint.name}, End = {endPoint.name}");
            }
            else
            {
                Debug.LogError("Line points not found in LeftGridPoints for SubGrid!");
            }
        }
    }

   
    // this is used to create a line betwen 2 points. 
    void CreateLine(RectTransform start, RectTransform end)
    {
        GameObject lineObject = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.tag = "Finish";
        lineRenderer.startWidth = 11f;
        lineRenderer.endWidth = 11f;
        lineRenderer.startColor = new Color(57, 57, 57);
        lineRenderer.endColor = new Color(57, 57, 57);

        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end.position);
        instantiatedLines.Add(lineObject);
    }


    RectTransform GetRandomPoint()
    {
        return LeftGridPoints[Random.Range(0, LeftGridPoints.Count)];
    }

    RectTransform GetRandomNeighbor(RectTransform point)
    {
        // Find the index of the given point in the grid
        int index = LeftGridPoints.IndexOf(point);

        if (index == -1)
        {
            Debug.LogError("The given point is not in the gridPoints list.");
            return null;
        }

        // Calculate the row and column of the current point
        int row = index / 3; // Grid is 3x3, so each row contains 3 points
        int col = index % 3;

        // List of valid neighbors
        List<RectTransform> neighbors = new List<RectTransform>();

        // Define all possible relative neighbor positions
        int[,] directions = {
        {-1, -1}, {-1, 0}, {-1, 1}, // Top-left, top, top-right
        { 0, -1},          { 0, 1}, // Left,        , Right
        { 1, -1}, { 1, 0}, { 1, 1}  // Bottom-left, bottom, bottom-right
    };

        // Check each possible neighbor
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newRow = row + directions[i, 0];
            int newCol = col + directions[i, 1];

            // Ensure the new position is within grid bounds
            if (newRow >= 0 && newRow < 3 && newCol >= 0 && newCol < 3)
            {
                int neighborIndex = newRow * 3 + newCol; // Convert back to a single index
                neighbors.Add(LeftGridPoints[neighborIndex]);
            }
        }

        Debug.Log("Neighbors count for point " + index + ": " + neighbors.Count);

        // Return a random neighbor from the list
        return neighbors[Random.Range(0, neighbors.Count)];
    }


    public void OnYesButtonPressed()
    {
        if (AnsStatus)
        {
            patternScores++;
            StartCoroutine(Display_Result(levelManager.img_Tick));
        }
        else
        {
            StartCoroutine(Display_Result(levelManager.img_False));
        }
    }

    public void OnNoButtonPressed()
    {
        if (!AnsStatus)
        {
            patternScores++;
            StartCoroutine(Display_Result(levelManager.img_Tick));
        }
        else
        {
            StartCoroutine(Display_Result(levelManager.img_False));

        }
    }

    public IEnumerator Display_Result(Sprite resultImg)
    {
        levelManager.SubstractionResultImg.enabled = true;
        levelManager.SubstractionResultImg.sprite = resultImg;

        levelManager.TimerCheck = false;
        yield return new WaitForSeconds(1f);
        levelManager.TimerCheck = true;

        levelManager.countdownTime = 0.0f;
        //  StopCoroutine("TimerScreen");
        
        ClearLines();
        levelManager.ShowCircleScreenAgain();
    }
}
