using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{

    PlayerControls m_playerControls = null;
    UIManager m_uiManager = null;
    PlayerMovement m_movement = null;
    PlayerInput m_playerInput = null;

    bool isShowingInv = false;

    // Start is called before the first frame update
    void Start()
    {
        m_playerControls = GetComponent<PlayerController>().PlayerControls;
        m_movement = GetComponent<PlayerMovement>();
        m_playerInput = GetComponent<PlayerInput>();

        m_playerControls.Player.ShowInventory.started += DisplayInventory;
        m_playerControls.UI.Cancel.started += HideInventory;

        m_uiManager = UIManager.Instance;
    }

    private void OnDestroy()
    {
        m_playerControls.Player.ShowInventory.started -= DisplayInventory;
        m_playerControls.UI.Cancel.started -= HideInventory;

    }

    void DisplayInventory(InputAction.CallbackContext context)
    {
        isShowingInv = true;
        m_uiManager.SetInventoryActive(true);
        m_uiManager.ToggleInstructions("Inventory");
        m_movement.StopMovement();
        m_playerControls.Player.Disable();
        m_playerInput.SwitchCurrentActionMap("UI");
        m_playerControls.UI.Enable();

    }

    void HideInventory(InputAction.CallbackContext context)
    {
        if (!isShowingInv) return;
        isShowingInv = false;
        m_uiManager.SetInventoryActive(false);
        m_uiManager.ToggleInstructions("");
        m_movement.SetFrozen(false);
        m_playerControls.UI.Disable();
        m_playerInput.SwitchCurrentActionMap("UI");
        m_playerControls.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
