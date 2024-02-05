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
    [SerializeField] private Transform m_startPanel = null;

    private Button[] m_buttons = null;
    
    public void StartGame()
    {
        m_setStartMenuActive.Invoke(false);
        m_startPlayerCam.Invoke();
        m_switchToPlayerInput.Invoke();

        foreach (var button in m_buttons)
        {
            button.interactable = false;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        Application.targetFrameRate = 30;
        m_setStartMenuActive.Invoke(true);
        m_startButton.Select();
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        m_buttons = m_startPanel.GetComponentsInChildren<Button>();
    }
}
