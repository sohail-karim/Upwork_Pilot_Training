using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private LevelData levelData;
    private Vector2 scrollPos;
    private List<LevelData> levels = new List<LevelData>(); // List to manage levels

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);

        // Levels List
        GUILayout.Label("Levels", EditorStyles.boldLabel);
        if (GUILayout.Button("Load Levels"))
        {
            LoadLevels();
        }

        foreach (var level in levels)
        {
            if (GUILayout.Button(level.name))
            {
                levelData = level;
            }
        }

        // Level Properties
        GUILayout.Label("Level Properties", EditorStyles.boldLabel);

        levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

        if (levelData != null)
        {
            EditorGUILayout.Space();
            levelData.rows = EditorGUILayout.IntField("Rows", levelData.rows);
            levelData.cols = EditorGUILayout.IntField("Cols", levelData.cols);

            EditorGUILayout.Space();
            levelData.redPosition = EditorGUILayout.Vector2IntField("Red Position", levelData.redPosition);
            levelData.blackPosition = EditorGUILayout.Vector2IntField("Black Position", levelData.blackPosition);

            EditorGUILayout.Space();
            if (GUILayout.Button("Add Obstacle"))
            {
                levelData.obstacles.Add(new ObstacleData());
            }

            EditorGUILayout.Space();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < levelData.obstacles.Count; i++)
            {
                EditorGUILayout.LabelField("Obstacle " + (i + 1));
                levelData.obstacles[i].position = EditorGUILayout.Vector2IntField("Position", levelData.obstacles[i].position);
                levelData.obstacles[i].size = EditorGUILayout.Vector2IntField("Size", levelData.obstacles[i].size);
                levelData.obstacles[i].isStatic = EditorGUILayout.Toggle("Static", levelData.obstacles[i].isStatic);

                if (GUILayout.Button("Remove Obstacle"))
                {
                    levelData.obstacles.RemoveAt(i);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(levelData);
        }
    }

    void LoadLevels()
    {
        // Load levels from resources or asset folder
        levels = new List<LevelData>(Resources.LoadAll<LevelData>("Levels"));
    }
}
