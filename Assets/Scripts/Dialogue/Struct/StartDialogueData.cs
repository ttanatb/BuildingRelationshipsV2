using UnityEngine;

namespace Dialogue.Struct
{
    [System.Serializable]
    public struct StartDialogueData
    {
        [field: SerializeField]
        public string NodeName { get; set; }
    }
}
