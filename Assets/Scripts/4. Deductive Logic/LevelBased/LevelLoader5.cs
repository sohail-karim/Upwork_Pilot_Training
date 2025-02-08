using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;

public class LevelLoader5 : MonoBehaviour
{
    public GridManagerPro5 gridManager; // Reference to your GridManagerPro script
    public string fileNamePrefix = "Level"; // Prefix for the JSON file name


    public static LevelLoader5 instance;

    private void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class LevelData
    {
        public Vector2Int[] cluePoints;
        public string[] clueSprites;
        public Vector2Int questionMarkPoint;
        public string correctSpriteName;
    }


    public void LoadLevelData(int levelno)
    {
        string fileName = $"{fileNamePrefix}{levelno}"; // Without .json
        TextAsset jsonFile = Resources.Load<TextAsset>($"Levels/{fileName}");

        if (jsonFile != null)
        {
            string json = jsonFile.text;
            LevelData levelData = JsonConvert.DeserializeObject<LevelData>(json);

            gridManager.StartGame();

            // Clear existing grid
            gridManager.clearGrid();

            // Load clue points and sprites
            for (int i = 0; i < levelData.cluePoints.Length; i++)
            {
                Vector2Int cluePoint = levelData.cluePoints[i];
                string spriteName = levelData.clueSprites[i];
                gridManager.gridCells[cluePoint.x, cluePoint.y].GetComponent<Image>().sprite = gridManager.shapeSprites.First(sprite => sprite.name == spriteName);
                gridManager.cellFilled[cluePoint.x, cluePoint.y] = true;
            }

            // Load question mark point
            gridManager.questionMarkPosition = levelData.questionMarkPoint;
            Debug.Log(gridManager.questionMarkPosition + " Position");
            gridManager.correctSprite = gridManager.shapeSprites.First(sprite => sprite.name == levelData.correctSpriteName);

            Debug.Log(gridManager.correctSprite + " Correct Sprite ");

            gridManager.gridCells[gridManager.questionMarkPosition.x, gridManager.questionMarkPosition.y].GetComponent<Image>().sprite = gridManager.questionMarkSprite;

            Debug.Log("Level data loaded successfully.");
        }
        else
        {
            Debug.LogError("Level data file not found in Resources.");
        }
    }
}
