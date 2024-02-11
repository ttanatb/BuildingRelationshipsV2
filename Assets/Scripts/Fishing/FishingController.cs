using System;
using UnityEngine;
using Fishing.SO;
using Fishing.Structs;
using GameEvents;
using GameEvents.Fishing;
using NaughtyAttributes;
using Sound.SO;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using Yarn.Compiler;
using Random = UnityEngine.Random;

public class FishingController : Singleton<FishingController>
{
    [SerializeField] [Expandable]
    private FishingConfig m_config = null;

    [SerializeField] private SoSetUiActiveStateEvent m_fishingUiActiveEvent = null;
    [FormerlySerializedAs("m_updateFishingCompletionRatioEvent")] [SerializeField] 
    private UpdateFishProgressEvent m_updateFishProgressEvent = null;
    [FormerlySerializedAs("m_fishReelEndEvent")] [SerializeField] 
    private EndFishReelEvent m_endFishReelEvent = null;
    [SerializeField] private UpdateFishPositionEvent m_fishPosition = null;
    [SerializeField] private FishJumpEvent m_fishJumpEvent = null;
    [SerializeField] private AnimationCurve m_curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private PlayLoopingAudioEvent m_fishReelLoopSound = null;
    [SerializeField] private float m_defaultPitch = 0.9f;
    [SerializeField] private float m_badPitch = 0.7f;
    [SerializeField] private float m_goodPitch = 1.0f;
    
    private FishData m_currFishData = new FishData();
    private Fish m_currFish = null;

    private Vector2 m_fishIconPos = Vector2.zero;
    private Vector2 m_targetFishPos = Vector2.zero;
    private Vector2 m_prevFishPos = Vector2.zero;
    private float m_fishIconLerpTimer = 0.0f;
    
    private Vector2 m_fishingIndicatorPos = Vector2.zero;

    private Vector2 m_fishingIndicatorVel = Vector2.zero;

    private float m_completionRatio = 0.5f;
    private float m_fishJumpTimer = 0.0f;
    private bool m_isActive = false;
    [SerializeField]
    private bool m_shouldDecay = false;

    private UIManager m_uiManager = null;
    private UIFishingController m_ui = null;

    // Use this for initialization
    private void Start()
    {
        m_uiManager = UIManager.Instance;
        m_ui = m_uiManager.FishingControllerUI;
        m_fishingUiActiveEvent.Invoke(false);
        
        Assert.IsNotNull(m_endFishReelEvent);
    }

    public void StartFishing(FishData stats, Fish fish)
    {
        m_currFishData = stats;
        m_currFish = fish;
        m_fishingUiActiveEvent.Invoke(true);

        m_completionRatio = m_config.StartCompletionRatio;
        m_isActive = true;
        m_fishJumpTimer = 0.0f;
        m_fishingIndicatorVel = Vector2.zero;
        m_shouldDecay = false;

        m_fishIconPos = m_config.StartFishPos;
        m_prevFishPos = m_fishIconPos;
        m_targetFishPos = m_fishIconPos;
        m_fishingIndicatorPos = Vector2.zero;
        m_fishReelLoopSound.InvokePitch(m_defaultPitch);
    }

    public void AdjustPosVertical(float vel)
    {
        m_fishingIndicatorVel.y = (vel * m_config.FishUiIndicatorSpeed);
    }

    public void AdjustPosHorizontal(float vel)
    {
        m_fishingIndicatorVel.x = (vel * m_config.FishUiIndicatorSpeed);
    }

    private bool IsFishInIndicator()
    {
        var bounds = m_ui.ControllerRect;
        var iconRect = new Rect(new Vector2() {
            x = Mathf.LerpUnclamped(bounds.xMin, bounds.xMax, m_fishIconPos.x),
            y = Mathf.LerpUnclamped(bounds.yMin, bounds.yMax - m_currFishData.FishIconSize.y,
                m_fishIconPos.y),
        }, m_currFishData.FishIconSize);
        var indicatorRect = new Rect(new Vector2() {
            x = Mathf.LerpUnclamped(bounds.xMin, bounds.xMax, m_fishingIndicatorPos.x),
            y = Mathf.LerpUnclamped(bounds.yMin, bounds.yMax - m_currFishData.IndicatorSize.y, 
                m_fishingIndicatorPos.y),
        }, m_currFishData.IndicatorSize);

        return iconRect.Overlaps(indicatorRect);
    }

    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawSphere(m_ui.transform.position, m_iconRectDebug.height / 2.0f);
    //
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawSphere(m_ui.transform.position, m_indicatorRectDebug.height / 2.0f);
    // }

