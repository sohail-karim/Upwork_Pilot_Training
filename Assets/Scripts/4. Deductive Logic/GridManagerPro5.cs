using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Linq;

public class GridManagerPro5 : MonoBehaviour
{
    private int CurrentLevel;

    public GameObject cellPrefab;
    public Transform gridParent;
    public Sprite[] shapeSprites; // Array of 4 unique shape sprites

    public Sprite questionMarkSprite; // Question mark sprite
    public Sprite defaultSprite;
    [HideInInspector]

    public GameObject[,] gridCells = new GameObject[5, 5];

    [HideInInspector]
    public Vector2Int questionMarkPosition;
    [HideInInspector]
    public Sprite correctSprite; // Store the correct sprite for the question mark position
    [HideInInspector]
    public bool[,] cellFilled = new bool[5, 5]; // Track the filled state of each cell
    [HideInInspector]
    public Sprite[,] solutionGrid = new Sprite[5, 5]; // Solution grid for backtracking

    public GameObject buttonPrefab;
    public Transform buttonParent;
    private GameObject[] optionButtons;

    // Popup panel
    public GameObject popupPanel;
    public Transform popupGridParent;
    private Vector2Int selectedCellPosition;

    public static GridManagerPro5 instance;
    public static DeductiveGameManager deductiveGameManager;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
      //    StartGame();
        deductiveGameManager = DeductiveGameManager.instance;
    }

    public void clearGrid()
    {
        // Destroy old grid and buttons if they exist
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        GenerateGrid();
        CreateOptionButtons();

    }

    public void StartGame()
    {

        clearGrid();
        //    LoadLevelData();

        //  GenerateGrid();
        //  CreateOptionButtons();
        GenerateSolutionGrid();
        AssignClues();
    }

    void GenerateGrid()
    {
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                gridCells[row, col] = cell;

                // Ensure the cell has a Button component
                Button cellButton = cell.GetComponent<Button>();
                if (cellButton == null)
                {
                    cellButton = cell.AddComponent<Button>();
                }

                int currentRow = row;
                int currentCol = col;
                cellButton.onClick.AddListener(() => OnGridCellClick(currentRow, currentCol));

                // Initialize cellFilled state
                cellFilled[row, col] = false;
            }
        }
    }

    void CreateOptionButtons()
    {
        optionButtons = new GameObject[shapeSprites.Length];
        for (int i = 0; i < shapeSprites.Length; i++)
        {
            if(i == shapeSprites.Length-1)
            {
                return;
            }
            GameObject button = Instantiate(buttonPrefab, buttonParent);
            button.GetComponent<Image>().sprite = shapeSprites[i];
            int index = i; // Capture the index in a local variable
            button.GetComponent<Button>().onClick.AddListener(() => OnOptionButtonClick(index));
            optionButtons[i] = button;
        }
    }

    void GenerateSolutionGrid()
    {
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                solutionGrid[row, col] = null;
            }
        }

        questionMarkPosition = new Vector2Int(Random.Range(0, 5), Random.Range(0, 5));

        // Place sprites ensuring a unique solution
        if (!SolveGrid(0, 0))
        {
            Debug.LogError("Failed to generate a unique solution.");
        }

        // Determine the correct sprite for the question mark position
        correctSprite = solutionGrid[questionMarkPosition.x, questionMarkPosition.y];
        Debug.Log("Correct Ans: " + questionMarkPosition.x + " - " + questionMarkPosition.y + " - " + correctSprite);

        solutionGrid[questionMarkPosition.x, questionMarkPosition.y] = null;
    }

    bool SolveGrid(int row, int col)
    {
        if (row == 5)
        {
            row = 0;
            col++;
            if (col == 5)
            {
                return true;
            }
        }

        if (solutionGrid[row, col] != null)
        {
            return SolveGrid(row + 1, col);
        }

      

        foreach (Sprite sprite in shapeSprites)
        {
            if (IsValid(row, col, sprite))
            {
                solutionGrid[row, col] = sprite;
                if (SolveGrid(row + 1, col))
                {
                    return true;
                }
                solutionGrid[row, col] = null;
            }
        }

        return false;
    }


    bool IsValid(int row, int col, Sprite sprite)
    {
        for (int i = 0; i < 5; i++)
        {
            if (solutionGrid[row, i] == sprite || solutionGrid[i, col] == sprite)
            {
                return false;
            }
        }

        return true;
    }

    void AssignClues()
    {
        int cluesToPlace = Random.Range(8, 10);
        HashSet<Vector2Int> cluePositions = new HashSet<Vector2Int>();

        while (cluePositions.Count < cluesToPlace)
        {
            int row = Random.Range(0, 5);
            int col = Random.Range(0, 5);

            if (row == questionMarkPosition.x && col == questionMarkPosition.y)
                continue;

            Vector2Int position = new Vector2Int(row, col);

            if (!cluePositions.Contains(position))
            {
                gridCells[row, col].GetComponent<Image>().sprite = solutionGrid[row, col];
                cellFilled[row, col] = true;
                cluePositions.Add(position);
            }
        }

        gridCells[questionMarkPosition.x, questionMarkPosition.y].GetComponent<Image>().sprite = questionMarkSprite;

        // Validate the uniqueness of the solution with given clues
        if (!IsUniqueSolution())
        {
            Debug.LogError("Multiple solutions detected, regenerating clues.");
            StartGame(); // Restart to regenerate clues ensuring uniqueness
        }
    }

    bool IsUniqueSolution()
    {
        // Implement a function to validate if the current grid setup leads to a unique solution
        // This function should use a similar backtracking approach to check for multiple solutions

        // Create a temporary copy of the solution grid
        Sprite[,] tempGrid = new Sprite[5, 5];
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                tempGrid[row, col] = gridCells[row, col].GetComponent<Image>().sprite;
            }
        }

        return ValidateUniqueness(tempGrid);
    }

    bool ValidateUniqueness(Sprite[,] tempGrid)
    {
        int solutionsCount = 0;

        bool Validate(int row, int col)
        {
            if (row == 5)
            {
                row = 0;
                col++;
                if (col == 5)
                {
                    solutionsCount++;
                    return solutionsCount <= 1;
                }
            }

            if (tempGrid[row, col] != null)
            {
                return Validate(row + 1, col);
            }

            foreach (Sprite sprite in shapeSprites)
            {
                if (IsValidInTempGrid(row, col, sprite, tempGrid))
                {
                    tempGrid[row, col] = sprite;
                    if (!Validate(row + 1, col))
                    {
                        return false;
                    }
                    tempGrid[row, col] = null;
                }
            }

            return true;
        }

        Validate(0, 0);
        return solutionsCount == 1;
    }

    bool IsValidInTempGrid(int row, int col, Sprite sprite, Sprite[,] tempGrid)
    {
        for (int i = 0; i < 5; i++)
        {
            if (tempGrid[row, i] == sprite || tempGrid[i, col] == sprite)
            {
                return false;
            }
        }

        return true;
    }

    void OnGridCellClick(int row, int col)
    {
        if (!cellFilled[row, col])
        {
            selectedCellPosition = new Vector2Int(row, col);
            ShowPopup(row, col);
        }
    }

    void ShowPopup(int row, int col)
    {
        popupPanel.SetActive(true);
        //     popupPanel.transform.position = gridCells[row, col].transform.position; // Position popup over the clicked cell

        // Remove old popup buttons if they exist
        foreach (Transform child in popupGridParent)
        {
            Destroy(child.gameObject);
        }

        // Create new popup buttons
        for (int i = 0; i < shapeSprites.Length; i++)
        {
            GameObject button = Instantiate(buttonPrefab, popupGridParent);
            button.GetComponent<Image>().sprite = shapeSprites[i];
            int index = i; // Capture the index in a local variable
            

            if(i < shapeSprites.Length-1)
            {
                button.GetComponent<Button>().onClick.AddListener(() => OnPopupButtonClick(index));
              
            }
            else
            {
                button.GetComponent<Button>().onClick.AddListener(() => onCrossSpriteClicked());
            }
        }
    }


    // THis function is used to add the Xross sign to popup panel and when its called it will remove the sprite. 
    // if user accedently adds the sprite to an empt cell he can remove it with by clicking on cross button.

    // Helping Function to remove sprite
    void onCrossSpriteClicked()
    {
        gridCells[selectedCellPosition.x, selectedCellPosition.y].GetComponent<Image>().sprite = defaultSprite;

        // Update cellFilled state
        //     cellFilled[selectedCellPosition.x, selectedCellPosition.y] = true;

        // Hide the popup
        popupPanel.SetActive(false);
    }

    void OnPopupButtonClick(int index)
    {
        if (index < 0 || index >= shapeSprites.Length)
        {
            Debug.LogError("Index out of bounds: " + index);
            return;
        }

        Sprite selectedSprite = shapeSprites[index];
        // Replace the sprite in the selected cell
        gridCells[selectedCellPosition.x, selectedCellPosition.y].GetComponent<Image>().sprite = selectedSprite;

        // Update cellFilled state
  //     cellFilled[selectedCellPosition.x, selectedCellPosition.y] = true;

        // Hide the popup
        popupPanel.SetActive(false);
    }

    void OnOptionButtonClick(int index)
    {
        if (index < 0 || index >= shapeSprites.Length)
        {
            Debug.LogError("Index out of bounds: " + index);
            return;
        }

        Sprite selectedSprite = shapeSprites[index];
        // Replace the question mark sprite
        gridCells[questionMarkPosition.x, questionMarkPosition.y].GetComponent<Image>().sprite = selectedSprite;


        FindAndHighlightButton(); // It will highlight the correct Sprite. 
        deductiveGameManager.QuestiosnAttempted++;
        // Check if the placement is correct
        if (selectedSprite == correctSprite)
        {
            CurrentLevel++;
            Debug.Log("Correct");
            deductiveGameManager.CorrectAns++;

            PlayerPrefs.GetInt("CurrentLevel", CurrentLevel);
        }
        else
        {
            Debug.Log("Wrong");
        }

        deductiveGameManager.GameOver();
    }


    public void FindAndHighlightButton()
    {
        // Loop through all child objects of buttonParent
        foreach (Transform child in buttonParent)
        {
            // Get the Image component from the child
            Image buttonImage = child.GetComponent<Image>();

            if (buttonImage != null)
            {
                // Compare the sprite of the button with the target sprite
                if (buttonImage.sprite == correctSprite)
                {
                    // Change the color of the button to green if the sprites match
                    buttonImage.color = Color.green;
                    Debug.Log($"Match found on button: {child.name}");
                    return; // Exit the loop after finding the match
                }
            }
        }

        Debug.LogWarning("No button found with the matching sprite.");
    }

    //------------------------------


    [System.Serializable]
    public class LevelData
    {
        public Vector2Int[] cluePoints;
        public string[] clueSprites;
        public Vector2Int questionMarkPoint;
        public string correctSpriteName;
    }


    [ContextMenu("Save Level Data")]
    [ContextMenu("Save Level Data")]
    public void SaveLevelData()
    {
        LevelData levelData = new LevelData();

        // Collect clue points and sprites
        List<Vector2Int> cluePointsList = new List<Vector2Int>();
        List<string> clueSpritesList = new List<string>();
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                if (cellFilled[row, col] && !(row == questionMarkPosition.x && col == questionMarkPosition.y))
                {
                    cluePointsList.Add(new Vector2Int(row, col));
                    clueSpritesList.Add(gridCells[row, col].GetComponent<Image>().sprite.name);
                }
            }
        }
        levelData.cluePoints = cluePointsList.ToArray();
        levelData.clueSprites = clueSpritesList.ToArray();

        // Set question mark position
        levelData.questionMarkPoint = questionMarkPosition;

        // Set correct sprite name
        levelData.correctSpriteName = correctSprite.name;

        // Convert to JSON
        string json = JsonConvert.SerializeObject(levelData, Newtonsoft.Json.Formatting.Indented);

        // Find the next available file name
        int levelNumber = 1;
        string directory = Path.Combine(Application.dataPath, "Scripts\\4. Deductive Logic\\LevelBased\\Levels");
        string filePath;

        do
        {
            filePath = Path.Combine(directory, $"Level{levelNumber}.json");
            levelNumber++;
        } while (File.Exists(filePath));

        // Save to file
        File.WriteAllText(filePath, json);

        Debug.Log("Level data saved to " + filePath);
    }




}