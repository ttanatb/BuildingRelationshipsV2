using Cinemachine;
using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using Fishing.SO;
using Fishing.Structs;
using GameEvents;
using Input.SO;
using Inventory.SO;
using Inventory.Structs;
using NaughtyAttributes;
using Skills.SO;
using Skills.Structs;
using UnityEditor;
using UnityEngine.InputSystem;
using Utilr.SoGameEvents;

public class PlayerFishing : MonoBehaviour
{
    [SerializeField] private FishingSign m_currFishingSign = null;
    [SerializeField] private LayerMask m_fishingSignLayerMask = 1 << 12;
    [SerializeField] private LayerMask m_waterLayerMask = 1 << 4;
    [SerializeField] private float m_waterMaxDist = 20.0f;
    [SerializeField] private float m_reticleSpeed = 10.0f;
    [SerializeField] private ObtainItemEvent m_obtainItemEvent = null;

    private CinemachineVirtualCamera m_fishingCamera = null;

    [SerializeField] private FishingReticle m_fishingReticle = null;
    [SerializeField] private FishingRod m_fishingRod = null;

    private PlayerMovement m_playerMovement = null;
    private FishingController m_fishingController = null;
    private UIManager m_uiManager = null;

    [SerializeField] private InputActionReference m_playerInteract = null;
    [SerializeField] private InputActionReference m_fishingInteract = null;
    [SerializeField] private InputActionReference m_fishingCancel = null;
    [SerializeField] private InputActionReference m_fishingAim = null;

    [SerializeField] private SwitchInputActionMapEvent m_switchToFishing = null;
    [SerializeField] private SwitchInputActionMapEvent m_switchToPlayer = null;

    [SerializeField] private FishReelStartEvent m_fishReelStartEvent = null;
    [SerializeField] private FishReelEndEvent m_fishReelEndEvent = null;

    [SerializeField] private SetSkillLevelEvent m_setSkillLevelEvent = null;

    private enum FishingState
    {
        NotFishing,
        Aiming,
        Waiting,
        Reeling,
        Finished,
    }

    private FishingState m_currState = FishingState.NotFishing;
    private InMemoryVariableStorage m_variableStorage = null;
    private HashSet<ItemData.ItemID> m_caughtFish = null;
    private int m_currFishingSkillLevel = 1;

    private void OnUpdateFishingCapability(SkillTypeAndLevel data)
    {
        if (data.SkillType == PlayerSkill.SkillType.Fish)
            m_currFishingSkillLevel = data.Level;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_fishingController = FishingController.Instance;
        m_uiManager = UIManager.Instance;
        m_caughtFish = new HashSet<ItemData.ItemID>();
        m_variableStorage = FindObjectOfType<InMemoryVariableStorage>();

        m_playerInteract.action.performed += SwitchToFishing;
        m_fishingInteract.action.performed += FishingInteract;
        m_fishingCancel.action.performed += FishingCancel;
        m_fishingAim.action.performed += AimFishingReticle;
        m_fishingAim.action.canceled += AimFishingReticle;

        m_fishingReticle.SetActive(false);
        m_fishingRod.SetActive(false);

        m_fishReelStartEvent.Event.AddListener(FishReelStarted);
        m_fishReelEndEvent.Event.AddListener(FishReelEnded);
        
        m_setSkillLevelEvent.Event.AddListener(OnUpdateFishingCapability);
    }


