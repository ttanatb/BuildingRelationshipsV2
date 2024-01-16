using UnityEngine;

namespace FlagSystem.Structs
{
    [System.Serializable]
    public struct FlagNodeCompletionStatus
    {
        [field: SerializeField]
        public bool Completed { get; set; }
    }
}
