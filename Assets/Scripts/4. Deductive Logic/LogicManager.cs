using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicManager : MonoBehaviour
{

    public List<Sprite> shapeSprites = new List<Sprite>();

    public Sprite questionMark;
    public GameObject cellPrefab; // Prefab for grid cells
    public Transform gridParent; // Parent object for the grid


    public List<GameObject> SpawnedButtons = new List<GameObject>();
    List<int> selectedCells = new List<int>();

    private GameObject[,] gridCells = new GameObject[4, 4]; // 4x4 grid

    void Start()
    {
        
        GenerateGrid();
    }

    // 1. Generate the 4x4 Grid
    public void GenerateGrid()
    {
        //clearing the remaining logs
        SpawnedButtons.Clear();
        selectedCells.Clear();

        foreach (Transform child in gridParent) Destroy(child.gameObject);




        // Step 1: Create the Grid
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent); // Instantiate a cell
                gridCells[row, col] = cell; // Store it in the grid array
                SpawnedButtons.Add(cell); // Add the actual instantiated cell to the list
            }
        }

        // Step 2: Select 6 Unique Random Shapes
        if (shapeSprites.Count < 6)
        {
            Debug.LogError("Not enough shapes to select 6 unique ones!");
            return;
        }

        while (selectedCells.Count < 6)
        {
            int randomShapeIndex = Random.Range(0, shapeSprites.Count); // Randomly select a shape index
            if (!selectedCells.Contains(randomShapeIndex))
            {
                selectedCells.Add(randomShapeIndex); // Add to the list of selected shapes
                Debug.Log(randomShapeIndex); // Log the selected shape index
            }
        }


        MarkRandomCellsWithShapes();
    }

    void MarkRandomCellsWithShapes()
    {
        // Shuffle the SpawnedButtons list to pick random cells
        List<GameObject> shuffledButtons = new List<GameObject>(SpawnedButtons);
        for (int i = 0; i < shuffledButtons.Count; i++)
        {
            int randomIndex = Random.Range(0, shuffledButtons.Count);
            GameObject temp = shuffledButtons[i];
            shuffledButtons[i] = shuffledButtons[randomIndex];
            shuffledButtons[randomIndex] = temp;
        }

        // Assign shapes to the first 6 shuffled cells
        for (int i = 0; i < selectedCells.Count; i++)
        {
            int shapeIndex = selectedCells[i]; // Get the shape index
            Sprite shapeSprite = shapeSprites[shapeIndex]; // Get the sprite
            GameObject cell = shuffledButtons[i]; // Get a random cell

            // Assign the sprite to the Image component of the cell
            Image cellImage = cell.GetComponent<Image>();
            if (cellImage != null)
            {
                cellImage.sprite = shapeSprite;
            }
            else
            {
                Debug.LogError("Cell does not have an Image component!");
            }
        }


        shuffledButtons.Clear();
    }


}
