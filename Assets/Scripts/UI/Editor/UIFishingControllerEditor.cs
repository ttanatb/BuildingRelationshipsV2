#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIFishingController))]
public class UIFishingControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UIFishingController script = (UIFishingController)target;
        if (GUILayout.Button("Fire Test Item Event"))
        {
        }
        if (GUILayout.Button("One of each item"))
        {
        }
    }
}

#endif