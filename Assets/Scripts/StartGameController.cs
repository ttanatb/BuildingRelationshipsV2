using UnityEngine;
using GameEvents;
using Input.SO;
using UnityEngine.UI;
using Utilr.SoGameEvents;

public class StartGameController : MonoBehaviour
{
    [SerializeField] private SoSetUiActiveStateEvent m_setStartMenuActive = null;
    [SerializeField] private SoGameEvent m_startPlayerCam = null;
    [SerializeField] private SwitchInputActionMapEvent m_switchToPlayerInput = null;
    [SerializeField] private Button m_startButton = null;
    
    public void StartGame()
    {
        m_setStartMenuActive.Invoke(false);
        m_startPlayerCam.Invoke();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        m_startButton.interactable = false;
        m_switchToPlayerInput.Invoke();
    }

    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = 30;
        m_setStartMenuActive.Invoke(true);
    }
}
