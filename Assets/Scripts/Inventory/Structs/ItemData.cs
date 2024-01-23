using System.Text;
using UnityEngine;

namespace Inventory.Structs
{
    [System.Serializable]
    public struct ItemData
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
            Count,
        }
        
        [field: SerializeField]
        public ItemID Id { get; set; }
        
        [field: SerializeField]
        public string DisplayName { get; set; }
        
        [field: SerializeField]
        public string DisplayNamePlural { get; set; }
        
        [field: SerializeField]
        public string Description { get; set; }
        
        [field: SerializeField]
        public Sprite Sprite { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"ID ({Id}) Name ({DisplayName}) Desc ({Description}) Sprite ({Sprite}).");
            return sb.ToString();
        }
    }
}
