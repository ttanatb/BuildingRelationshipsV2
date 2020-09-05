#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(WorldGenerator))]
public class WorldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WorldGenerator script = (WorldGenerator)target;
        if (GUILayout.Button("Assign Children"))
        {
            script.AssignChildObjects();
        }
        if (GUILayout.Button("Build Waterway"))
        {
            script.BuildWaterway();
        }
        if (GUILayout.Button("Generate Blocks"))
        {
            script.GenerateBlocks();
        }
    }
}
#endif