using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level Design/Level Data")]
public class LevelData : ScriptableObject
{
    public int rows;
    public int cols;
    public Vector2Int redPosition;
    public Vector2Int blackPosition;
    public List<ObstacleData> obstacles;
}

[System.Serializable]
public class ObstacleData
{
    public Vector2Int position;
    public Vector2Int size; // width and height
    public bool isStatic;
}

[System.Serializable]
public class SpawnedObjectData
{
    public string position;
    public int row;
    public int column;

    public SpawnedObjectData(string name, int row, int column)
    {
        this.position = name;
        this.row = row;
        this.column = column;
    }
}


