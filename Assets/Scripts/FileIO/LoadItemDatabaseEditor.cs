#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoadItemDatabase))]
public class LoadItemDatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LoadItemDatabase convertor = (LoadItemDatabase)target;
        if (GUILayout.Button("Convert"))
        {
            convertor.ReadCSV();
        }
    }
}

#endif