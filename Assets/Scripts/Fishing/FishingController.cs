using UnityEngine;
using System.Collections;
using GameEvents;
using GameEvents.Fishing;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class FishingController : Singleton<FishingController>
{
    const float STARTING_COMPLETION_RATIO = 0.35f;
    const float START_DECAY_THRESHOLD = 0.10f;

    [SerializeField] private SoSetUiActiveStateEvent m_fishingUiActiveEvent = null;
    [SerializeField] private SoUpdateFishingCompletionRatioEvent m_updateFishingCompletionRatioEvent = null;

    FishStats m_currFishStats = new FishStats();
    Fish m_currFish = null;

    Vector2 m_fishIconPos = Vector2.zero;
    Vector2 m_fishingIndicatorPos = Vector2.zero;

    Vector2 m_fishingIndicatorVel = Vector2.zero;

    [SerializeField]
    float m_indicatorSpeed = 1.0f;

    float m_completionRatio = 0.5f;
    float m_fishJumpTimer = 0.0f;
    bool m_isActive = false;
    bool m_shouldDecay = false;

    UIManager m_uiManager = null;
    UIFishingController m_ui = null;
    EventManager m_eventManager = null;

    public void StartFishing(FishStats stats, Fish fish)
    {
        m_currFishStats = stats;
        m_currFish = fish;
        m_fishingUiActiveEvent.Invoke(true);

        m_ui.FishIconLerpRate = stats.FishLerpRate;
        m_completionRatio = STARTING_COMPLETION_RATIO;
        m_isActive = true;
        m_fishJumpTimer = 0.0f;
        m_fishingIndicatorVel = Vector2.zero;
        m_shouldDecay = false;

        m_fishIconPos = new Vector2(0, 0.8f);
        m_ui.HardSetPos(m_fishIconPos);
        m_fishingIndicatorPos = Vector2.zero;
    }

    // Use this for initialization
    void Start()
    {
        m_uiManager = UIManager.Instance;
        m_ui = m_uiManager.FishingControllerUI;
        m_fishingUiActiveEvent.Invoke(false);
        m_eventManager = EventManager.Instance;
    }

    public void AdjustPosVert(float vel)
    {
        m_fishingIndicatorVel.y = (vel * m_indicatorSpeed);
    }

    public void AdjustPosHori(float vel)
    {
        m_fishingIndicatorVel.x = (vel * m_indicatorSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_isActive) return;

        if (m_ui.IsFishInBounds())
        {
            m_completionRatio += m_currFishStats.CompletionRate * Time.deltaTime;
            if (m_completionRatio > STARTING_COMPLETION_RATIO + START_DECAY_THRESHOLD)
                m_shouldDecay = true;
        }
        else if (m_shouldDecay)
            m_completionRatio -= m_currFishStats.DecayRate * Time.deltaTime;
        m_updateFishingCompletionRatioEvent.Invoke(m_completionRatio);

        if (m_completionRatio <= 0)
        {
            m_eventManager.TriggerFishReelEndedEvent(false, m_currFishStats.id, m_currFish);
            m_fishingUiActiveEvent.Invoke(false);
            m_currFish.FishingFailedReeling();
            m_isActive = false;
        }
        else if (m_completionRatio >= 1)
        {
            m_isActive = false;
            m_fishingUiActiveEvent.Invoke(false);
            m_eventManager.TriggerFishReelEndedEvent(true, m_currFishStats.id, m_currFish);
        }

        if (m_fishJumpTimer > m_currFishStats.JumpIntervalSec)
        {
            FishJump();
        }
        else
        {
            m_fishJumpTimer += Time.deltaTime;
        }

        m_fishingIndicatorPos += m_fishingIndicatorVel * Time.deltaTime;
        m_fishingIndicatorPos.x = Mathf.Clamp(m_fishingIndicatorPos.x,
            m_currFishStats.MinBounds.x, m_currFishStats.MaxBounds.x);
        m_fishingIndicatorPos.y = Mathf.Clamp(m_fishingIndicatorPos.y,
            m_currFishStats.MinBounds.y, m_currFishStats.MaxBounds.y);

        m_ui.SetFishingIndicatorPos(m_fishingIndicatorPos);
    }

    void FishJump()
    {
        Vector2 offset = new Vector2(
            Random.Range(m_currFishStats.MinJumpDistance.x,
                         m_currFishStats.MaxJumpDistance.x),
            Random.Range(m_currFishStats.MinJumpDistance.y,
                 m_currFishStats.MaxJumpDistance.y));
        if (Random.value < 0.5) offset.x = -offset.x;
        if (Random.value < 0.5) offset.y = -offset.y;

        m_fishIconPos += offset;
        /**
        if (m_fishIconPos.x < m_currFishStats.MinBounds.x)
        {
            float diff = m_currFishStats.MinBounds.x - m_fishIconPos.x;
            m_fishIconPos.x = m_currFishStats.MinBounds.x + diff;
        }

        if (m_fishIconPos.x > m_currFishStats.MaxBounds.x)
        {
            float diff = m_fishIconPos.x - m_currFishStats.MaxBounds.x;
            m_fishIconPos.x = m_currFishStats.MaxBounds.x - diff;
        }

        if (m_fishIconPos.y < m_currFishStats.MinBounds.y)
        {
            float diff = m_currFishStats.MinBounds.y - m_fishIconPos.y;
            m_fishIconPos.y = m_currFishStats.MinBounds.y + diff;
        }

        if (m_fishIconPos.y > m_currFishStats.MaxBounds.y)
        {
            float diff = m_fishIconPos.y - m_currFishStats.MaxBounds.y;
            m_fishIconPos.y = m_currFishStats.MaxBounds.y - diff;
        }*/

        m_fishIconPos.x = Mathf.Clamp(m_fishIconPos.x,
                                      m_currFishStats.MinBounds.x,
                                      m_currFishStats.MaxBounds.x);


        m_fishIconPos.y = Mathf.Clamp(m_fishIconPos.y,
                                      m_currFishStats.MinBounds.y,
                                      m_currFishStats.MaxBounds.y);

        m_ui.SetFishIconPos(m_fishIconPos);
        m_fishJumpTimer = 0.0f;
    }
}
