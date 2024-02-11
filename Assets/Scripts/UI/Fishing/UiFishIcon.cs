using Fishing.SO;
using Fishing.Structs;
using GameEvents.Fishing;
using NaughtyAttributes;
using UnityEngine;

public class UiFishIcon : MonoBehaviour
{
    [SerializeField] [Expandable]
    private FishingConfig m_config = null;

    [SerializeField] private Animator m_animator = null;
    [SerializeField] [AnimatorParam("m_animator")] 
    private int m_fishJumpParam = 0;
    [SerializeField] [AnimatorParam("m_animator")] 
    private int m_fishCaptureParam = 0;
    
    [SerializeField] private FishJumpEvent m_jumpEvent = null;
    [SerializeField] private UpdateFishProgressEvent m_progressEvent = null;
    [SerializeField] private StartFishReelEvent m_startFishReelEvent = null;
    [SerializeField] private EndFishReelEvent m_endFishReelEvent = null;
    private float m_prevProgress = 0.0f;

    private void Start()
    {
        m_jumpEvent.Event.AddListener(OnJump);
        m_progressEvent.Event.AddListener(OnFishProgress);
        m_startFishReelEvent.Event.AddListener(OnFishReelStart);
        m_endFishReelEvent.Event.AddListener(OnFishReelEnd);
    }

    private void OnDestroy()
    {
        m_jumpEvent.Event.RemoveListener(OnJump);
        m_progressEvent.Event.RemoveListener(OnFishProgress);
        m_startFishReelEvent.Event.RemoveListener(OnFishReelStart);
        m_endFishReelEvent.Event.RemoveListener(OnFishReelEnd);
    }
    
    private void OnFishReelEnd(FishReelEndData _)
    {
        m_animator.SetBool(m_fishCaptureParam, false);
        m_animator.ResetTrigger(m_fishJumpParam);
    }

    private void OnFishProgress(float progress)
    {
        if (Mathf.Abs(progress - m_prevProgress) < float.Epsilon) return;

        m_animator.SetBool(m_fishCaptureParam, progress > m_prevProgress);
        m_prevProgress = progress;
    }

    private void OnFishReelStart(FishReelStartData _)
    {
        m_prevProgress = m_config.StartCompletionRatio;
    }

    private void OnJump()
    {
        m_animator.SetTrigger(m_fishJumpParam);
    }
}
