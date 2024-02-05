using NaughtyAttributes;
using UnityEngine;
using TMPro;
using UI.Events;
using UI.Structs;
using UnityEngine.UI;
using Utilr.SoGameEvents;

public class UiInteractableMarker : MonoBehaviour
{
    [SerializeField] private RectTransform m_parentCanvas = null;
    [SerializeField] private TextMeshProUGUI m_interactMessage = null;
    [SerializeField] private SoCurrentCamera m_cameraEvent = null;
    [SerializeField] private PositionUiOnWorldPosEvent m_positionEvent = null;
    [SerializeField] private ShowInteractableMarkerEvent m_showEvent = null;
    [SerializeField] private HideInteractableMarkerEvent m_hideEvent = null;

    [SerializeField] private Animator m_animator = null;
    [AnimatorParam("m_animator")]
    [SerializeField] private int m_showParam = 0;
    [AnimatorParam("m_animator")]
    [SerializeField] private int m_hideParam = 0;
    
    private Camera m_camera = null;
    private RectTransform m_transform = null;
    

    private void Start()
    {
        TryGetComponent(out m_transform);
        m_camera = m_cameraEvent.Cam;
        
        m_positionEvent.Event.AddListener(OnPosition);
        m_showEvent.Event.AddListener(OnShow);
        m_hideEvent.Event.AddListener(OnHide);
    }
    
    private void OnDestroy()
    {
        m_positionEvent.Event.RemoveListener(OnPosition);
        m_showEvent.Event.RemoveListener(OnShow);
        m_hideEvent.Event.RemoveListener(OnHide);
    }
    
    private void OnHide()
    {
        m_animator.SetTrigger(m_hideParam);
    }
    
    private void OnShow(ShowInteractableMarkerData data)
    {
        m_interactMessage.text = data.Text.GetText();
        m_animator.SetTrigger(m_showParam);
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_transform);
    }

    private void OnPosition(PositionUiOnWorldPosData data)
    {
        var pos = m_camera.WorldToViewportPoint(data.Position);
        var rect = m_parentCanvas.rect;

        pos.x *= rect.size.x;
        pos.y *= rect.size.y;

        m_transform.anchoredPosition = pos;
    }
}
