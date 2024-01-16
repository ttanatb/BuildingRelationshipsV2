using System;
using GameEvents;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class UiBase : MonoBehaviour
    {
        [FormerlySerializedAs("m_toggleUiEvent")]
        [SerializeField]
        protected SoSetUiActiveStateEvent m_setUiActiveStateEvent = null;

        [SerializeField]
        protected CanvasGroup m_canvasGroup = null;
        
        protected virtual void Start()
        {
            m_setUiActiveStateEvent.Event.AddListener(SetActiveState);
        }

        protected virtual void SetActiveState(bool isActive)
        {
            Debug.Log($"{gameObject.name} is being active state to {isActive}");
            m_canvasGroup.alpha = isActive ? 1.0f : 0.0f;
        }
    }
}
