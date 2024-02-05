using System;
using NaughtyAttributes;
using UI.Events;
using UnityEngine;
using Utilr.Attributes;
using Utilr.SoGameEvents;

namespace UI
{
    public class SlShowBottomUi : MonoBehaviour
    {
        [SerializeField] [IncludeAllAssetsWithType]
        private ShowBottomUiEvent[] m_showBottomUiEvents = null;
        
        [SerializeField]
        private SoGameEvent m_hideBottomUiEvent = null;

        [SerializeField] private PlayOneShotRandomAudioClipEvent m_onSuccessAudio = null;

        [SerializeField] private Animator m_animator = null;
        [SerializeField] [AnimatorParam("m_animator")]
        private int m_showTrigger = 1;
        
        [SerializeField] [AnimatorParam("m_animator")]
        private int m_hideTrigger = 1;

        private void Start()
        {
            foreach (var e in m_showBottomUiEvents)
            {
                e.Event.AddListener(OnShowBottomUi);
            }
            
            m_hideBottomUiEvent.Event.AddListener(OnHideBottomUi);
        }

        private void OnDestroy()
        {
            foreach (var e in m_showBottomUiEvents)
            {
                e.Event.RemoveListener(OnShowBottomUi);
            }
            
            m_hideBottomUiEvent.Event.RemoveListener(OnHideBottomUi);
        }

        private void OnHideBottomUi()
        {
            m_animator.ResetTrigger(m_hideTrigger);
            m_animator.ResetTrigger(m_showTrigger);
            m_animator.SetTrigger(m_hideTrigger);
        }
        
        private void OnShowBottomUi()
        {
            m_animator.ResetTrigger(m_showTrigger);
            m_animator.SetTrigger(m_showTrigger);
        }
    }
}
