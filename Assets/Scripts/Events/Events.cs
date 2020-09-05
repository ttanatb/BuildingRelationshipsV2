using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class ItemEvent : UnityEvent<CollectibleItem.ItemID, int>
{

}


[System.Serializable]
public class FishReelStartEvent : UnityEvent<FishStats, Fish>
{

}


[System.Serializable]
public class FishReelEndedEvent : UnityEvent<bool, CollectibleItem.ItemID, Fish>
{

}


[System.Serializable]
public class PlayerSkillUnlockedEvent : UnityEvent<PlayerSkill>
{

}

[System.Serializable]
public class FlagEvent : UnityEvent<string>
{

}
