using Dialogue.SO;
using UnityEngine;
using Utilr.Utility;

namespace Movement
{
    public class SlPositionPlayer : MonoBehaviour
    {
        [SerializeField]
        [Utilr.Attributes.IncludeAllAssetsWithType]
        private PositionPlayerEvent[] m_positionPlayerEvents = null;

        private PlayerMovement m_playerMovement = null;
        

        private void Start()
        {
            TryGetComponent(out m_playerMovement);
            
            foreach (var evt in m_positionPlayerEvents)
            {
                evt.Event.AddListener(OnPositionPlayer);
            }
        }

        private void OnDestroy()
        {
            foreach (var evt in m_positionPlayerEvents)
            {
                evt.Event.RemoveListener(OnPositionPlayer);
            }
        }
        
        private void OnPositionPlayer(Transform target)
        {
            m_playerMovement.StopMovement();
            var thisTransform = transform;
            thisTransform.position = target.position;
            thisTransform.rotation = target.rotation;

            StartCoroutine(Helper.ExecuteNextFrame(() => {
                m_playerMovement.SetFrozen(false);
            }));
        }
    }
}
