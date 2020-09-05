using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Events;
using System.Collections.Generic;

public class FishDatabaseManager : Singleton<FishDatabaseManager>
{
    private EventManager m_eventManager = null;

    [SerializeField]
    private FishDatabaseSO m_databaseSO = null;

#if UNITY_EDITOR
    const string PATH_PREFIX = "Assets/ScriptableObjects/";
    const string PATH_SUFFIX = ".asset";

    [SerializeField]
    string soAssetName = "fish";
#endif

    private Dictionary<CollectibleItem.ItemID, FishStats> m_itemDatabase = null;

    public FishStats GetStatsFor(CollectibleItem.ItemID id)
    {
        return m_itemDatabase[id];
    }

    private void Awake()
    {
        m_itemDatabase = new Dictionary<CollectibleItem.ItemID, FishStats>();
#if UNITY_EDITOR
        if (m_databaseSO == null && soAssetName != "")
        {
            m_databaseSO = (FishDatabaseSO)AssetDatabase.
                LoadAssetAtPath(PATH_PREFIX + soAssetName + PATH_SUFFIX, typeof(CollectibleSO));

        }
#endif
    }

    // Use this for initialization
    void Start()
    {
        m_eventManager = EventManager.Instance;


        foreach (var stat in m_databaseSO.fishStats)
        {
            m_itemDatabase.Add(stat.id, stat);
        }
    }


}
