using Cinemachine;
using UnityEngine;

namespace Cam.Struct
{
    [System.Serializable]
    public struct CamTransitionData
    {
        [field: SerializeField] 
        public CinemachineBlendDefinition BlendDefinition { get; set; }
    }
}
