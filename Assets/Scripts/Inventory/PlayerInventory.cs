using System.Collections;
using System.Collections.Generic;
using GameEvents;
using Input.SO;
using Inventory.SO;
using Sound.Struct;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private SoSetUiActiveStateEvent m_setInventoryUiActiveStateEvent = null;
    [SerializeField] private SwitchInputActionMapEvent m_switchInputToUiEvent = null;
    [SerializeField] private SwitchInputActionMapEvent m_switchInputToGameplayEvent = null;

    [SerializeField] private InputActionReference m_showInventory = null;
    [SerializeField] private InputActionReference m_hideInventory = null;
    [SerializeField] private PlayOneShotRandomAudioClipEvent m_showInventoryAudio = null;
    [SerializeField] private PlayOneShotRandomAudioClipEvent m_hideInventoryAudio = null;


    private UIManager m_uiManager = null;
    private PlayerMovement m_movement = null;
    private bool m_isShowingInv = false;

    private void Start()
    {
        TryGetComponent(out m_movement);

        m_showInventory.action.performed += DisplayInventory;
        m_hideInventory.action.performed += HideInventory;

        m_uiManager = UIManager.Instance;
    }

    private void OnDestroy()
    {
        m_showInventory.action.performed -= DisplayInventory;
        m_hideInventory.action.performed -= HideInventory;
    }

    private void DisplayInventory(InputAction.CallbackContext context)
    {
        if (m_isShowingInv) return;

        m_isShowingInv = true;
        m_setInventoryUiActiveStateEvent.Invoke(true);
        m_uiManager.ToggleInstructions("Inventory");
        m_movement.StopMovement();
        m_switchInputToUiEvent.Invoke();
        m_showInventoryAudio.Invoke();
    }

    private void HideInventory(InputAction.CallbackContext context)
    {
        if (!m_isShowingInv) return;
        
        m_isShowingInv = false;
        m_setInventoryUiActiveStateEvent.Invoke(false);
        m_uiManager.ToggleInstructions("");
        m_movement.SetFrozen(false);
        m_switchInputToGameplayEvent.Invoke();
        m_hideInventoryAudio.Invoke();
    }
}
