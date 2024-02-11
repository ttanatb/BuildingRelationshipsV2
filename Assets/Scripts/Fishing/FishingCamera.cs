using System;
using Cinemachine;
using Cinemachine.PostFX;
using Fishing.SO;
using Fishing.Structs;
using GameEvents.Fishing;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Util.Fishing
{
    public class FishingCamera : MonoBehaviour
    {
        [Expandable] [SerializeField] private FishingConfig m_config = null;
        [SerializeField] private AnimationCurve m_fovCurve = null;
        [SerializeField] private AnimationCurve m_vignetteCurve = null;
        [SerializeField] private float m_defaultSmoothness = 0.2f;
        [SerializeField] private float m_loseProgressSmoothness = 1.0f;
        [SerializeField] private float m_defaultIntensity = 0.25f;
        [SerializeField] private float m_loseProgressIntensity = 0.5f;
        [SerializeField] private float m_smoothnessDuration = 0.5f;
        [SerializeField] private UpdateFishProgressEvent m_updateFishProgressEvent = null;
        [SerializeField] private StartFishReelEvent m_startFishReelEvent = null;
        [SerializeField] private EndFishReelEvent m_endFishReelEvent = null;

        private CinemachineVirtualCamera m_cam = null;
        private Vignette m_vignette = null;
        private float m_prevPercent = 0.0f;
        private float m_prevSmoothness = 0.0f;
        private float m_targetSmoothness = 0.0f;
        private float m_prevIntensity = 0.0f;
        private float m_targetIntensity = 0.0f;
        private bool m_isProgressing = false;
        private float m_timer = 0.0f;
        
        private void Start()
        {
            TryGetComponent(out m_cam);
            TryGetComponent(out CinemachinePostProcessing postProcessing);
            var profile = postProcessing.m_Profile;
            m_vignette = profile.GetSetting<Vignette>();

            m_updateFishProgressEvent.Event.AddListener(OnFishProgress);
            m_startFishReelEvent.Event.AddListener(OnFishStart);
            m_endFishReelEvent.Event.AddListener(OnFishEnd);
        }

        private void OnDestroy()
        {
            m_updateFishProgressEvent.Event.RemoveListener(OnFishProgress);
            m_startFishReelEvent.Event.RemoveListener(OnFishStart);
            m_endFishReelEvent.Event.RemoveListener(OnFishEnd);
        }

        private void Update()
        {
            if (m_timer > m_smoothnessDuration + Time.deltaTime) return;
            
            // Apply curve to timer progression.
            float t = m_vignetteCurve.Evaluate(m_timer / m_smoothnessDuration);
            
            // Convert timer / duration to curr smoothness -> target smoothness
            m_vignette.smoothness.Override(Mathf.Lerp(m_prevSmoothness, m_targetSmoothness, t));
            m_vignette.intensity.Override(Mathf.Lerp(m_prevIntensity, m_targetIntensity, t));
            
            m_timer += Time.deltaTime;
        }

        private void OnFishStart(FishReelStartData _)
        {
            m_prevPercent = m_config.StartCompletionRatio;
            m_targetSmoothness = m_defaultSmoothness;
            m_prevSmoothness = m_targetSmoothness;
            m_timer = 0.0f;
            m_vignette.enabled.Override(true);
            m_vignette.active = true;
            OnFishProgress(m_config.StartCompletionRatio);
        }
        
        private void OnFishEnd(FishReelEndData _)
        {
            m_vignette.enabled.Override(false);
            m_vignette.active = false;
        }


        private void OnFishProgress(float percent)
        {
            m_cam.m_Lens.FieldOfView = m_fovCurve.Evaluate(percent);
            UpdateProgressDelta(percent);
            m_prevPercent = percent;
        }

        private void UpdateProgressDelta(float percent)
        {
            // increasing
            if (percent - m_prevPercent > float.Epsilon)
            {
                if (m_isProgressing) return;
                
                m_timer = 0.0f;
                m_prevSmoothness = m_vignette.smoothness.value;
                m_targetSmoothness = m_defaultSmoothness;
                m_prevIntensity = m_vignette.intensity.value;
                m_targetIntensity = m_defaultIntensity;
                m_isProgressing = true;
            } 
            // decreasing
            else if (m_prevPercent - percent > float.Epsilon)
            {
                if (!m_isProgressing) return;
                
                m_timer = 0.0f;
                m_prevSmoothness = m_vignette.smoothness.value;
                m_targetSmoothness = m_loseProgressSmoothness;
                m_prevIntensity = m_vignette.intensity.value;
                m_targetIntensity = m_loseProgressIntensity;
                m_isProgressing = false;
            }
        }
    }
}
