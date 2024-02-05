using Dialogue.SO;
using UnityEngine;

namespace GameEvents
{
    public class StPositionPlayer : MonoBehaviour
    {
        [SerializeField] private PositionPlayerEvent m_event = null;

        private void Start()
        {
            var data = m_event.Data;
            data.Transform = transform;
            m_event.Data = data;
        }
    }
}
