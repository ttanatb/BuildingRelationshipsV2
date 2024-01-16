using UnityEngine;
using Utilr.Structs;

namespace VolleyGame.Structs
{
    [System.Serializable]
    public struct VolleyCharacterMovementData
    {
        [field: SerializeField]
        public LerpAnimData<Vector3> AnimData { get; set; }
    }
}
