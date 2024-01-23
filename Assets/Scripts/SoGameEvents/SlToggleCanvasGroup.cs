using UnityEngine;
using Utilr.SoGameEvents;
using Utilr.Structs;
using Utilr.Utility;

public class SlToggleCanvasGroup : MonoBehaviour
{
    [SerializeField] private SoGameEvent m_gameEvent = null;
    [SerializeField] private LerpAnimData<float> m_anim = new LerpAnimData<float>()
    {
        Duration = 0.2f,
        Curve = AnimationCurve.EaseInOut(0,0,1,1),
        InitialValue = 0,
        FinalValue = 1,
    };
    [SerializeField] private bool m_isToggle = true;
    
    private CanvasGroup m_canvasGroup = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        TryGetComponent(out m_canvasGroup);
        m_gameEvent.Event.AddListener(OnEventTrigger);
    }
    
    private void OnDestroy()
    {
        m_gameEvent.Event.RemoveListener(OnEventTrigger);
    }
    
    private void OnEventTrigger()
    {
        StopAllCoroutines();
        var anim = m_anim;
        if (m_isToggle && m_canvasGroup.alpha > 0.9f)
        {
            anim.InitialValue = 1.0f;
            anim.FinalValue = 0.0f;
        }
        StartCoroutine(Helper.LerpOverTime(anim, Mathf.Lerp, value => {
            m_canvasGroup.alpha = value;
        }, null));
    }
}
