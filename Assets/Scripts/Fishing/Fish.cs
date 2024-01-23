using Fishing.SO;
using Fishing.Structs;
using Inventory.Structs;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class Fish : MonoBehaviour
{
    [Expandable] [SerializeField] private FishDatabase m_fishDatabase = null;
    private NavmeshWanderer m_navmeshWanderer = null;

    [FormerlySerializedAs("m_fishIDNew")] 
    [SerializeField] private ItemData.ItemID m_fishID = ItemData.ItemID.Invalid;

    [SerializeField] private FishReelStartEvent m_fishReelStartEvent = null;
    private FishData m_fishData = new FishData();

    private void Start()
    {
        m_fishData = m_fishDatabase.Dict[m_fishID];
        m_navmeshWanderer = GetComponent<NavmeshWanderer>();
    }

    public void TriggerFishEvent()
    {
        m_fishReelStartEvent.Invoke(new FishReelStartData()
        {
            FishData = m_fishData,
            Fish = this,
        });
    }

    public void FishingFailedReeling()
    {
        m_navmeshWanderer.SetStopped(false);
    }

}
