using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LevelImporter : EditorWindow
{
    private string jsonFilePath = "Assets/Scripts/5. Motion challenge/Customlevels/levels.json"; // Path to the JSON file

    [MenuItem("Window/Level Importer")]
    public static void ShowWindow()
    {
        GetWindow<LevelImporter>("Level Importer");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Importer", EditorStyles.boldLabel);

        jsonFilePath = EditorGUILayout.TextField("JSON File Path", jsonFilePath);

        if (GUILayout.Button("Import Levels"))
        {
            ImportLevelsFromJSON();
        }
    }

    void ImportLevelsFromJSON()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogError("JSON file not found!");
            return;
        }

        string jsonContent = File.ReadAllText(jsonFilePath);
        LevelDataList levelList;
        try
        {
            levelList = JsonUtility.FromJson<LevelDataList>(jsonContent);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to parse JSON: {e.Message}");
            return;
        }

        for (int i = 0; i < levelList.levels.Count; i++)
        {
            var level = levelList.levels[i];
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
            levelData.rows = level.rows;
            levelData.cols = level.cols;
            levelData.redPosition = level.redPosition;
            levelData.blackPosition = level.blackPosition;
            levelData.obstacles = level.obstacles;

            // Create a custom name for each asset file
            string assetName = $"Level_{i + 1}_Grid_{level.rows}x{level.cols}";
            string assetPath = $"Assets/Scripts/5. Motion challenge/Customlevels/{assetName}.asset";

            AssetDatabase.CreateAsset(levelData, assetPath);

        }

        AssetDatabase.SaveAssets();
        Debug.Log("Levels imported and saved successfully!");
    }

    [System.Serializable]
    public class LevelDataList
    {
        public List<LevelDataObject> levels;
    }

    [System.Serializable]
    public class LevelDataObject
    {
        public int rows;
        public int cols;
        public Vector2Int redPosition;
        public Vector2Int blackPosition;
        public List<ObstacleData> obstacles;
    }
}