    private void OnDestroy()
    {
        m_playerInteract.action.performed -= SwitchToFishing;
        m_fishingInteract.action.performed -= FishingInteract;
        m_fishingCancel.action.performed -= FishingCancel;
        m_fishingAim.action.performed -= AimFishingReticle;
        m_fishingAim.action.canceled -= AimFishingReticle;
        
        m_fishReelStartEvent.Event.RemoveListener(FishReelStarted);
        m_fishReelEndEvent.Event.RemoveListener(FishReelEnded);
        
        m_setSkillLevelEvent.Event.RemoveListener(OnUpdateFishingCapability);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FishReelStarted(FishReelStartData data)
    {
        if (m_currState != FishingState.Waiting)
            Debug.LogError("Received fish reel started event while in state (" + m_currState + ")");

        m_currFishingSign.FishingArea.ActivateFish(false);
        m_fishingController.StartFishing(data.FishData, data.Fish);

        m_fishingReticle.SetAnimTriggReelStart();
        m_fishingRod.SetAnimTriggReelStart();

        m_uiManager.ToggleInstructions("");
        ChangeState(FishingState.Reeling);
    }

    private void FishReelEnded(FishReelEndData data)
    {
        if (m_currState != FishingState.Reeling)
            Debug.LogError("Received fish reel ended event while in state (" + m_currState + ")");

        if (data.Success)
        {
            m_currFishingSign.FishingArea.RemoveFish(data.Fish);
            m_obtainItemEvent.Invoke(new ItemCount(){Id = data.Id, Count = 1});
            m_caughtFish.Add(data.Id);
            Debug.Log("caught " + m_caughtFish.Count + " different fishes");
            m_variableStorage.SetValue("$fishSpeciesCaught", m_caughtFish.Count);
            Debug.Log(m_variableStorage);
        }

        m_fishingReticle.SetAnimTriggReelEnd();
        m_fishingRod.SetAnimTriggReelEnd();
        m_uiManager.ToggleInstructions("Aiming");
        ChangeState(FishingState.Aiming);
    }

    void AimFishingReticle(InputAction.CallbackContext context)
    {
        var pointerVel = context.ReadValue<Vector2>();
        Debug.Log($"Move input triggered with: {pointerVel}, performed {context.performed}, canceled {context.canceled}");

        if (m_currState == FishingState.Aiming)
        {
            m_fishingReticle.SetVelocityY(pointerVel.y * m_reticleSpeed);
            m_fishingReticle.SetVelocityX(pointerVel.x * m_reticleSpeed);
        }
        else if (m_currState == FishingState.Reeling)
        {
            m_fishingController.AdjustPosVertical(pointerVel.y);
            m_fishingController.AdjustPosHorizontal(pointerVel.x);
        }
    }

    void SwitchToFishing(InputAction.CallbackContext context)
    {
        if (m_currFishingSign == null) return;

        m_currFishingSign.SetInteractable(false);
        m_switchToFishing.Invoke();

        m_playerMovement.StopMovement();
        m_playerMovement.SetPosition(m_currFishingSign.PlayerAnchor);

        m_fishingReticle.SetActive(true);
        m_fishingRod.SetActive(true);
        m_fishingRod.FishingAreaTransform = m_currFishingSign.FishingArea.transform;
        m_currFishingSign.ActivateCamEvent.Invoke();
        var fishingCam = m_currFishingSign.FishingCamera;
        Ray ray = new Ray(fishingCam.position, fishingCam.forward);

        if (Physics.Raycast(ray, out RaycastHit hitinfo, m_waterMaxDist, m_waterLayerMask))
        {
            m_fishingReticle.SetPosition(hitinfo.point + Vector3.up * 0.1f);
        }
        m_fishingCamera.LookAt = m_fishingReticle.transform;

        m_uiManager.ToggleInstructions("Aiming");

        ChangeState(FishingState.Aiming);
    }

    void FishingInteract(InputAction.CallbackContext context)
    {

        switch (m_currState)
        {
            // Cast fishing rod.
            case FishingState.Aiming:
                m_currFishingSign.FishingArea.ActivateFish(true);
                m_fishingReticle.SetVelocityX(0.0f);
                m_fishingReticle.SetVelocityY(0.0f);
                m_fishingReticle.SetAnimTriggCastStart();
                m_fishingRod.SetAnimTriggCastStart();
                m_uiManager.ToggleInstructions("Casted");
                ChangeState(FishingState.Waiting);
                break;
        }
    }

    void FishingCancel(InputAction.CallbackContext context)
    {
        switch (m_currState)
        {
            // cancel out fishing entirely
            case FishingState.Aiming:
                if (m_currFishingSign != null)
                {
                    m_currFishingSign.SetInteractable(true);
                    m_currFishingSign.ResetLookat();
                }

                m_playerMovement.SetFrozen(false);
                m_fishingReticle.SetActive(false);
                m_fishingRod.SetActive(false);
                m_switchToPlayer.Invoke();

                m_uiManager.ToggleInstructions("");
                ChangeState(FishingState.NotFishing);
                break;
            // recast
            case FishingState.Waiting:
                m_currFishingSign.FishingArea.ActivateFish(false);
                m_fishingReticle.SetAnimTriggCastEnd();
                m_fishingRod.SetAnimTriggCastEnd();
                m_uiManager.ToggleInstructions("Aiming");
                ChangeState(FishingState.Aiming);
                break;
        }
    }

    private void ChangeState(FishingState state)
    {
        m_currState = state;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_currFishingSkillLevel == 0) return;
        if (m_fishingSignLayerMask == (m_fishingSignLayerMask | (1 << other.gameObject.layer)))
        {
            FishingSign sign = other.GetComponent<FishingSign>();
            if (sign == null)
            {
                Debug.LogError("Entered trigger of chracter who is not fishing sign.");
                return;
            }

            if (m_currFishingSign != null)
            {
                Debug.LogWarning("Overriding current fishing sign to talk to.");
                m_currFishingSign.SetInteractable(false);
            }

            // Check NodeExists
            if (!sign.Interactible)
            {
                m_fishingCamera = null;
                m_currFishingSign = null;
                return;
            }

            m_currFishingSign = sign;
            m_fishingCamera = m_currFishingSign.FishingCamera.GetComponent<CinemachineVirtualCamera>();
            m_currFishingSign.SetInteractable(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_currFishingSkillLevel == 0) return;
        if (m_fishingSignLayerMask == (m_fishingSignLayerMask | (1 << other.gameObject.layer)))
        {
            FishingSign sign = other.GetComponent<FishingSign>();
            if (sign == null)
            {
                Debug.LogError("Exited trigger of chracter who is not fish sign.");
                return;
            }

            if (m_currFishingSign != sign)
            {
                Debug.LogWarning("Exiting fish sign range that you weren't in range of.");
                sign.SetInteractable(false);
            }

            if (m_currFishingSign == null)
                return;

            m_currFishingSign.SetInteractable(false);
            m_fishingCamera = null;
            m_currFishingSign = null;
        }
    }
}
