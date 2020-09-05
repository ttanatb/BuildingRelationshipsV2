#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(RandomHouseGenerator))]
public class RandomHouseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomHouseGenerator script = (RandomHouseGenerator)target;
        if (GUILayout.Button("Assign Children"))
        {
            script.AssignChildObjects();
        }

        if (GUILayout.Button("Build Object"))
        {
            script.GenerateHouse();
        }
    }
}
#endif