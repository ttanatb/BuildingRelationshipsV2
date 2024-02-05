using UnityEngine;
using Fishing.SO;
using Fishing.Structs;
using GameEvents;
using GameEvents.Fishing;
using NaughtyAttributes;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class FishingController : Singleton<FishingController>
{
    [SerializeField] [Expandable]
    private FishingConfig m_config = null;

    [SerializeField] private SoSetUiActiveStateEvent m_fishingUiActiveEvent = null;
    [SerializeField] private SoUpdateFishingCompletionRatioEvent m_updateFishingCompletionRatioEvent = null;
    [SerializeField] private FishReelEndEvent m_fishReelEndEvent = null;

    private FishData m_currFishData = new FishData();
    private Fish m_currFish = null;

    private Vector2 m_fishIconPos = Vector2.zero;
    private Vector2 m_fishingIndicatorPos = Vector2.zero;

    private Vector2 m_fishingIndicatorVel = Vector2.zero;

    private float m_completionRatio = 0.5f;
    private float m_fishJumpTimer = 0.0f;
    private bool m_isActive = false;
    private bool m_shouldDecay = false;

    private UIManager m_uiManager = null;
    private UIFishingController m_ui = null;

    // Use this for initialization
    private void Start()
    {
        m_uiManager = UIManager.Instance;
        m_ui = m_uiManager.FishingControllerUI;
        m_fishingUiActiveEvent.Invoke(false);
        
        Assert.IsNotNull(m_fishReelEndEvent);
    }

    public void StartFishing(FishData stats, Fish fish)
    {
        m_currFishData = stats;
        m_currFish = fish;
        m_fishingUiActiveEvent.Invoke(true);

        m_ui.FishIconLerpRate = stats.FishLerpRate;
        m_completionRatio = m_config.FishUiStartingCompletionRatio;
        m_isActive = true;
        m_fishJumpTimer = 0.0f;
        m_fishingIndicatorVel = Vector2.zero;
        m_shouldDecay = false;

        m_fishIconPos = new Vector2(0, 0.8f);
        m_ui.HardSetPos(m_fishIconPos);
        m_fishingIndicatorPos = Vector2.zero;
        
        m_ui.SetFishIconSize(m_currFishData.FishIconSize);
        m_ui.SetIndicatorSize(m_currFishData.IndicatorSize);
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
        var iconRect = new Rect(m_fishIconPos * m_config.BarSize, m_currFishData.FishIconSize);
        var indicatorRect = new Rect(m_fishingIndicatorPos * m_config.BarSize, m_currFishData.IndicatorSize);

        return iconRect.Overlaps(indicatorRect);
    }

    // Update is called once per frame
    private void Update()
    {
        if (!m_isActive) return;

        if (IsFishInIndicator())
        {
            m_completionRatio += m_currFishData.CompletionRate * Time.deltaTime;
            if (m_completionRatio > 
                m_config.FishUiStartingCompletionRatio + m_config.FishUiStartDecayThreshold)
                m_shouldDecay = true;
        }
        else if (m_shouldDecay)
            m_completionRatio -= m_currFishData.DecayRate * Time.deltaTime;
        m_updateFishingCompletionRatioEvent.Invoke(m_completionRatio);

        if (m_completionRatio <= 0)
        {
            m_fishReelEndEvent.Invoke(new FishReelEndData()
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
            m_fishReelEndEvent.Invoke(new FishReelEndData()
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
                                      m_currFishData.MinBounds.x,
                                      m_currFishData.MaxBounds.x);


        m_fishIconPos.y = Mathf.Clamp(m_fishIconPos.y,
                                      m_currFishData.MinBounds.y,
                                      m_currFishData.MaxBounds.y);

        m_ui.StartFishIconLerp(m_fishIconPos);
        m_fishJumpTimer = 0.0f;
    }
}
