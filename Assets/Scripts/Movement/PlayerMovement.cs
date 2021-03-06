﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : AutonomousAgent
{
    [SerializeField]
    Transform m_cameraTransform = null;

    [SerializeField]
    TrailRenderer[] m_trailRenderers = null;

    [SerializeField]
    Vector2 m_accToApplyFromInput = new Vector2(0.5f, 2.0f);

    [SerializeField]
    float m_currForwardPushForce = 0.0f;

    [SerializeField]
    float m_currRightPushForce = 0.0f;

    [SerializeField]
    float m_dashSpeedIncrement = 50.0f;

    [SerializeField]
    float m_dashDuration = 2.0f;
    float m_dashTimer = 0.0f;

    [SerializeField]
    float m_jumpSpeed = 50.0f;

    [SerializeField]
    float m_noRollJumpSpeed = 50.0f;

    PlayerControls m_playerControls = null;

    Vector3 m_totalMovementForce = Vector3.zero;

    [SerializeField]
    int m_jumpCount = 0;
    int m_baseJumpCount = 0;

    float m_dashCooldownTimer = 0;
    float m_baseDashTimer = 0;
    float m_jumpModifier = 0;

    EventManager m_eventManager = null;

    static readonly int[] JUMP_COUNT = { 0, 1, 2, int.MaxValue };
    static readonly float[] JUMP_DIST_MODIFIER = { 0.6f, 0.8f, 1.0f, 1.25f };
    static readonly float[] DASH_TIMER = { float.PositiveInfinity, 2.0f, 1.0f, 0.0f };
    static readonly RigidbodyConstraints[] CONSTRAINTS = {
        RigidbodyConstraints.FreezeRotationZ,
        RigidbodyConstraints.None,
    };

    int m_currJumpIndex = 0;
    int m_currJumpDistIndex = 0;
    int m_currDashTimerIndex = 0;
    int m_currConstraintIndex = 0;

    [SerializeField]
    RigidbodyConstraints m_baseConstraints = RigidbodyConstraints.FreezeRotationZ;

    [SerializeField]
    LayerMask m_jumpRegainLayerMask = 1;


    private void UpdateSkillLevel(PlayerSkill skill)
    {
        switch (skill.type)
        {
            case PlayerSkill.Type.JumpCount:
                m_currJumpIndex += skill.level;
                m_currJumpIndex = Mathf.Min(JUMP_COUNT.Length - 1, m_currJumpIndex);
                m_baseJumpCount = JUMP_COUNT[m_currJumpIndex];
                m_jumpCount = m_baseJumpCount;
                break;
            case PlayerSkill.Type.Roll:
                m_currConstraintIndex += skill.level;
                m_currConstraintIndex = Mathf.Min(CONSTRAINTS.Length - 1, m_currConstraintIndex);
                m_baseConstraints = CONSTRAINTS[m_currConstraintIndex];
                break;

            case PlayerSkill.Type.JumpDist:
                m_currJumpDistIndex += skill.level;
                m_currJumpDistIndex = Mathf.Min(JUMP_DIST_MODIFIER.Length - 1, m_currJumpDistIndex);
                m_jumpModifier = JUMP_DIST_MODIFIER[m_currJumpDistIndex];
                break;
            case PlayerSkill.Type.Dash:
                m_currDashTimerIndex += skill.level;
                m_currDashTimerIndex = Mathf.Min(DASH_TIMER.Length - 1, m_currDashTimerIndex);
                m_baseDashTimer = DASH_TIMER[m_currDashTimerIndex];
                m_dashCooldownTimer = m_baseDashTimer;
                break;
        }
    }

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

        m_playerControls = GetComponent<PlayerController>().PlayerControls;
        m_eventManager = EventManager.Instance;

        if (m_cameraTransform == null)
            m_cameraTransform = Camera.main.transform;

        m_playerControls.Player.Dash.started += Dash;
        m_playerControls.Player.Jump.started += Jump;
        m_playerControls.Player.Horizontal.started += UpdateMovementHorizontal;
        m_playerControls.Player.Vertical.started += UpdateMovementVertical;

        m_playerControls.Player.Horizontal.canceled += UpdateMovementHorizontal;
        m_playerControls.Player.Vertical.canceled += UpdateMovementVertical;
        m_eventManager.AddSkillUnlockedListener(UpdateSkillLevel);
    }

    private void OnDestroy()
    {
        m_playerControls.Player.Dash.started -= Dash;
        m_playerControls.Player.Jump.started -= Jump;
        m_playerControls.Player.Horizontal.started -= UpdateMovementHorizontal;
        m_playerControls.Player.Vertical.started -= UpdateMovementVertical;

        m_playerControls.Player.Horizontal.canceled -= UpdateMovementHorizontal;
        m_playerControls.Player.Vertical.canceled -= UpdateMovementVertical;
    }

    // Update is called once per frame
    void Update()
    {
        m_dashCooldownTimer -= Time.deltaTime;
        Quaternion rot = Quaternion.AngleAxis(m_cameraTransform.eulerAngles.y, Vector3.up);
        Vector3 forward = rot * Vector3.forward;
        Vector3 right = rot * Vector3.right;
        m_totalMovementForce = right * m_currRightPushForce + forward * m_currForwardPushForce;
        if (m_baseConstraints == RigidbodyConstraints.FreezeRotation)
        {
            m_totalMovementForce += Vector3.up * m_noRollJumpSpeed;
            if (m_rigidbody.velocity.sqrMagnitude > float.Epsilon)
            {
                Vector3 norm = m_rigidbody.velocity.normalized;
                Vector3 normProj = Vector3.ProjectOnPlane(norm, Vector3.up);
                if (normProj.sqrMagnitude > float.Epsilon)
                    transform.forward = normProj;
            }
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

    public void Dash(InputAction.CallbackContext context)
    {
        if (m_dashCooldownTimer > 0.0f) return;

        Vector3 dir = m_totalMovementForce.normalized;
        m_rigidbody.velocity = dir * (m_dashSpeedIncrement + m_maxSpeed);
        m_dashTimer = 0.0f;
        m_dashCooldownTimer = m_baseDashTimer;
    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (m_jumpCount < 1) { return; }


        m_rigidbody.AddForce(Vector3.up * m_jumpSpeed * m_jumpModifier);
        m_jumpCount--;
    }

    public void UpdateMovementVertical(InputAction.CallbackContext context)
    {
        m_currForwardPushForce = UpdateMovementCommon(context, m_accToApplyFromInput.y);
    }

    public void UpdateMovementHorizontal(InputAction.CallbackContext context)
    {
        m_currRightPushForce = UpdateMovementCommon(context, m_accToApplyFromInput.y);
    }

    public void StopMovement()
    {
        m_rigidbody.velocity = Vector3.zero;
        m_rigidbody.angularVelocity = Vector3.zero;
        m_currRightPushForce = 0.0f;
        m_currForwardPushForce = 0.0f;
        m_totalMovementForce = Vector3.zero;

        SetFrozen(true);
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

    private float UpdateMovementCommon(InputAction.CallbackContext context, float updateRate)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            return 0.0f;
        }

        float input = context.ReadValue<float>();
        //Debug.Log(string.Format("Vertical: {0} Phase: {1}", input, context.phase));

        if (context.phase != InputActionPhase.Started && context.phase != InputActionPhase.Performed)
            Debug.LogWarning(string.Format("Unexpectedly got input phase ({0}) for UpdateMovementVertical", context.phase));

        return input * updateRate;
    }

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
