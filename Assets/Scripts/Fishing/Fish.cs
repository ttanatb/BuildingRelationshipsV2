using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    FishDatabaseManager m_fishDatabase = null;
    EventManager m_eventManager = null;

    NavmeshWanderer m_navmeshWanderer = null;

    [SerializeField]
    CollectibleItem.ItemID m_fishID = CollectibleItem.ItemID.Invalid;

    private FishStats m_fishStats = new FishStats();

    private void Start()
    {
        m_fishDatabase = FishDatabaseManager.Instance;
        m_eventManager = EventManager.Instance;
        m_fishStats = m_fishDatabase.GetStatsFor(m_fishID);
        m_navmeshWanderer = GetComponent<NavmeshWanderer>();
    }

    public void TriggerFishEvent()
    {
        m_eventManager.TriggerFishReelStartEvent(m_fishStats, this);
    }

    public void FishingFailedReeling()
    {
        m_navmeshWanderer.SetStopped(false);
    }

    private void Update()
    {

    }
}
