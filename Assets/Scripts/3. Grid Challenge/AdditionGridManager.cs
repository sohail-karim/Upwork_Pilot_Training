using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionGridManager : MonoBehaviour
{
    public GameObject linePrefab; // Prefab of the line renderer
    public List<RectTransform> LeftGridPoints = new List<RectTransform>(); // List of grid points (RectTransforms)
    public List<RectTransform> RightGridPoints = new List<RectTransform>(); // List of grid points (RectTransforms)
    public List<RectTransform> sumGridPoints = new List<RectTransform>(); // List of grid points (RectTransforms)
    private List<(RectTransform, RectTransform)> drawnLines = new List<(RectTransform, RectTransform)>();
    private List<(RectTransform, RectTransform)> secondGridLines = new List<(RectTransform, RectTransform)>();
    HashSet<(RectTransform, RectTransform)> combinedLines = new HashSet<(RectTransform, RectTransform)>();

    //Just to manage how many total Lines were created so to later on clear them easily. 
    private List<GameObject> instantiatedLines = new List<GameObject>(); // Store all line objects



    [Space]
    public Button yesButton; // Yes button for checking the symmetry
    public Button noButton; // No button for checking the difference


    public static AdditionGridManager instance;

    public bool AnsStatus= false;

    public int patternScores = 0;

    LevelManager levelManager;
    private void Awake()
    {
        instance = this;
    }
    bool correctAnswer;
    int namesCounter = 0;
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

    void ClearLines()
    {

        // Find all GameObjects with the tag "Circle"
        GameObject[] circles = GameObject.FindGameObjectsWithTag("Finish");

        // Loop through and destroy each one
        foreach (GameObject circle in circles)
        {
            Destroy(circle);
        }

        Debug.Log($"{circles.Length} Circle objects destroyed.");

        // Destroy all instantiated line GameObjects
        foreach (GameObject line in instantiatedLines)
        {
            Destroy(line); // Destroy each line
        }

        instantiatedLines.Clear(); // Clear the GameObject list
        Debug.Log("All visual lines cleared from the screen.");

        // Clear stored line data
        drawnLines.Clear();
        secondGridLines.Clear();
        combinedLines.Clear();
        Debug.Log("All stored line data cleared.");
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
         
      CombineGridsIntoSumGrid(RotateorNot);
    }

    void AddLineToDrawnLines(RectTransform start, RectTransform end)
    {
        // Add the line in both directions since the order of points doesn't matter
        drawnLines.Add((start, end));
        drawnLines.Add((end, start));
    }

    bool IsLineDrawn(RectTransform start, RectTransform end)
    {
        // Check if the line in either direction has been drawn
        return drawnLines.Contains((start, end)) || drawnLines.Contains((end, start));
    }
    void DrawRandomLineForSecondGrid()
    {
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
        secondGridLines.Add((startPoint, endPoint));
        // Create the unique line between the points
        CreateLine(startPoint, endPoint);
    }
    bool IsLineDrawnInFirstGrid(RectTransform start, RectTransform end)
    {
        // Check if the line exists in the first grid's drawn lines
        return drawnLines.Contains((start, end)) || drawnLines.Contains((end, start));
    }

    // Adding both the grid Lines Now 

    void CombineGridsIntoSumGrid(bool randomizeOrder = true)
    {

        // Add lines from both grids while avoiding duplicates
        AddUniqueLines(combinedLines, drawnLines);
        AddUniqueLines(combinedLines, secondGridLines);

        // Convert the HashSet to a List for further processing
        List<(RectTransform, RectTransform)> combinedLinesList = new List<(RectTransform, RectTransform)>(combinedLines);

        // Optionally shuffle and modify lines
        if (randomizeOrder)
        {
            ShuffleLinesAndModifyPoints(combinedLinesList, sumGridPoints);
        }
        // yield return new WaitForSeconds(5f);

        foreach (var line in combinedLines)
        {
            int startIndex = GetGridPointIndex(line.Item1, LeftGridPoints, RightGridPoints);
            int endIndex = GetGridPointIndex(line.Item2, LeftGridPoints, RightGridPoints);

            if (startIndex != -1 && endIndex != -1)
            {
                CreateLine(sumGridPoints[startIndex], sumGridPoints[endIndex]);
            }
        }
    }




    // Utility to shuffle lines
    void ShuffleLinesAndModifyPoints(List<(RectTransform, RectTransform)> lines, List<RectTransform> gridPoints)
    {
        // Shuffle the order of the lines
        for (int i = lines.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = lines[i];
            lines[i] = lines[randomIndex];
            lines[randomIndex] = temp;
        }
        Debug.Log("Lines have been shuffled.");

        // Remove one line randomly
        if (lines.Count > 0)
        {
            int lineToRemoveIndex = Random.Range(0, lines.Count);
            (RectTransform, RectTransform) lineToRemove = lines[lineToRemoveIndex];
            lines.RemoveAt(lineToRemoveIndex);
            Debug.Log($"Removed line: {lineToRemove.Item1.name} -> {lineToRemove.Item2.name}");
        }

        // Add a new valid line
        if (lines.Count > 0)
        {
            RectTransform newStart;
            RectTransform newEnd;

            // Find a valid new line
            int attempts = 0;
            const int maxAttempts = 100;
            do
            {
                newStart = gridPoints[Random.Range(0, gridPoints.Count)];
                newEnd = gridPoints[Random.Range(0, gridPoints.Count)];
                attempts++;
            }
            while (
                (newStart == newEnd || // Avoid self-loops
                 ContainsLine(lines, newStart, newEnd)) && // Avoid duplicate lines
                 attempts < maxAttempts
            );

            // Add the new line if valid
            if (attempts < maxAttempts)
            {
                lines.Add((newStart, newEnd));
                CreateLine(newStart, newEnd);
                AnsStatus = true;
                Debug.Log($"Added new line: {newStart.name} -> {newEnd.name}");

            }
            else
            {
                Debug.LogWarning("Failed to find a valid line modification.");
            }
        }
    }



    bool ContainsLine(List<(RectTransform, RectTransform)> lines, RectTransform start, RectTransform end)
    {
        return lines.Contains((start, end)) || lines.Contains((end, start));
    }

    void AddUniqueLines(HashSet<(RectTransform, RectTransform)> lineSet, List<(RectTransform, RectTransform)> linesToAdd)
    {
        foreach (var line in linesToAdd)
        {
            // Add the line if neither orientation exists in the set
            if (!lineSet.Contains(line) && !lineSet.Contains((line.Item2, line.Item1)))
            {
                lineSet.Add(line);
            }
        }
    }

    // Utility to find the corresponding point index in the Sum Grid
    int GetGridPointIndex(RectTransform point, List<RectTransform> leftGrid, List<RectTransform> rightGrid)
    {
        int index = leftGrid.IndexOf(point);
        if (index == -1) // If not found in the Left Grid, check the Right Grid
        {
            index = rightGrid.IndexOf(point);
        }
        return index;
    }

    // this is used to create a line betwen 2 points. 
    void CreateLine(RectTransform start, RectTransform end)
    {
        // Instantiate a line
        GameObject lineObject = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        namesCounter++;
        lineObject.name = namesCounter.ToString();
        lineRenderer.startWidth = 11f; // Set the width of the line
        lineRenderer.endWidth = 11f;
        lineRenderer.startColor = new Color(57, 57, 57);
        lineRenderer.endColor = new Color(57, 57, 57); ;

        // Set the positions of the line renderer (convert RectTransform to world position)
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end.position);

        // Store the instantiated line object
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
        if (!AnsStatus)
        {
            StartCoroutine(Display_Result(levelManager.img_Tick));
            patternScores++;

        }
        else
        {
            StartCoroutine(Display_Result(levelManager.img_False));
        }
    }

    public void OnNoButtonPressed()
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

    public IEnumerator Display_Result(Sprite resultImg)
    {
        levelManager.AdditionResultImg.enabled = true;
        levelManager.AdditionResultImg.sprite = resultImg;
        levelManager.TimerCheck = false;
        yield return new WaitForSeconds(1f);
        levelManager.TimerCheck = true;

        levelManager.countdownTime = 0.0f;
        //  StopCoroutine("TimerScreen");
       
        ClearLines();
        levelManager.ShowCircleScreenAgain();
    }

}
