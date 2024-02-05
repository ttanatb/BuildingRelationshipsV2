using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using Dialogue.SO;
using Dialogue.Struct;
using Inventory.SO;
using Inventory.Structs;
using Skills.SO;
using Skills.Structs;
using UnityEngine.Serialization;
using Utilr.Attributes;
using Random = UnityEngine.Random;

public class PlayerMovement : AutonomousAgent
{
    [SerializeField] private Transform m_cameraTransform = null;
    [SerializeField] private TrailRenderer[] m_trailRenderers = null;
    [SerializeField] private Vector2 m_accToApplyFromInput = new Vector2(0.5f, 2.0f);
    [SerializeField] private float m_multiplierSpeedImpulse = 500.0f;
    [SerializeField] private float m_multiplierSpeedJumpImpulse = 100.0f;
    [SerializeField] private float m_currForwardPushForce = 0.0f;
    [SerializeField] private float m_currRightPushForce = 0.0f;
    [SerializeField] private float m_dashSpeedIncrement = 50.0f;
    [SerializeField] private float m_dashDuration = 2.0f;
    private float m_dashTimer = 0.0f;

    [SerializeField] private float m_jumpSpeed = 50.0f;

    [SerializeField] private float m_jumpRollModifier = 10.0f;
    [SerializeField] private float m_jumpSpinRate = 0.20f;
    [SerializeField] private float m_jumpSpinModifier = 500.0f;

    [SerializeField] private PlayJumpSoundEvent m_playJumpSoundEvent = null;
    [SerializeField] private PlayOneShotRandomAudioClipEvent m_dashAudioEvent = null;

    [SerializeField] private float m_noRollJumpSpeed = 50.0f;
    [SerializeField] private InputActionReference m_dashInput = null;
    [SerializeField] private InputActionReference m_jumpInput = null;
    [SerializeField] private InputActionReference m_freezePosInput = null;
    [SerializeField] private InputActionReference m_moveInput = null;

    [SerializeField] private CinemachineVirtualCamera m_cinematicCam = null;

    [SerializeField] private SetSkillLevelEvent m_setSkillLevelEvent = null;


    private Vector3 m_totalMovementForce = Vector3.zero;

    [SerializeField] private int m_jumpCount = 0;
    private int m_baseJumpCount = 0;

    private float m_dashCooldownTimer = 0;
    private float m_baseDashTimer = 0;
    private float m_jumpModifier = 0;

    private static readonly int[] JUMP_COUNT = { 0, 1, 2, 3, int.MaxValue };
    private static readonly float[] JUMP_DIST_MODIFIER = { 0.7f, 1.0f, 1.3f };
    private static readonly float[] DASH_TIMER = { float.PositiveInfinity, 2.0f, 1.0f, 0.0f };
    private static readonly RigidbodyConstraints[] CONSTRAINTS = {
        RigidbodyConstraints.None,
        RigidbodyConstraints.None,
    };

    private int m_currJumpIndex = 0;
    private int m_currJumpDistIndex = 0;
    private int m_currDashTimerIndex = 0;
    private int m_currConstraintIndex = 0;

    [SerializeField] private RigidbodyConstraints m_baseConstraints = RigidbodyConstraints.FreezeRotationZ;

    [SerializeField] private LayerMask m_jumpRegainLayerMask = 1;
    
    [SerializeField] private StopDialogueEvent m_stopDialogueEvent = null;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        m_baseJumpCount = JUMP_COUNT[m_currJumpIndex];
        m_baseConstraints = CONSTRAINTS[m_currConstraintIndex];
        m_jumpModifier = JUMP_DIST_MODIFIER[m_currJumpDistIndex];
        m_baseDashTimer = DASH_TIMER[m_currDashTimerIndex];


        m_rigidbody.constraints = m_baseConstraints;
        m_dashCooldownTimer = m_baseDashTimer;
        m_jumpCount = m_baseJumpCount;


        if (m_cameraTransform == null)
            m_cameraTransform = Camera.main.transform;

        m_dashInput.action.performed += Dash;
        m_jumpInput.action.performed += Jump;
        m_freezePosInput.action.performed += FreezePosition;
        m_moveInput.action.started += StartMove;
        m_moveInput.action.performed += Move;
        m_moveInput.action.canceled += Move;
        
