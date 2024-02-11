using Fishing.SO;
using Fishing.Structs;
using GameEvents.Fishing;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Utilr.Structs;
using Utilr.Utility;

public class UIFishingController : UiBase
{
    [FormerlySerializedAs("m_updateFishingCompletionRatioEvent")]
    [SerializeField] private UpdateFishProgressEvent m_updateFishProgressEvent = null;
    [SerializeField] private UpdateFishPositionEvent m_fishPosition = null;
    [SerializeField] private StartFishReelEvent m_startFishReelEvent = null;

    [SerializeField] private FishingConfig m_config = null;

    [SerializeField] private RectTransform m_fishIcon = null;

    [SerializeField] private RectTransform m_fishingBackGaugeBar = null;

    [SerializeField] private RectTransform m_fishingIndicator = null;
    private Vector2 m_fishingIndicatorSize = Vector2.zero;

    [SerializeField] private float m_usableWidth = 100.0f;

    [SerializeField] private float m_bottomOffset = 20.0f;

    [SerializeField] private float m_topOffset = 10.0f;

    private Rect m_fishingArea = new Rect();

    public Rect ControllerRect => m_fishingArea;

    [SerializeField] private RectTransform m_completionIndicator = null;
    [SerializeField] private float m_completionBarHeight = 0.0f;

    private float m_completionRatio = 0.5f;
    private Vector2 m_fishIconSize = Vector2.zero;

    [FormerlySerializedAs("completionBounds")]
    [SerializeField]
    private Vector2 m_completionBounds = Vector2.zero;

    [SerializeField] private bool m_isFishInBounds = false;

    [SerializeField] private LerpAnimData<float> m_alphaLerp = new LerpAnimData<float>();


    // UnityAction m_fishInBoundsCb = null;

    // Start is called before the first frame update
    private void Awake()
    {
        m_fishingIndicatorSize = m_fishingIndicator.rect.size;
        float fishingBarHeight = m_fishingBackGaugeBar.rect.height;

        m_fishIconSize = m_fishIcon.rect.size;
        m_fishingArea = new Rect(
            m_fishingIndicator.anchoredPosition.x,
            m_fishingIndicator.anchoredPosition.y,
            m_usableWidth,
            fishingBarHeight - m_topOffset - m_bottomOffset);
    }

    protected override void Start()
    {
        m_updateFishProgressEvent.Event.AddListener(SetCompletionRatio);
        m_fishPosition.Event.AddListener(OnFishPositionUpdate);
        m_startFishReelEvent.Event.AddListener(OnFishStart);
        base.Start();
    }

    protected override void SetActiveState(bool isActive)
    {
        var animData = m_alphaLerp;
        animData.InitialValue = isActive ? 0 : 1;
        animData.FinalValue = isActive ? 1 : 0;
        StartCoroutine(Helper.LerpOverTime(animData, Mathf.Lerp,
            value => m_canvasGroup.alpha = value, null));
    }

    private void OnFishStart(FishReelStartData data)
    {
        SetCompletionRatio(m_config.StartCompletionRatio);

        var fishSize = data.FishData.FishIconSize;
        m_fishIcon.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fishSize.x);
        m_fishIcon.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fishSize.y);
        m_fishIconSize = fishSize;
        
        var indicatorSize = data.FishData.IndicatorSize;
        m_fishingIndicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, indicatorSize.x);
        m_fishingIndicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, indicatorSize.y);
        m_fishingIndicatorSize = indicatorSize;
        
        HardSetPos(m_config.StartFishPos);
    }

    private void OnDestroy()
    {
        m_updateFishProgressEvent.Event.RemoveListener(SetCompletionRatio);
        m_fishPosition.Event.RemoveListener(OnFishPositionUpdate);
        m_startFishReelEvent.Event.RemoveListener(OnFishStart);
    }

    private void OnFishPositionUpdate(FishPositionData data)
    {
        HardSetPos(data.Position);
    }

    private void SetCompletionRatio(float ratio)
    {
        m_completionRatio = ratio;
        m_completionRatio = Mathf.Clamp(m_completionRatio, m_completionBounds.x, m_completionBounds.y);
        float completionHeight = Mathf.LerpUnclamped(0.0f, m_completionBarHeight, m_completionRatio);
        m_completionIndicator.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, completionHeight);
    }

    private void HardSetPos(Vector2 pos)
    {
        var fishPos = Vector2.zero;
        fishPos.x = Mathf.LerpUnclamped(m_fishingArea.xMin, m_fishingArea.xMax, pos.x);
        fishPos.y = Mathf.LerpUnclamped(m_fishingArea.yMin,
            m_fishingArea.yMax - m_fishIconSize.y, pos.y);
        m_fishIcon.anchoredPosition = fishPos;
    }

    public void SetFishingIndicatorPos(Vector2 pos)
    {
        var indicatorPos = Vector2.zero;
        indicatorPos.x = Mathf.LerpUnclamped(m_fishingArea.xMin, m_fishingArea.xMax, pos.x);
        indicatorPos.y = Mathf.LerpUnclamped(m_fishingArea.yMin,
            m_fishingArea.yMax - m_fishingIndicatorSize.y, pos.y);
        m_fishingIndicator.anchoredPosition = indicatorPos;
    }
}
