using UI.Events;
using UnityEngine;
using UnityEngine.UI;
using Utilr.Attributes;
using Utilr.Structs;
using Utilr.Utility;

public class BottomUiElement : MonoBehaviour
{
    [SerializeField] [IncludeAllAssetsWithType]
    private ShowBottomUiEvent[] m_showBottomUiEvents = null;

    [SerializeField] private ShowBottomUiEvent m_showEvent = null;
    [SerializeField] private LerpAnimData<float> m_anim = new LerpAnimData<float>()
    {
        Duration = 0.2f,
        Curve = AnimationCurve.EaseInOut(0,0,1,1),
        InitialValue = 0,
        FinalValue = 1,
    };
    
    private CanvasGroup m_canvasGroup = null;
    private RectTransform m_rectTransform = null;

    
    // Start is called before the first frame update
    private void Start()
    {
        TryGetComponent(out m_canvasGroup);
        TryGetComponent(out m_rectTransform);
        m_showEvent.Event.AddListener(OnShowThisInstruction);
        foreach (var e in m_showBottomUiEvents)
        {
            if (e == m_showEvent)
                continue;
            
            e.Event.AddListener(OnShowOtherElement);
        }
    }

    private void OnDestroy()
    {
        m_showEvent.Event.RemoveListener(OnShowThisInstruction);
        foreach (var e in m_showBottomUiEvents)
        {
            if (e == m_showEvent)
                continue;
            
            e.Event.RemoveListener(OnShowOtherElement);
        }
    }
    
    private void OnShowThisInstruction()
    {
        StopAllCoroutines();
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rectTransform);

        StartCoroutine(Helper.LerpOverTime(m_anim, Mathf.Lerp, value => {
            m_canvasGroup.alpha = value;
        }, null));
    }
    
    private void OnShowOtherElement()
    {
        m_canvasGroup.alpha = 0;
    }
}