        m_setSkillLevelEvent.Event.AddListener(UpdateSkillLevel);
        m_stopDialogueEvent.Event.AddListener(EnableMovement);
    }

    private void OnDestroy()
    {
        m_dashInput.action.performed -= Dash;
        m_jumpInput.action.performed -= Jump;
        m_freezePosInput.action.performed -= FreezePosition;
        m_moveInput.action.started -= StartMove;
        m_moveInput.action.performed -= Move;
        m_moveInput.action.canceled -= Move;

        m_stopDialogueEvent.Event.RemoveListener(EnableMovement);
        m_setSkillLevelEvent.Event.RemoveListener(UpdateSkillLevel);
    }

    private void UpdateSkillLevel(SkillTypeAndLevel skill)
    {
        switch (skill.SkillType)
        {
            case PlayerSkill.SkillType.JumpCount:
                m_currJumpIndex += skill.Level;
                m_currJumpIndex = Mathf.Min(JUMP_COUNT.Length - 1, m_currJumpIndex);
                m_baseJumpCount = JUMP_COUNT[m_currJumpIndex];
                m_jumpCount = m_baseJumpCount;
                break;
            case PlayerSkill.SkillType.Roll:
                m_currConstraintIndex += skill.Level;
                m_currConstraintIndex = Mathf.Min(CONSTRAINTS.Length - 1, m_currConstraintIndex);
                m_baseConstraints = CONSTRAINTS[m_currConstraintIndex];
                break;

            case PlayerSkill.SkillType.JumpDist:
                m_currJumpDistIndex += skill.Level;
                m_currJumpDistIndex = Mathf.Min(JUMP_DIST_MODIFIER.Length - 1, m_currJumpDistIndex);
                m_jumpModifier = JUMP_DIST_MODIFIER[m_currJumpDistIndex];
                break;
            case PlayerSkill.SkillType.Dash:
                m_currDashTimerIndex += skill.Level;
                m_currDashTimerIndex = Mathf.Min(DASH_TIMER.Length - 1, m_currDashTimerIndex);
                m_baseDashTimer = DASH_TIMER[m_currDashTimerIndex];
                m_dashCooldownTimer = m_baseDashTimer;
                break;
            case PlayerSkill.SkillType.Invalid:
                break;
            case PlayerSkill.SkillType.Fish:
                break;
            case PlayerSkill.SkillType.Count:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        m_dashCooldownTimer -= Time.deltaTime;
        
        var rot = Quaternion.AngleAxis(m_cameraTransform.eulerAngles.y, Vector3.up);
        var forward = rot * Vector3.forward;
        var right = rot * Vector3.right;
        m_totalMovementForce = right * m_currRightPushForce + forward * m_currForwardPushForce;
        if (m_baseConstraints == RigidbodyConstraints.FreezeRotation)
        {
            //m_totalMovementForce += Vector3.up * m_noRollJumpSpeed;
            //if (m_rigidbody.velocity.sqrMagnitude > float.Epsilon)
            //{
            //    Vector3 norm = m_rigidbody.velocity.normalized;
            //    Vector3 normProj = Vector3.ProjectOnPlane(norm, Vector3.up);
            //    if (normProj.sqrMagnitude > float.Epsilon)
            //        transform.forward = normProj;
            //}
        }
        // update max speed based off of dash speed

        if (m_dashTimer < m_dashDuration)
        {
            m_maxSpeed =
                Mathf.Lerp(m_baseMaxSpeed + m_dashSpeedIncrement,
                m_baseMaxSpeed,
                m_dashTimer / m_dashDuration);
            m_dashTimer += Time.deltaTime;
        }

        float lerpFactor = (m_maxSpeed - m_baseMaxSpeed) / m_dashSpeedIncrement;
        foreach (var r in m_trailRenderers)
        {
            r.widthMultiplier = lerpFactor;
        }

        m_rigidbody.AddForce(m_totalMovementForce);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void FreezePosition(InputAction.CallbackContext context)
    {
        if (m_baseConstraints == m_rigidbody.constraints)
        {
            SetFrozen(true);
            transform.up = Vector3.up;
            
            transform.position -= Vector3.up * 100.0f;
            m_cinematicCam.Priority = 10000;
        } 
        else
        {
            SetFrozen(false);
            m_cinematicCam.Priority = 0;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (m_dashCooldownTimer > 0.0f) return;

        var dir = m_totalMovementForce.normalized;
        m_rigidbody.velocity = dir * (m_dashSpeedIncrement + m_maxSpeed);
        m_dashTimer = 0.0f;
        m_dashCooldownTimer = m_baseDashTimer;
        
        m_dashAudioEvent.Invoke();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (m_jumpCount < 1) { return; }

        m_rigidbody.AddForce(Vector3.up * m_jumpSpeed * m_jumpModifier);
        m_rigidbody.AddTorque(transform.right * m_jumpRollModifier);
        if (Random.value <  m_jumpSpinRate) 
            m_rigidbody.AddTorque(transform.up * m_jumpSpinModifier);
        m_jumpCount--;
        m_playJumpSoundEvent.Invoke();
    }

    private void StartMove(InputAction.CallbackContext ctx) {
        var input = ctx.ReadValue<Vector2>();
        
        var rot = Quaternion.AngleAxis(m_cameraTransform.eulerAngles.y, Vector3.up);
        var forward = rot * Vector3.forward;
        var right = rot * Vector3.right;
        var impulseMovement = right * input.x + forward * input.y;
        
        m_rigidbody.AddForce(impulseMovement * m_multiplierSpeedImpulse + Vector3.up * m_multiplierSpeedJumpImpulse);
    }
    
    private void Move(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();
        // Debug.Log($"Move input triggered with: {input}, performed {ctx.performed}, canceled {ctx.canceled}");

        m_currRightPushForce = input.x * m_accToApplyFromInput.x;
        m_currForwardPushForce = input.y * m_accToApplyFromInput.y;
        
    }

    // public void UpdateMovementVertical(InputAction.CallbackContext context)
    // {
    //     m_currForwardPushForce = UpdateMovementCommon(context, m_accToApplyFromInput.y);
    // }
    //
    // public void UpdateMovementHorizontal(InputAction.CallbackContext context)
    // {
    //     if (m_rigidbody.constraints == RigidbodyConstraints.FreezeAll)
    //     {
    //         transform.rotation *= Quaternion.AngleAxis(context.ReadValue<float>() * 10.0f, Vector3.up);
    //     }
    //
    //     m_currRightPushForce = UpdateMovementCommon(context, m_accToApplyFromInput.y);
    // }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_">Unused, hack to quickly pass as listener to event</param>
    public void StopMovement(StartDialogueData _ = new StartDialogueData())
    {
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        m_currRightPushForce = 0.0f;
        m_currForwardPushForce = 0.0f;
        m_totalMovementForce = Vector3.zero;

        SetFrozen(true);
    }

    private void EnableMovement()
    {
        SetFrozen(false);
    }

    public void SetFrozen(bool isFrozen)
    {
        m_rigidbody.constraints = isFrozen ?
            RigidbodyConstraints.FreezeAll : m_baseConstraints;
    }

    public void SetPosition(Transform playerAnchor)
    {
        transform.position = playerAnchor.position;
        transform.forward = playerAnchor.forward;
    }

    // private float UpdateMovementCommon(InputAction.CallbackContext context, float updateRate)
    // {
    //     if (context.phase == InputActionPhase.Canceled)
    //     {
    //         return 0.0f;
    //     }
    //
    //     var input = context.ReadValue<Vector2>();
    //     //Debug.Log(string.Format("Vertical: {0} Phase: {1}", input, context.phase));
    //
    //     if (context.phase != InputActionPhase.Started && context.phase != InputActionPhase.Performed)
    //         Debug.LogWarning(string.Format("Unexpectedly got input phase ({0}) for UpdateMovementVertical", context.phase));
    //
    //     return input * updateRate;
    // }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_jumpCount == m_baseJumpCount)
            return;

        if (m_jumpRegainLayerMask ==
            (m_jumpRegainLayerMask | (1 << collision.collider.gameObject.layer)))
        {
            bool allContactsBelow = true;
            foreach (var c in collision.contacts)
            {
                if (transform.position.y < c.point.y)
                {
                    allContactsBelow = false;
                }
            }
            if (allContactsBelow)
            {
                m_jumpCount = m_baseJumpCount;
                Debug.Log("Regained Jump");
            }
        }
    }
}
