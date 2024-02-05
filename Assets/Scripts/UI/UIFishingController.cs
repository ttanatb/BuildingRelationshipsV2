using System.Collections;
using System.Collections.Generic;
using GameEvents.Fishing;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIFishingController : UiBase
{
    [SerializeField]
    private SoUpdateFishingCompletionRatioEvent m_updateFishingCompletionRatioEvent = null;
    
    [SerializeField]
    RectTransform m_fishIcon = null;

    [SerializeField]
    RectTransform m_fishingBackGaugeBar = null;

    [SerializeField]
    RectTransform m_fishingIndicator = null;
    Vector2 m_fishingIndicatorSize = Vector2.zero;

    [SerializeField]
    float m_usableWidth = 100.0f;

    [SerializeField]
    float m_bottomOffset = 20.0f;

    [SerializeField]
    float m_topOffset = 10.0f;

    Rect m_fishingArea = new Rect();

    [SerializeField]
    RectTransform m_completionIndicator = null;
    float m_completionBarHeight = 0.0f;

    [SerializeField]
    float m_completionRatio = 0.5f;

    [SerializeField]
    Vector2 m_fishIconPos = Vector2.zero;
    [SerializeField]
    Vector2 m_targetFishIconPosNorm = Vector2.zero;
    [SerializeField]
    Vector2 m_targetFishIconPos = Vector2.zero;

    Vector2 m_fishIconSize = Vector2.zero;

    [SerializeField]
    Vector2 m_fishingIndicatorPos = Vector2.zero;

    [SerializeField]
    Vector2 minBounds = Vector2.zero;

    [SerializeField]
    Vector2 maxBounds = Vector2.zero;

    [SerializeField]
    Vector2 completionBounds = Vector2.zero;

    [SerializeField]
    bool m_isFishInBounds = false;

    [SerializeField]
    float m_fishIconLerpTimer = 0.0f;

    [SerializeField]
    float m_fishIconLerpRate = 2.0f;

    private Vector2 m_prevFishPos = Vector2.zero;
    private Vector2 m_prevFishAnchoredPos = Vector2.zero;


    public float FishIconLerpRate { set { m_fishIconLerpRate = value; } }

    // UnityAction m_fishInBoundsCb = null;

    // Start is called before the first frame update
    private void Awake()
    {
        m_fishingIndicatorSize = m_fishingIndicator.rect.size;
        float fishingBarHeight = m_fishingBackGaugeBar.rect.height;

        m_completionBarHeight = m_completionIndicator.rect.height;
        m_fishIconSize = m_fishIcon.rect.size;

        m_fishingArea = new Rect(
            m_fishingIndicator.anchoredPosition.x,
            m_fishingIndicator.anchoredPosition.y,
            m_usableWidth,
            fishingBarHeight - m_topOffset - m_bottomOffset);
    }
    
    protected override void Start()
    {
        m_updateFishingCompletionRatioEvent.Event.AddListener(SetCompletionRatio);
        base.Start();
    }

    private void SetCompletionRatio(float ratio)
    {
        m_completionRatio = ratio;
        m_completionRatio = Mathf.Clamp(m_completionRatio, completionBounds.x, completionBounds.y);
        float completionHeight = Mathf.LerpUnclamped(0.0f, m_completionBarHeight, m_completionRatio);
        m_completionIndicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, completionHeight);
    }

    public void SetFishIconSize(Vector2 size)
    {
        m_fishIcon.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        m_fishIcon.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }
    
    public void SetIndicatorSize(Vector2 size)
    {
        m_fishingIndicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        m_fishingIndicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }


    public void StartFishIconLerp(Vector2 pos)
    {
        m_prevFishPos = m_fishIconPos;
        m_prevFishAnchoredPos = m_fishIcon.anchoredPosition;
        m_targetFishIconPosNorm = pos;
        Vector2 fishPos = Vector2.zero;
        fishPos.x = Mathf.LerpUnclamped(m_fishingArea.xMin, m_fishingArea.xMax, pos.x);
        fishPos.y = Mathf.LerpUnclamped(m_fishingArea.yMin, m_fishingArea.yMax, pos.y);
        m_targetFishIconPos = fishPos;
        m_fishIconLerpTimer = 0.0f;
    }

    public void HardSetPos(Vector2 pos)
    {
        m_fishIconPos = pos;
        m_prevFishPos = pos;
        m_targetFishIconPosNorm = pos;
        Vector2 fishPos = Vector2.zero;
        fishPos.x = Mathf.LerpUnclamped(m_fishingArea.xMin, m_fishingArea.xMax, m_fishIconPos.x);
        fishPos.y = Mathf.LerpUnclamped(m_fishingArea.yMin, m_fishingArea.yMax, m_fishIconPos.y);
        m_targetFishIconPos = fishPos;
        m_prevFishAnchoredPos = fishPos;
        m_fishIcon.anchoredPosition = fishPos;
        m_fishIconLerpTimer = 1.0f;
    }

    public void SetFishingIndicatorPos(Vector2 pos)
    {
        m_fishingIndicatorPos = pos;
        Vector2 indicatorPos = Vector2.zero;
        indicatorPos.x = Mathf.LerpUnclamped(m_fishingArea.xMin, m_fishingArea.xMax, m_fishingIndicatorPos.x);
        indicatorPos.y = Mathf.LerpUnclamped(m_fishingArea.yMin,
            m_fishingArea.yMax - m_fishingIndicatorSize.y, m_fishingIndicatorPos.y);
        m_fishingIndicator.anchoredPosition = indicatorPos;
    }

    public bool IsFishInBounds()
    {
        var indicatorCorners = new Vector3[4];
        m_fishingIndicator.GetWorldCorners(indicatorCorners);

        var matrix = m_fishingIndicator.worldToLocalMatrix;
        var fishIconPos = matrix.MultiplyPoint(m_fishIcon.position
            + Vector3.left * m_fishIconSize.x / 2.0f
            + Vector3.down * m_fishIconSize.y / 2.0f);
        var fishRect = new Rect(fishIconPos, m_fishIconSize);

        m_isFishInBounds = fishRect.Overlaps(m_fishingIndicator.rect);
        return m_isFishInBounds;
    }

    private void Update()
    {
        m_fishIconPos = Vector2.Lerp(m_prevFishPos,
            m_targetFishIconPosNorm, m_fishIconLerpTimer);
        m_fishIcon.anchoredPosition = Vector2.Lerp(m_prevFishAnchoredPos,
            m_targetFishIconPos, m_fishIconLerpTimer);

        m_fishIconLerpTimer += m_fishIconLerpRate * Time.deltaTime;
    }
}
