using Dialogue.SO;
using Dialogue.Struct;
using NaughtyAttributes;
using UnityEngine;
using Utilr.Attributes;

namespace Dialogue
{
    public class PlayerAnimController : MonoBehaviour
    {
        [IncludeAllAssetsWithType]
        [SerializeField] private ActorAnimationEvent[] m_actorAnimEvents = null;
        
        [SerializeField] private string m_actorId = "";

        [SerializeField]
        private Animator m_animator = null;

        [SerializeField] [AnimatorParam("m_animator")]
        private int m_animTriggerStartBouncing = 1;

        [SerializeField] [AnimatorParam("m_animator")]
        private int m_animTriggerStopBouncing = 1;

        private void Start()
        {
            ActorManager.Instance.Add(m_actorId, null);

            foreach (var evt in m_actorAnimEvents)
            {
                evt.Event.AddListener(OnActorAnim);
            }
        }
 
        private void OnDestroy()
        {
            foreach (var evt in m_actorAnimEvents)
            {
                evt.Event.RemoveListener(OnActorAnim);
            }
        }
        
        private void OnActorAnim(ActorAnimationData data)
        {
            if (data.ActorId != m_actorId)
                return;

            switch (data.TriggerName)
            {
                case ActorAnimationData.AnimTrigger.StartBouncing:
                    m_animator.ResetTrigger(m_animTriggerStopBouncing);
                    m_animator.SetTrigger(m_animTriggerStartBouncing);
                    break;
                case ActorAnimationData.AnimTrigger.StopBouncing:
                    m_animator.ResetTrigger(m_animTriggerStartBouncing);
                    m_animator.SetTrigger(m_animTriggerStopBouncing);
                    break;
                default:
                    Debug.LogWarning($"{gameObject} ({data.ActorId}) does not support {data.TriggerName}");
                    break;
            }
        }
    }
}
