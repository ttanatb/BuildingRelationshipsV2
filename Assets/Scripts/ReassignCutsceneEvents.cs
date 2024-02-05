using System.Collections.Generic;
using System.Reflection;
using Dialogue;
using GameEvents;
using NaughtyAttributes;
using Dialogue.SO;
#if UNITY_EDITOR
using Cinemachine;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Assertions;
using Utilr.SoGameEvents;

public class ReassignCutsceneEvents : MonoBehaviour
{
    [SerializeField]
    private string m_find = "";

    [SerializeField]
    private string m_replace = "";
    
    [SerializeField]
    private string[] m_folderSearch = null;

    [SerializeField]
    private Transform m_npcAnchor = null;

    #if UNITY_EDITOR
    [Button]
    private void ReassignValue()
    {
        TryGetComponent(out StOnTriggerEnter onTriggerEnter);
        TryGetComponent(out SlToggleTriggerActive toggleTriggerActive);
        var positionPlayer = GetComponentInChildren<StPositionPlayer>();
        var camListeners = GetComponentsInChildren<SlActivateCamListener>();

        ProcessComponent<StOnTriggerEnter, SoGameEventBase>(onTriggerEnter, "m_eventToFire");
        ProcessComponent<StPositionPlayer, PositionPlayerEvent>(positionPlayer, "m_event");
        ProcessComponent<SlToggleTriggerActive, EnableTriggerEvent>(toggleTriggerActive, "m_enableTriggerEvent");
        ProcessComponent<SlToggleTriggerActive, DisableTriggerEvent>(toggleTriggerActive, "m_disableTriggerEvent");
        foreach (var cam in camListeners)
        {
            ProcessComponent<SlActivateCamListener, SoGameEvent>(cam, "m_eventToListenTo");
        }
        
        var cams = GetComponentsInChildren<CinemachineVirtualCamera>();
        foreach (var cam in cams)
        {
            cam.m_Follow = m_npcAnchor;
            if (cam.gameObject.name.Contains("Over Shoulder"))
                cam.m_LookAt = positionPlayer.transform;
            else cam.LookAt = m_npcAnchor; 
        }
        
        EditorUtility.SetDirty(gameObject);
    }

    private void ProcessOnTriggerEnter(StOnTriggerEnter onTriggerEnter)
    {
        var field = typeof(StOnTriggerEnter).GetField("m_eventToFire", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field);
        
        var oldEvent = field.GetValue(onTriggerEnter) as SoGameEventBase;
        Assert.IsNotNull(oldEvent);

        if (!oldEvent.name.Contains(m_find))
        {
            Debug.Log($"Skipping {onTriggerEnter}, field doesn't match search param");
            return;
        }

        string formattedName = oldEvent.name.Replace(m_find, m_replace);
        var results = FindAsset(formattedName, m_folderSearch, typeof(SoGameEventBase));
        foreach (object res in results)
        {
            var gameEvent = res as SoGameEventBase;
            Assert.IsNotNull(gameEvent);
            if (gameEvent.name != formattedName)
                continue;

            field.SetValue(onTriggerEnter, res);
        }
    }
    
    private void ProcessComponent<T, TField>(T component, string fieldName) where TField : ScriptableObject
    {
        var field = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field);
        
        var oldEvent = field.GetValue(component) as TField;
        Assert.IsNotNull(oldEvent);
        
        if (!oldEvent.name.Contains(m_find))
        {
            Debug.Log($"Skipping {component}: value {oldEvent.name} from {fieldName} " +
                $"doesn't match search param ({m_find})");
            return;
        }
        
        string formattedName = oldEvent.name.Replace(m_find, m_replace);
        var results = FindAsset(formattedName, m_folderSearch, typeof(TField));
        foreach (object res in results)
        {
            var gameEvent = res as TField;
            Assert.IsNotNull(gameEvent);
            if (gameEvent.name != formattedName)
                continue;

            field.SetValue(component, res);
        }
    }

    
    
    private static IEnumerable<object> FindAsset(string filter, string[] folder, System.Type type)
    {
        string[] guids = AssetDatabase.FindAssets(filter);
        object[] a = new object[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath(path, type);
        }

        return a;
    }
    #endif
}
