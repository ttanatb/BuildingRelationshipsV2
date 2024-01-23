using UnityEngine;

namespace Input.Structs
{
    [System.Serializable]
    public struct InputCompositePath
    {
        [field: SerializeField]
        public string[] Paths { get; set; }
    }
}
