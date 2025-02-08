using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManagerPro))]
public class GridManagerProEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridManagerPro script = (GridManagerPro)target;

        if (GUILayout.Button("Save Level Data"))
        {
            script.SaveLevelData();
        }
    }
}
