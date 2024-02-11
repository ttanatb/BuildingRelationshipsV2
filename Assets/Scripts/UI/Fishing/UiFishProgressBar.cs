using Fishing.SO;
using Fishing.Structs;
using GameEvents.Fishing;
using NaughtyAttributes;
using UnityEngine;

namespace UI.Fishing
{
    public class UiFishProgressBar : MonoBehaviour
    {
        [SerializeField] [Expandable]
        private FishingConfig m_config = null;
        
        [SerializeField] private Animator m_animator = null;
        [SerializeField] [AnimatorParam("m_animator")] 
        private int m_fishProgressParam = 0;
        [SerializeField] [AnimatorParam("m_animator")] 
        private int m_fishDrainParam = 0;
    
        [SerializeField] private UpdateFishProgressEvent m_progressEvent = null;
        [SerializeField] private StartFishReelEvent m_startFishReelEvent = null;
        [SerializeField] private EndFishReelEvent m_endFishReelEvent = null;
        private float m_prevProgress = 0.0f;

        private void Start()
        {
            m_progressEvent.Event.AddListener(OnFishProgress);
            m_startFishReelEvent.Event.AddListener(OnFishReelStart);
            m_endFishReelEvent.Event.AddListener(OnFishReelEnd);
        }

        private void OnDestroy()
        {
            m_progressEvent.Event.RemoveListener(OnFishProgress);
            m_startFishReelEvent.Event.RemoveListener(OnFishReelStart);
            m_endFishReelEvent.Event.AddListener(OnFishReelEnd);
        }
        
        private void OnFishReelEnd(FishReelEndData _)
        {
            m_animator.SetBool(m_fishProgressParam, false);
            m_animator.SetBool(m_fishDrainParam, false);
        }

        private void OnFishProgress(float progress)
        {
            if (Mathf.Abs(progress - m_prevProgress) < float.Epsilon) return;
            
            m_animator.SetBool(m_fishProgressParam, progress > m_prevProgress);
            m_animator.SetBool(m_fishDrainParam, progress < m_prevProgress);
            m_prevProgress = progress;
        }

        private void OnFishReelStart(FishReelStartData _)
        {
            m_prevProgress = m_config.StartCompletionRatio;
        }
    }
}