    // Update is called once per frame
    private void Update()
    {
        if (!m_isActive) return;
        
        // Move the fish
        m_fishIconLerpTimer += m_currFishData.FishLerpRate * Time.deltaTime;
        m_fishIconLerpTimer = Mathf.Clamp01(m_fishIconLerpTimer);
        float posLerpTimer = m_curve.Evaluate(m_fishIconLerpTimer);
        m_fishIconPos = Vector2.Lerp(m_prevFishPos, m_targetFishPos, posLerpTimer);
        m_fishPosition.Invoke(new FishPositionData() { Position = m_fishIconPos });

        if (IsFishInIndicator())
        {
            m_completionRatio += m_currFishData.CompletionRate * Time.deltaTime;
            m_fishReelLoopSound.InvokePitch(m_goodPitch);

            // Threshold before starting to decay, skip progress event until past threshold
            if (!m_shouldDecay && 
                m_completionRatio > m_config.StartCompletionRatio + m_config.FishUiStartDecayThreshold)
            {
                m_shouldDecay = true;
            }
            else
            {
                m_updateFishProgressEvent.Invoke(m_completionRatio);
            }
        }
        else if (m_shouldDecay)
        {
            m_fishReelLoopSound.InvokePitch(m_badPitch);

            m_completionRatio -= m_currFishData.DecayRate * Time.deltaTime;
            m_updateFishProgressEvent.Invoke(m_completionRatio);
        }
        

        if (m_completionRatio <= 0)
        {
            m_endFishReelEvent.Invoke(new FishReelEndData()
            {
                Success = false, Id = m_currFishData.Id, Fish = m_currFish,
            });
            m_fishingUiActiveEvent.Invoke(false);
            m_currFish.FishingFailedReeling();
            m_isActive = false;
        }
        else if (m_completionRatio >= 1 - float.Epsilon)
        {
            m_isActive = false;
            m_fishingUiActiveEvent.Invoke(false);
            m_endFishReelEvent.Invoke(new FishReelEndData()
            {
                Success = true, Id = m_currFishData.Id, Fish = m_currFish,
            });
        }

        if (m_fishJumpTimer > m_currFishData.JumpIntervalSec)
        {
            FishJump();
        }
        else
        {
            m_fishJumpTimer += Time.deltaTime;
        }

        m_fishingIndicatorPos += m_fishingIndicatorVel * Time.deltaTime;
        m_fishingIndicatorPos.x = Mathf.Clamp(m_fishingIndicatorPos.x,
            m_currFishData.MinBounds.x, m_currFishData.MaxBounds.x);
        m_fishingIndicatorPos.y = Mathf.Clamp(m_fishingIndicatorPos.y,
            m_currFishData.MinBounds.y, m_currFishData.MaxBounds.y);

        m_ui.SetFishingIndicatorPos(m_fishingIndicatorPos);
    }

    private void FishJump()
    {
        var offset = new Vector2(
            Random.Range(m_currFishData.MinJumpDistance.x,
                         m_currFishData.MaxJumpDistance.x),
            Random.Range(m_currFishData.MinJumpDistance.y,
                 m_currFishData.MaxJumpDistance.y));
        if (Random.value < 0.5) offset.x = -offset.x;
        if (Random.value < 0.5) offset.y = -offset.y;

        m_prevFishPos = m_fishIconPos;

        m_targetFishPos = m_fishIconPos + offset;
        m_targetFishPos.x = Mathf.Clamp(m_targetFishPos.x,
                                      m_currFishData.MinBounds.x,
                                      m_currFishData.MaxBounds.x);
        m_targetFishPos.y = Mathf.Clamp(m_targetFishPos.y,
                                      m_currFishData.MinBounds.y,
                                      m_currFishData.MaxBounds.y);

        m_fishJumpTimer = 0.0f;
        m_fishIconLerpTimer = 0.0f;
        m_fishJumpEvent.Invoke();
    }
}
