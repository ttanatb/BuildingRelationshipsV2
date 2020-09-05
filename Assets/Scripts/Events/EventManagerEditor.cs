#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EventManager))]
public class EventManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EventManager script = (EventManager)target;
        if (GUILayout.Button("Fire Test Item Event"))
        {
            script.FireTestItemEvent();
        }
        if (GUILayout.Button("Fire Test Skill Event"))
        {
            script.FireTestSkillUnlockedEvent();
        }
        if (GUILayout.Button("Unlock ALL skills"))
        {
            script.TestUnlockAllSkills();
        }
        if (GUILayout.Button("One of each item"))
        {
            script.AddOneOfEachItem();
        }
    }
}

#endif