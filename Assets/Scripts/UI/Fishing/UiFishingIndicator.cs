using Fishing.SO;
using Fishing.Structs;
using GameEvents.Fishing;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct ArrowAnimator
{
    [field: SerializeField]
    public Animator Animator { get; set; }
    
    [field: SerializeField]
    [field: AnimatorParam("Animator")]
    public int ActiveParam { get; set; }

    public void SetActive(bool active)
    {
        Animator.SetBool(ActiveParam, active);
    }
}

public class UiFishingIndicator : MonoBehaviour
{
    [SerializeField] [Expandable]
    private FishingConfig m_config = null;

    [SerializeField] private InputActionReference m_fishingAim = null;
    [SerializeField] private ArrowAnimator m_upArrow =  new ArrowAnimator();
    [SerializeField] private ArrowAnimator m_leftArrow =  new ArrowAnimator();
    [SerializeField] private ArrowAnimator m_rightArrow =  new ArrowAnimator();
    [SerializeField] private ArrowAnimator m_downArrow =  new ArrowAnimator();
    [SerializeField] private Animator m_animator = null;
    [SerializeField] [AnimatorParam("m_animator")]
    private int m_fishProgressActive = 0;
    
    [SerializeField] private UpdateFishProgressEvent m_fishProgressEvent = null;
    [SerializeField] private StartFishReelEvent m_startFishReelEvent = null;
    [SerializeField] private EndFishReelEvent m_endFishReelEvent = null;

    private float m_prevProgress = 0.0f;

    // Start is called before the first frame update
    private void Start()
    {
        m_fishingAim.action.performed += AimFishingReticle;
        m_fishingAim.action.canceled += AimFishingReticle;
        m_fishProgressEvent.Event.AddListener(OnFishProgress);
        m_startFishReelEvent.Event.AddListener(OnFishReelStart);
        m_endFishReelEvent.Event.AddListener(OnFishReelEnd);
    }
    
    private void OnDestroy()
    {
        m_fishingAim.action.performed -= AimFishingReticle;
        m_fishingAim.action.canceled -= AimFishingReticle;
        m_fishProgressEvent.Event.RemoveListener(OnFishProgress);
        m_startFishReelEvent.Event.RemoveListener(OnFishReelStart);
        m_endFishReelEvent.Event.RemoveListener(OnFishReelEnd);
    }
    
    private void OnFishReelEnd(FishReelEndData _)
    {
        m_animator.SetBool(m_fishProgressActive, false);
    }

    private void OnFishProgress(float progress)
    {
        if (Mathf.Abs(progress - m_prevProgress) < float.Epsilon) return;

        m_animator.SetBool(m_fishProgressActive, progress > m_prevProgress);
        m_prevProgress = progress;
    }

    private void OnFishReelStart(FishReelStartData _)
    {
        m_prevProgress = m_config.StartCompletionRatio;
    }

    private void AimFishingReticle(InputAction.CallbackContext ctx)
    {
        var pointerVel = ctx.ReadValue<Vector2>();
        // Debug.Log($"Move input triggered with: {pointerVel}, performed {ctx.performed}, canceled {ctx.canceled}");

        m_upArrow.SetActive(pointerVel.y > float.Epsilon);
        m_downArrow.SetActive(pointerVel.y < -float.Epsilon);
        m_rightArrow.SetActive(pointerVel.x > float.Epsilon);
        m_leftArrow.SetActive(pointerVel.x < -float.Epsilon);
    }
}
