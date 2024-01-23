using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UiSelectableWithAudio : MonoBehaviour, ISelectHandler, ISubmitHandler
    {
        [SerializeField] private PlayOneShotRandomAudioClipEvent m_buttonSelectedAudio = null;
        [SerializeField] private PlayOneShotRandomAudioClipEvent m_buttonPressedAudio = null;

        public void OnSelect(BaseEventData eventData)
        {
            m_buttonSelectedAudio.Invoke();
        }
        
        public void OnSubmit(BaseEventData eventData)
        {
            m_buttonPressedAudio.Invoke();
        }
    }
}
