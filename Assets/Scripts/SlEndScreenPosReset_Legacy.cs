using UnityEngine;
using UnityEngine.Serialization;
using Utilr.SoGameEvents;

public class SlEndScreenPosReset_Legacy : MonoBehaviour
{
    [SerializeField] private SoGameEvent m_resetPositionEvent = null;
    [FormerlySerializedAs("eventName")]
    [SerializeField]
    private string m_eventName = "resetPlayerPosition";
    
    [FormerlySerializedAs("player")]
    [SerializeField]
    private GameObject m_player = null;

    [FormerlySerializedAs("blackScreen")]
    [SerializeField]
    private GameObject m_blackScreen = null;

    // Start is called before the first frame update
    private void Start()
    {
        m_resetPositionEvent.Event.AddListener(OnResetPosition);
    }

    private void OnDestroy()
    {
        m_resetPositionEvent.Event.RemoveListener(OnResetPosition);
    }

    private void OnResetPosition()
    {
        if (name != m_eventName)
            return;

        m_player.transform.position = transform.position;
        m_blackScreen.SetActive(false);
    }
}
