using UnityEngine;
using UnityEngine.Serialization;
using Utilr.SoGameEvents;

public class SlEnableOnEvent_Legacy : MonoBehaviour
{
    [SerializeField] private SoGameEvent m_gameEvent = null;
    
    [FormerlySerializedAs("eventName")]
    [SerializeField]
    private string m_eventName = "";
    
    [FormerlySerializedAs("gameObj")]
    [SerializeField]
    private GameObject m_gameObj = null;

    // Start is called before the first frame update
    private void Start()
    {
        m_gameEvent.Event.AddListener(OnEnableEvent);
        Invoke(nameof(DisableGameObject), 1);
    }

    private void OnDestroy()
    {
        m_gameEvent.Event.RemoveListener(OnEnableEvent);
    }

    private void DisableGameObject()
    {
        m_gameObj.SetActive(false);
    }

    private void OnEnableEvent()
    {

        m_gameObj.SetActive(true);
    }
}
