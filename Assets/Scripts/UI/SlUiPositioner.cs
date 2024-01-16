using System;
using UI.Events;
using UI.Structs;
using UnityEngine;
using Utilr.Attributes;

public class SlUiPositioner : MonoBehaviour
{
    [IncludeAllAssetsWithType]
    [SerializeField] private PositionUiEvent[] m_positionUiEvent = null;
    private RectTransform m_rectTransform = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        TryGetComponent(out m_rectTransform);
        foreach (var e in m_positionUiEvent)
        {
            e.Event.AddListener(OnPositionUi);
        }
    }
    
    private void OnDestroy()
    {
        foreach (var e in m_positionUiEvent)
        {
            e.Event.RemoveListener(OnPositionUi);
        }
    }

    private void OnPositionUi(PositionUiData data)
    {
        var min = m_rectTransform.anchorMin;
        var max = m_rectTransform.anchorMax;
        var negation = Vector2.one;
        
        switch (data.Anchor)
        {
            case TextAnchor.UpperLeft:
            case TextAnchor.UpperCenter:
            case TextAnchor.UpperRight:
                min.y = 1;
                max.y = 1;
                negation.y = -1;
                break;
            case TextAnchor.MiddleLeft:
            case TextAnchor.MiddleCenter:
            case TextAnchor.MiddleRight:
                min.y = 0.5f;
                max.y = 0.5f;
                break;
            case TextAnchor.LowerLeft:
            case TextAnchor.LowerCenter:
            case TextAnchor.LowerRight:
                min.y = 0;
                max.y = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        switch (data.Anchor)
        {
            case TextAnchor.UpperLeft:
            case TextAnchor.MiddleLeft:
            case TextAnchor.LowerLeft:
                min.x = 0;
                max.x = 0;
                break;
            case TextAnchor.UpperCenter:
            case TextAnchor.MiddleCenter:
            case TextAnchor.LowerCenter:
                min.x = 0.5f;
                max.x = 0.5f;
                break;
            case TextAnchor.UpperRight:
            case TextAnchor.MiddleRight:
            case TextAnchor.LowerRight:
                min.x = 1;
                max.x = 1;
                negation.x = -1;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        m_rectTransform.anchorMin = min;
        m_rectTransform.anchorMax = max;
        m_rectTransform.pivot = min;
        
        m_rectTransform.anchoredPosition = data.Offset * negation;
    }
}
