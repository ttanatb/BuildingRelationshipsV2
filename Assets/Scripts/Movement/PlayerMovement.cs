using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : AutonomousAgent
{
    [SerializeField]
    Transform m_cameraTransform = null;

    // X is for start-phase, Y is for performed-phase
    [SerializeField]
    Vector2 m_accToApplyFromInput = new Vector2(0.5f, 2.0f);

    [SerializeField]
    float m_currForwardPushForce = 0.0f;

    [SerializeField]
    float m_currRightPushForce = 0.0f;

    [SerializeField]
    float m_dashSpeed = 20.0f;

    [SerializeField]
    float m_jumpSpeed = 50.0f;

    Vector3 m_dashVelocity = Vector3.zero;
    Vector3 m_jumpVelocity = Vector3.zero;

    PlayerInput m_playerInput = null;
    PlayerControls m_playerControls = null;

    bool m_shouldStop = false;

    private void Awake()
    {
    }



    // Start is called before the first frame update
    void Start()
    {
        m_playerControls = GetComponent<PlayerController>().PlayerControls;

        if (m_cameraTransform == null)
            m_cameraTransform = Camera.main.transform;

        m_playerInput = GetComponent<PlayerInput>();

        m_playerControls.Player.Dash.started += Dash;
        m_playerControls.Player.Jump.started += Jump;
        m_playerControls.Player.Horizontal.started += UpdateMovementHorizontal;
        m_playerControls.Player.Vertical.started += UpdateMovementVertical;

        m_playerControls.Player.Horizontal.canceled += UpdateMovementHorizontal;
        m_playerControls.Player.Vertical.canceled += UpdateMovementVertical;
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
        Quaternion rot = Quaternion.AngleAxis(m_cameraTransform.eulerAngles.y, Vector3.up);
        Vector3 forward = rot * Vector3.forward;
        Vector3 right = rot * Vector3.right;
        Vector3 totalForce = right * m_currRightPushForce + forward * m_currForwardPushForce;

        m_currForce += Vector3.ClampMagnitude(totalForce, m_maxForce);
    }

    protected override void LateUpdate()
    {
        //if (m_dashVelocity.sqrMagnitude > 0.0f)
        //{
        //    m_currVelocity = m_dashVelocity;
        //    m_dashVelocity = Vector3.zero;
        //}

        m_currVelocity += m_jumpVelocity;
        m_jumpVelocity = Vector3.zero;

        if (m_shouldStop)
        {
            m_shouldStop = false;
            m_currVelocity = Vector3.zero;
            m_currForce = Vector3.zero;
        }

        base.LateUpdate();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        transform.position += m_dashSpeed * transform.forward;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        m_jumpVelocity = Vector3.up * m_jumpSpeed;
    }

    public void UpdateMovementVertical(InputAction.CallbackContext context)
    {
        UpdateMovementCommon(context, out m_currForwardPushForce);
    }

    public void UpdateMovementHorizontal(InputAction.CallbackContext context)
    {
        UpdateMovementCommon(context, out m_currRightPushForce);
    }

    public void StopMovement()
    {
        m_shouldStop = true;
        m_currRightPushForce = 0.0f;
        m_currForwardPushForce = 0.0f;
    }

    private void UpdateMovementCommon(InputAction.CallbackContext context, out float currAcc)
    {
        if (context.phase == InputActionPhase.Canceled)
        {
            currAcc = 0.0f;
            return;
        }

        float input = context.ReadValue<float>();
        Debug.Log(string.Format("Vertical: {0} Phase: {1}", input, context.phase));

        if (context.phase != InputActionPhase.Started && context.phase != InputActionPhase.Performed)
            Debug.LogWarning(string.Format("Unexpectedly got input phase ({0}) for UpdateMovementVertical", context.phase));

        currAcc = input * m_accToApplyFromInput.x;
    }
}
