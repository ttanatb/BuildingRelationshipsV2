using UnityEngine;
using UnityEngine.Serialization;
using Utilr.SoGameEvents;

public class SlRemoveOnEvent_Legacy : MonoBehaviour
{
    [SerializeField] private SoGameEvent m_gameEvent = null;

    [FormerlySerializedAs("eventName")]
    [SerializeField]
    string m_eventName = "";

    // Start is called before the first frame update
    private void Start()
    {
        m_gameEvent.Event.AddListener(OnDisableEvent);
    }
    
    private void OnDestroy()
    {
        m_gameEvent.Event.RemoveListener(OnDisableEvent);
    }

    private void OnDisableEvent()
    {
        gameObject.SetActive(false);
    }
}
