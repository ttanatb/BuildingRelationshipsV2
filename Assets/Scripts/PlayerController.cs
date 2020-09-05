using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public PlayerControls PlayerControls { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        PlayerControls = new PlayerControls();
        PlayerControls.Fishing.Disable();
        PlayerControls.UI.Disable();
    }

    private void Start()
    {
        PlayerInput m_playerInput = GetComponent<PlayerInput>();

        Debug.AssertFormat(
                m_playerInput.notificationBehavior == PlayerNotifications.InvokeCSharpEvents,
                "Expected player input to use C Sharp events, instead is using {0}",
                m_playerInput.notificationBehavior);
    }

    private void OnEnable()
    {
        PlayerControls.Enable();
    }

    private void OnDisable()
    {
        PlayerControls.Disable();
    }

}
