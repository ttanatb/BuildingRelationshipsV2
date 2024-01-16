using Cinemachine;
using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using GameEvents;
using Input.SO;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using Utilr.SoGameEvents;

public class PlayerFishing : MonoBehaviour
{
    [SerializeField] private FishingSign m_currFishingSign = null;

    [SerializeField] private LayerMask m_fishingSignLayerMask = 1 << 12;
    [SerializeField] private LayerMask m_waterLayerMask = 1 << 4;

    [SerializeField] private float m_waterMaxDist = 20.0f;

    [SerializeField] private float m_reticleSpeed = 10.0f;

    private CinemachineVirtualCamera m_fishingCamera = null;

    [SerializeField] private FishingReticle m_fishingReticle = null;

    [SerializeField] private FishingRod m_fishingRod = null;

    private PlayerMovement m_playerMovement = null;
    private EventManager m_eventManager = null;
    private FishingController m_fishingController = null;
    private UIManager m_uiManager = null;

    [SerializeField] private InputActionReference m_playerInteract = null;
    [SerializeField] private InputActionReference m_fishingInteract = null;
    [SerializeField] private InputActionReference m_fishingCancel = null;
    [SerializeField] private InputActionReference m_fishingAim = null;

    [SerializeField] private SwitchInputActionMapEvent m_switchToFishing = null;
    [SerializeField] private SwitchInputActionMapEvent m_switchToPlayer = null;
    
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

    int m_currFishingSkillLevel = 1;

    private void UpdateFishingCapability(PlayerSkill skill)
    {
        if (skill.type == PlayerSkill.Type.Fish)
            m_currFishingSkillLevel += skill.level;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_eventManager = EventManager.Instance;
        m_playerMovement = GetComponent<PlayerMovement>();
        m_fishingController = FishingController.Instance;
        m_uiManager = UIManager.Instance;
        m_caughFish = new HashSet<CollectibleItem.ItemID>();
        m_variableStorage = FindObjectOfType<InMemoryVariableStorage>();

        m_playerInteract.action.performed += SwitchToFishing;
        m_fishingInteract.action.performed += FishingInteract;
        m_fishingCancel.action.performed += FishingCancel;
        m_fishingAim.action.performed += AimFishingReticle;
        m_fishingAim.action.canceled += AimFishingReticle;

        m_fishingReticle.SetActive(false);
        m_fishingRod.SetActive(false);

        m_eventManager.AddFishReelStartListener(FishReelStarted);
        m_eventManager.AddFishReelEndedListener(FishReelEnded);
        m_eventManager.AddSkillUnlockedListener(UpdateFishingCapability);
    }


    private void OnDestroy()
    {
        m_playerInteract.action.performed -= SwitchToFishing;
        m_fishingInteract.action.performed -= FishingInteract;
        m_fishingCancel.action.performed -= FishingCancel;
        m_fishingAim.action.performed -= AimFishingReticle;
        m_fishingAim.action.canceled -= AimFishingReticle;
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
            m_fishingController.AdjustPosVert(pointerVel.y);
            m_fishingController.AdjustPosHori(pointerVel.x);
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
