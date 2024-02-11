using Cinemachine;
using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using Cam.Events;
using Dialogue.SO;
using Fishing.SO;
using Fishing.Structs;
using GameEvents;
using Input.SO;
using Inventory.SO;
using Inventory.Structs;
using NaughtyAttributes;
using Skills.SO;
using Skills.Structs;
using Sound.SO;
using UI.Events;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utilr.SoGameEvents;
using Utilr.Utility;

public class PlayerFishing : MonoBehaviour
{
    [SerializeField] private FishingSign m_currFishingSign = null;
    [SerializeField] private LayerMask m_fishingSignLayerMask = 1 << 12;
    [SerializeField] private float m_reticleSpeed = 10.0f;
    [SerializeField] private ObtainItemEvent m_obtainItemEvent = null;

    [SerializeField] private FishingReticle m_fishingReticle = null;
    [SerializeField] private FishingRod m_fishingRod = null;

    private PlayerMovement m_playerMovement = null;
    private FishingController m_fishingController = null;

    [SerializeField] private InputActionReference m_playerInteract = null;
    [SerializeField] private InputActionReference m_fishingInteract = null;
    [SerializeField] private InputActionReference m_fishingCancel = null;
    [SerializeField] private InputActionReference m_fishingAim = null;

    [SerializeField] private SwitchInputActionMapEvent m_switchToFishing = null;
    [SerializeField] private SwitchInputActionMapEvent m_switchToPlayer = null;

    [FormerlySerializedAs("m_fishReelStartEvent")] 
    [SerializeField] private StartFishReelEvent m_startFishReelEvent = null;
    [FormerlySerializedAs("m_fishReelEndEvent")] 
    [SerializeField] private EndFishReelEvent m_endFishReelEvent = null;

    [SerializeField] private SetSkillLevelEvent m_setSkillLevelEvent = null;

    [SerializeField] private SoGameEvent m_reticleCloseUp = null;
    [SerializeField] private SoGameEvent m_playerCamera = null;
    
    [SerializeField] private float m_reelStartDelay = 0.7f;
    [SerializeField] private SetCamTransitionEvent m_fastTransition = null;
    [SerializeField] private SetCamTransitionEvent m_slowTransition = null;
    [SerializeField] private SetCamTransitionEvent m_defaultTransition = null;

    [SerializeField] private ActorAnimationEvent m_startPlayerBounce = null;
    [SerializeField] private ShowBottomUiEvent m_showAimControls = null;
    [SerializeField] private ShowBottomUiEvent m_showCastedControls = null;
    [SerializeField] private SoGameEvent m_hideControls = null;

    [SerializeField] private PlayOneShotRandomAudioClipEvent m_fishReelStartSound = null;
    [SerializeField] private PlayLoopingAudioEvent m_fishReelLoopSound = null;
    [SerializeField] private PlayLoopingAudioEvent m_splashLoopLowSound = null;
    [SerializeField] private PlayLoopingAudioEvent m_splashLoopHighSound = null;
    [SerializeField] private PlayLoopingAudioEvent m_bgmLoop = null;

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
        m_caughtFish = new HashSet<ItemData.ItemID>();
        m_variableStorage = FindObjectOfType<InMemoryVariableStorage>();

        m_playerInteract.action.performed += SwitchToFishing;
        m_fishingInteract.action.performed += FishingInteract;
        m_fishingCancel.action.performed += FishingCancel;
        m_fishingAim.action.performed += AimFishingReticle;
        m_fishingAim.action.canceled += AimFishingReticle;

        m_fishingReticle.SetActive(false);
        m_fishingRod.SetActive(false);

        m_startFishReelEvent.Event.AddListener(FishReelStarted);
        m_endFishReelEvent.Event.AddListener(FishReelEnded);
        
