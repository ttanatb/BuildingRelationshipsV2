using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CollectibleItem
{
    public enum ItemID
    {
        Invalid = 0,
        TestTato = 1,
        FlamiaRowania = 2,
        Barrelcuda = 3,
        ClownFish = 4,
        FarmraisedFish = 5,
        BottomFeeder = 6,
        NurseShark = 7,
        FireGoby = 8,
        Cabezon = 9,
        Boxfish = 10,
        SeaBass = 11,
        CommonCarp = 12,
        WeirdShoe = 13,
        FishingRod = 14,
        SpecialTicket = 15,
        CuriousFeather = 16,
        WeirdSocks = 17,
        kCount,
    }

    public ItemID id;
    public string displayName;
    public string displayNamePlural;
    public string description;
    public Sprite sprite;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("ID ({0}) Name ({1}) Desc ({2}) Sprite ({3}).",
            id, displayName, description, sprite);
        return sb.ToString();
    }
}

public class CollectibleSO : ScriptableObject
{
    public CollectibleItem[] items;
}
