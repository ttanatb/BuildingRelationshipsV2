using UnityEngine;

namespace Dialogue
{
    public class ActorPositionAnchor : MonoBehaviour
    {
        [SerializeField]
        private string m_posId = "";
        
        private void Start()
        {
            ActorPositionManager.Instance.Register(m_posId, this);
        }
    }
}
