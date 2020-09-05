using Cinemachine;
using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerFishing : MonoBehaviour
{
    [SerializeField]
    FishingSign m_currFishingSign = null;

    [SerializeField]
    LayerMask m_fishingSignLayerMask = 1 << 12;

    [SerializeField]
    LayerMask m_waterLayerMask = 1 << 4;

    [SerializeField]
    float m_waterMaxDist = 20.0f;

    [SerializeField]
    float m_reticleSpeed = 10.0f;

    CinemachineVirtualCamera m_fishingCamera = null;

    [SerializeField]
    FishingReticle m_fishingReticle = null;

    [SerializeField]
    FishingRod m_fishingRod = null;

    PlayerControls m_playerControls = null;
    PlayerInput m_playerInput = null;
    PlayerMovement m_playerMovement = null;
    EventManager m_eventManager = null;
    FishingController m_fishingController = null;
    UIManager m_uiManager = null;

    enum FishingState
    {
        NotFishing,
        Aiming,
        Waiting,
        Reeling,
        Finished,
    }

    FishingState m_currState = FishingState.NotFishing;
    InMemoryVariableStorage m_variableStorage = null;

    HashSet<CollectibleItem.ItemID> m_caughFish = null;

    int m_currFishingSkillLevel = 0;

    private void UpdateFishingCapability(PlayerSkill skill)
    {
        if (skill.type == PlayerSkill.Type.Fish)
            m_currFishingSkillLevel += skill.level;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_playerControls = GetComponent<PlayerController>().PlayerControls;
        m_eventManager = EventManager.Instance;
        m_playerInput = GetComponent<PlayerInput>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_fishingController = FishingController.Instance;
        m_uiManager = UIManager.Instance;
        m_caughFish = new HashSet<CollectibleItem.ItemID>();
        m_variableStorage = FindObjectOfType<InMemoryVariableStorage>();

        m_playerControls.Player.Interact.started += SwitchToFishing;
        m_playerControls.Fishing.Interact.started += FishingInteract;
        m_playerControls.Fishing.Cancel.started += FishingCancel;
        m_playerControls.Fishing.Vertical.started += AimFishingReticleVert;
        m_playerControls.Fishing.Vertical.canceled += AimFishingReticleVert;
        m_playerControls.Fishing.Horizontal.started += AimFishingReticleHori;
        m_playerControls.Fishing.Horizontal.canceled += AimFishingReticleHori;

        m_playerControls.Fishing.Disable();
        m_fishingReticle.SetActive(false);
        m_fishingRod.SetActive(false);

        m_eventManager.AddFishReelStartListener(FishReelStarted);
        m_eventManager.AddFishReelEndedListener(FishReelEnded);
        m_eventManager.AddSkillUnlockedListener(UpdateFishingCapability);
    }


    private void OnDestroy()
    {
        m_playerControls.Player.Interact.started -= SwitchToFishing;
        m_playerControls.Fishing.Interact.started += FishingInteract;
        m_playerControls.Fishing.Cancel.started -= FishingCancel;
        m_playerControls.Fishing.Vertical.started -= AimFishingReticleVert;
        m_playerControls.Fishing.Vertical.canceled -= AimFishingReticleVert;
        m_playerControls.Fishing.Horizontal.started -= AimFishingReticleHori;
        m_playerControls.Fishing.Horizontal.canceled -= AimFishingReticleHori;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FishReelStarted(FishStats stats, Fish fish)
    {
        if (m_currState != FishingState.Waiting)
            Debug.LogError("Received fish reel started event while in state (" + m_currState + ")");

        m_currFishingSign.FishingArea.ActivateFish(false);
        m_fishingController.StartFishing(stats, fish);

        m_fishingReticle.SetAnimTriggReelStart();
        m_fishingRod.SetAnimTriggReelStart();

        m_uiManager.ToggleInstructions("");
        ChangeState(FishingState.Reeling);
    }

    private void FishReelEnded(bool success, CollectibleItem.ItemID id, Fish fish)
    {
        if (m_currState != FishingState.Reeling)
            Debug.LogError("Received fish reel ended event while in state (" + m_currState + ")");

        if (success)
        {
            m_currFishingSign.FishingArea.RemoveFish(fish);
            m_eventManager.TriggerItemEvent(id, 1);
            m_caughFish.Add(id);
            Debug.Log("caught " + m_caughFish.Count + " different fishes");
            m_variableStorage.SetValue("$fishSpeciesCaught", m_caughFish.Count);
            Debug.Log(m_variableStorage);
        }

        m_fishingReticle.SetAnimTriggReelEnd();
        m_fishingRod.SetAnimTriggReelEnd();
        m_uiManager.ToggleInstructions("Aiming");
        ChangeState(FishingState.Aiming);
    }

    void AimFishingReticleVert(InputAction.CallbackContext context)
    {
        var pointerVel = context.ReadValue<float>();
        //Debug.Log("verti: " + context);
        if (m_currState == FishingState.Aiming)
        {
            m_fishingReticle.SetVelocityY(pointerVel * m_reticleSpeed);
        }
        else if (m_currState == FishingState.Reeling)
        {
            m_fishingController.AdjustPosVert(pointerVel);
        }
    }

    void AimFishingReticleHori(InputAction.CallbackContext context)
    {
        var pointerVel = context.ReadValue<float>();
        //Debug.Log("hori: " + context);
        if (m_currState == FishingState.Aiming)
        {
            m_fishingReticle.SetVelocityX(pointerVel * m_reticleSpeed);

        }
        else if (m_currState == FishingState.Reeling)
        {
            m_fishingController.AdjustPosHori(pointerVel);
        }

    }

    void SwitchToFishing(InputAction.CallbackContext context)
    {
        if (m_currFishingSign == null) return;

        m_currFishingSign.SetInteractable(false);
        m_playerControls.Player.Disable();
        m_playerInput.SwitchCurrentActionMap("Fishing");
        m_playerControls.Fishing.Enable();

        m_playerMovement.StopMovement();
        m_playerMovement.SetPosition(m_currFishingSign.PlayerAnchor);

        m_fishingReticle.SetActive(true);
        m_fishingRod.SetActive(true);
        m_fishingRod.FishingAreaTransform = m_currFishingSign.FishingArea.transform;
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
                m_playerControls.Fishing.Disable();
                m_playerInput.SwitchCurrentActionMap("Player");
                m_playerControls.Player.Enable();

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