        m_setSkillLevelEvent.Event.AddListener(OnUpdateFishingCapability);
    }


    private void OnDestroy()
    {
        m_playerInteract.action.performed -= SwitchToFishing;
        m_fishingInteract.action.performed -= FishingInteract;
        m_fishingCancel.action.performed -= FishingCancel;
        m_fishingAim.action.performed -= AimFishingReticle;
        m_fishingAim.action.canceled -= AimFishingReticle;
        
        m_startFishReelEvent.Event.RemoveListener(FishReelStarted);
        m_endFishReelEvent.Event.RemoveListener(FishReelEnded);
        
        m_setSkillLevelEvent.Event.RemoveListener(OnUpdateFishingCapability);
    }

    private void FishReelStarted(FishReelStartData data)
    {
        if (m_currState != FishingState.Waiting)
            Debug.LogError("Received fish reel started event while in state (" + m_currState + ")");

        m_currFishingSign.FishingArea.ActivateFish(false);
        m_fastTransition.Invoke();
        m_reticleCloseUp.Invoke();
        m_fishingReticle.Freeze();
        m_hideControls.Invoke();
        m_fishReelStartSound.Invoke();
        ChangeState(FishingState.Reeling);

        StartCoroutine(Helper.ExecuteAfter(() => {
            data.Fish.FishAnimator.StartReelingAnim();
            m_fishReelLoopSound.Invoke();
            m_splashLoopLowSound.Invoke();
            m_splashLoopHighSound.Invoke();
            
            m_fishingController.StartFishing(data.FishData, data.Fish);

            m_fishingReticle.AnimateStartReel();
            m_fishingRod.SetAnimTriggReelStart();

            m_slowTransition.Invoke();
            m_currFishingSign.ReelCamEvent.Invoke();
        }, m_reelStartDelay));
    }

    private void FishReelEnded(FishReelEndData data)
    {
        if (m_currState != FishingState.Reeling)
            Debug.LogError("Received fish reel ended event while in state (" + m_currState + ")");

        if (data.Success)
        {
            FishingArea.RemoveFish(data.Fish);
            m_obtainItemEvent.Invoke(new ItemCount(){Id = data.Id, Count = 1});
            m_caughtFish.Add(data.Id);
            Debug.Log("caught " + m_caughtFish.Count + " different fishes");
            m_variableStorage.SetValue("$fishSpeciesCaught", m_caughtFish.Count);
            Debug.Log(m_variableStorage);
            m_fishReelStartSound.Invoke();
        }
        else
        {
            data.Fish.FishAnimator.StopReelingAnim();
        }

        m_fishingReticle.AnimateStopReel();
        m_fishingRod.SetAnimTriggReelEnd();
        m_defaultTransition.Invoke();
        m_currFishingSign.AimCamEvent.Invoke();
        m_startPlayerBounce.Invoke();
        
        m_fishReelLoopSound.InvokeStop();
        m_splashLoopLowSound.InvokeStop();
        m_splashLoopHighSound.InvokeStop();
        
        m_showAimControls.Invoke();
        ChangeState(FishingState.Aiming);
    }

    private void AimFishingReticle(InputAction.CallbackContext context)
    {
        var pointerVel = context.ReadValue<Vector2>();
        Debug.Log($"Move input triggered with: {pointerVel}, performed {context.performed}, canceled {context.canceled}");

        if (m_currState == FishingState.Aiming)
        {
            m_fishingReticle.SetAccY(pointerVel.y * m_reticleSpeed);
            m_fishingReticle.SetAccX(pointerVel.x * m_reticleSpeed);
        }
        else if (m_currState == FishingState.Reeling)
        {
            m_fishingController.AdjustPosVertical(pointerVel.y);
            m_fishingController.AdjustPosHorizontal(pointerVel.x);
        }
    }

    private void SwitchToFishing(InputAction.CallbackContext context)
    {
        if (m_currFishingSign == null) return;

        m_currFishingSign.SetInteractable(false);
        m_switchToFishing.Invoke();

        m_playerMovement.StopMovement();
        m_playerMovement.SetPosition(m_currFishingSign.PlayerAnchor);

        m_fishingReticle.SetActive(true);
        m_fishingRod.SetActive(true);
        m_fishingRod.FishingAreaTransform = m_currFishingSign.FishingArea.transform;
        
        m_currFishingSign.AimCamEvent.Invoke();
        m_currFishingSign.ResetReticlePosition();
        
        m_showAimControls.Invoke();
        m_bgmLoop.InvokeStop();

        ChangeState(FishingState.Aiming);
    }

    private void FishingInteract(InputAction.CallbackContext context)
    {

        switch (m_currState)
        {
            // Cast fishing rod.
            case FishingState.Aiming:
                m_currFishingSign.FishingArea.ActivateFish(true);
                m_fishingReticle.SetAccX(0.0f);
                m_fishingReticle.SetAccY(0.0f);
                m_fishingReticle.AnimateStartCast();
                m_fishingRod.SetAnimTriggCastStart();
                m_showCastedControls.Invoke();
                ChangeState(FishingState.Waiting);
                break;
        }
    }

    private void FishingCancel(InputAction.CallbackContext context)
    {
        switch (m_currState)
        {
            // cancel out fishing entirely
            case FishingState.Aiming:
                if (m_currFishingSign != null)
                {
                    m_currFishingSign.SetInteractable(true);
                }

                m_playerMovement.SetFrozen(false);
                m_fishingReticle.SetActive(false);
                m_fishingRod.SetActive(false);
                m_switchToPlayer.Invoke();
                m_playerCamera.Invoke();

                m_hideControls.Invoke();
                m_bgmLoop.Invoke();
                ChangeState(FishingState.NotFishing);
                break;
            // recast
            case FishingState.Waiting:
                m_currFishingSign.FishingArea.ActivateFish(false);
                m_fishingReticle.AnimateStopCast();
                m_fishingRod.SetAnimTriggCastEnd();
                m_showAimControls.Invoke();
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
        if (m_fishingSignLayerMask != (m_fishingSignLayerMask | (1 << other.gameObject.layer)))
            return;
        
        var sign = other.GetComponent<FishingSign>();
        if (sign == null)
        {
            Debug.LogError("Entered trigger of character who is not fishing sign.");
            return;
        }

        if (m_currFishingSign != null)
        {
            Debug.LogWarning("Overriding current fishing sign to talk to.");
            m_currFishingSign.SetInteractable(false);
        }

        // Check NodeExists
        // if (!sign.Interactable)
        // {
        //     m_fishingCamera = null;
        //     m_currFishingSign = null;
        //     return;
        // }

        m_currFishingSign = sign;
        m_currFishingSign.SetInteractable(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_currFishingSkillLevel == 0) return;
        if (m_fishingSignLayerMask != (m_fishingSignLayerMask | (1 << other.gameObject.layer)))
            return;
        
        var sign = other.GetComponent<FishingSign>();
        if (sign == null)
        {
            Debug.LogError("Exited trigger of character who is not fish sign.");
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
        m_currFishingSign = null;
    }
}
