using Dialogue.SO;
using UnityEngine;

namespace GameEvents
{
    public class StPositionPlayer : MonoBehaviour
    {
        [SerializeField] private PositionPlayerEvent m_event = null;

        private void Start()
        {
            m_event.Data = transform;
        }
    }
}
