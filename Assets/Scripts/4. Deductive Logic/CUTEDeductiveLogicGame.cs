using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CUTEDeductiveLogicGame : MonoBehaviour
{
    public GameObject cellPrefab;  // Prefab for each cell
    public Transform gridParent;   // Parent transform for the grid
    public Sprite[] shapeSprites;  // Array of 4 unique sprites (+, -, *, /)

    private Sprite[,] grid = new Sprite[4, 4];  // The solution grid
    private bool[,] filled = new bool[4, 4];    // To track filled cells
    private Vector2Int questionMarkPosition;    // Position of the empty cell

    void Start()
    {
        GenerateGrid();
        RemoveClues();
        DisplayGrid();
    }

    // Generate the grid using backtracking to ensure uniqueness in rows and columns
    void GenerateGrid()
    {
        if (!FillGridWithBacktracking(0, 0))
        {
            Debug.LogError("Failed to generate a valid Latin square.");
        }
    }

    // Backtracking function to fill the grid with unique symbols
    bool FillGridWithBacktracking(int row, int col)
    {
        if (row == 4)
        {
            row = 0;
            col++;
            if (col == 4)
            {
                return true;  // We've filled all cells
            }
        }

        if (filled[row, col])
        {
            return FillGridWithBacktracking(row + 1, col);
        }

        // Try all possible symbols in this cell
        foreach (Sprite sprite in shapeSprites)
        {
            if (IsValid(row, col, sprite))
            {
                grid[row, col] = sprite;
                filled[row, col] = true;

                if (FillGridWithBacktracking(row + 1, col))
                {
                    return true;
                }

                // Backtrack
                grid[row, col] = null;
                filled[row, col] = false;
            }
        }

        return false;  // No valid placement found, need to backtrack
    }

    // Check if placing the symbol in grid[row, col] is valid
    bool IsValid(int row, int col, Sprite sprite)
    {
        for (int i = 0; i < 4; i++)
        {
            if ((grid[row, i] != null && grid[row, i].Equals(sprite)) || (grid[i, col] != null && grid[i, col].Equals(sprite)))
            {
                return false;  // Symbol already exists in this row or column
            }
        }
        return true;  // Valid placement
    }

    // Remove clues from the grid and check for uniqueness
    void RemoveClues()
    {
        int cluesToRemove = Random.Range(8, 10);  // Random number of clues to remove
        HashSet<Vector2Int> cluePositions = new HashSet<Vector2Int>();

        while (cluePositions.Count < cluesToRemove)
        {
            int row = Random.Range(0, 4);
            int col = Random.Range(0, 4);

            if (cluePositions.Contains(new Vector2Int(row, col)))
                continue;

            // Remove the clue by setting it to null
            grid[row, col] = null;
            filled[row, col] = false;  // Also mark it as not filled
            cluePositions.Add(new Vector2Int(row, col));

            // Check if the puzzle has a unique solution
            if (!HasUniqueSolution())
            {
                grid[row, col] = shapeSprites[Random.Range(0, shapeSprites.Length)];
                filled[row, col] = true;  // Restore clue if puzzle loses uniqueness
                cluePositions.Remove(new Vector2Int(row, col));
            }
        }

        // Final check if the puzzle still has a unique solution
        if (!HasUniqueSolution())
        {
            Debug.LogError("Multiple solutions found, regenerating grid.");
            Start();  // Restart if multiple solutions exist
        }
    }

    // Check if the puzzle has a unique solution
    bool HasUniqueSolution()
    {
        int solutionCount = 0;

        // Make a temporary copy of the grid
        Sprite[,] tempGrid = new Sprite[4, 4];
        System.Array.Copy(grid, tempGrid, grid.Length);

        // Try solving the puzzle using backtracking, count the number of solutions
        if (SolveGrid(tempGrid, 0, 0, ref solutionCount))
        {
            return solutionCount == 1;
        }

        return false;
    }

    // Solve the puzzle recursively and count the number of solutions
    bool SolveGrid(Sprite[,] tempGrid, int row, int col, ref int solutionCount)
    {
        if (row == 4)
        {
            row = 0;
            col++;
            if (col == 4)
            {
                solutionCount++;
                return solutionCount <= 1;  // If more than one solution, return false
            }
        }

        if (tempGrid[row, col] != null)
        {
            return SolveGrid(tempGrid, row + 1, col, ref solutionCount);
        }

        foreach (Sprite sprite in shapeSprites)
        {
            if (IsValidInGrid(tempGrid, row, col, sprite))
            {
                tempGrid[row, col] = sprite;
                if (!SolveGrid(tempGrid, row + 1, col, ref solutionCount))
                {
                    return false;
                }
                tempGrid[row, col] = null;
            }
        }

        return true;
    }

    // Validate if placing the symbol is valid in the temporary grid
    bool IsValidInGrid(Sprite[,] tempGrid, int row, int col, Sprite sprite)
    {
        for (int i = 0; i < 4; i++)
        {
            if ((tempGrid[row, i] != null && tempGrid[row, i].Equals(sprite)) || (tempGrid[i, col] != null && tempGrid[i, col].Equals(sprite)))
            {
                return false;  // Symbol already exists in this row or column
            }
        }
        return true;
    }

    // Display the generated grid in the Unity scene
    void DisplayGrid() 
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                // Set the sprite for the cell if filled
                if (grid[row, col] != null)
                {
                    cell.GetComponent<Image>().sprite = grid[row, col];
                }
                else
                {
                    cell.GetComponent<Image>().sprite = null;  // Empty cell
                }
            }
        }
    }
}
