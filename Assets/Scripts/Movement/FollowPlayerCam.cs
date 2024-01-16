using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowPlayerCam : AutonomousAgent
{
    const float BASE_CAM_SENSITVITY = 10.0f;

    [SerializeField]
    Transform m_playerTransform = null;

    [SerializeField]
    Transform m_cameraTransform = null;

    [SerializeField]
    Camera m_camera = null;

    // If camera within sqrDist of target, stop.
    [SerializeField]
    float m_slowingDistanceSqr = 0.1f;

    [SerializeField]
    float m_rotLerpFactor = 10f;

    // Fields for adjust child (camera).

    // Angle controlling how much camera is looking downwards to player.
    [SerializeField]
    float m_angleLookDown = 30.0f;

    // Angle controlling where around the player, the camera should be positioned.
    [SerializeField]
    float m_angleAroundPlayer = 180.0f;

    // Angle controlling how much camera is looking downwards to player.
    [SerializeField]
    Vector2 m_angleLookDownMinMax = new Vector2(0, 180);

    [SerializeField]
    Vector2 m_targetDistToPlayer = new Vector2(5.0f, 10.0f);

    [SerializeField]
    Vector2 m_lookSensitivity = Vector2.one;

    // Fields adjusted by input.

    [SerializeField]
    bool m_isAdjustingLookAngle = false;

    [SerializeField]
    Vector2 m_cameraAngleChange = Vector2.zero;

    // Binding input

    // PlayerInput m_playerInput = null;
    // PlayerControls m_playerControls = null;

    // DEBUG

    [SerializeField]
    Vector3 m_targetPosition = Vector3.zero;

    Quaternion m_targetRotation = Quaternion.identity;
    Vector3 nextPos = Vector3.zero;

    private void Awake()
    {
        // m_playerControls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_cameraTransform == null)
            m_cameraTransform = GetComponentInChildren<Camera>().transform;

        m_camera = m_cameraTransform.GetComponent<Camera>();
        //m_cameraAgent = m_cameraTransform.GetComponent<AutonomousAgent>();



        m_targetPosition = transform.position;
        m_targetRotation = transform.rotation;
        nextPos = transform.position;

        // m_playerInput = m_playerTransform.GetComponent<PlayerInput>();
        //
        // m_playerControls.Player.TriggerLook.started += UpdateRightClick;
        // m_playerControls.Player.TriggerLook.performed += UpdateRightClick;
        // m_playerControls.Player.TriggerLook.canceled += UpdateRightClick;
        // m_playerControls.Player.Look.started += UpdateMovement;
        // m_playerControls.Player.Look.performed += UpdateMovement;
        // m_playerControls.Player.Look.canceled += UpdateMovement;
    }

    private void OnDestroy()
    {
        // m_playerControls.Player.TriggerLook.started -= UpdateRightClick;
        // m_playerControls.Player.TriggerLook.performed -= UpdateRightClick;
        // m_playerControls.Player.TriggerLook.canceled -= UpdateRightClick;
        // m_playerControls.Player.Look.started -= UpdateMovement;
        // m_playerControls.Player.Look.performed -= UpdateMovement;
        // m_playerControls.Player.Look.canceled -= UpdateMovement;
    }

    private void UpdateLookRotation()
    {
        // Update x/y angles.
        if (m_isAdjustingLookAngle)
        {
            m_angleAroundPlayer += m_cameraAngleChange.x * BASE_CAM_SENSITVITY * m_lookSensitivity.x * Time.deltaTime;
            m_angleLookDown -= m_cameraAngleChange.y * BASE_CAM_SENSITVITY * m_lookSensitivity.y * Time.deltaTime;
            m_angleLookDown = Mathf.Clamp(m_angleLookDown, m_angleLookDownMinMax.x, m_angleLookDownMinMax.y);
        }

        // Parent rotation around player.
        Quaternion newParentRotation = Quaternion.AngleAxis(m_angleAroundPlayer, Vector3.up);
        transform.rotation = newParentRotation;

        // Set local position for camera.
        float y = Mathf.Sin(Mathf.Deg2Rad * m_angleLookDown);
        float z = Mathf.Cos(Mathf.Deg2Rad * m_angleLookDown);

        // TODO: Change dist based on player curr speed.
        float distToPlayer = m_targetDistToPlayer.x;
        Vector3 newLocalPos = (new Vector3(0.0f, y, z)).normalized * distToPlayer;
        m_cameraTransform.localPosition = newLocalPos;
        //m_cameraTransform.position = transform.localToWorldMatrix.MultiplyPoint(newLocalPos);
        //m_cameraAgent.Seek(transform.localToWorldMatrix.MultiplyPoint(newLocalPos), 1.0f, m_cameraStoppingDistSqrThreshold);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLookRotation();
        m_targetPosition = m_playerTransform.position;
        //Seek(m_targetPosition, 1.0f, m_slowingDistanceSqr);

        // Update position.
        /*
        Vector3 toTargetPos = m_targetPosition - transform.position;
        float toTargetPosSqrMag = toTargetPos.sqrMagnitude;
        Vector3 toTargetPosNorm = toTargetPos.normalized;

        Vector3 desiredVelocity = toTargetPosNorm * m_maxSpeed;
        Vector3 steeringForce = (desiredVelocity - m_currVelocity) * m_seekingFactor;

        float rampedSpeed = m_maxSpeed * (toTargetPosSqrMag / m_slowingDistance);
        float clippedSpeed = Mathf.Min(rampedSpeed, m_maxSpeed);

        desiredVelocity = (clippedSpeed / toTargetPosSqrMag) * toTargetPos;
        steeringForce += (desiredVelocity - m_currVelocity) * m_slowingFactor;

        // Update velocity
        m_currVelocity += steeringForce;
        m_currVelocity = Vector3.ClampMagnitude(m_currVelocity, m_maxSpeed);

        nextPos = transform.position + m_currVelocity * Time.deltaTime;
        //if (Vector3.Dot(m_targetPosition - nextPos, m_targetPosition - transform.position) < 0.0f)
        //{
        //    m_currVelocity = Vector3.zero;
        //}

        //if ((m_targetPosition - transform.position).sqrMagnitude < m_slowingDistance)
        //{
        //    m_currVelocity *= m_slowingFactor;
        //}

        if ((m_targetPosition - transform.position).sqrMagnitude < m_stoppingDistSqrThreshold)
        {
            m_currVelocity = Vector3.zero;
        }

        // Set target position.
        Vector3 newPos = transform.position + m_currVelocity * Time.deltaTime;
        transform.position = newPos;
        */

        // Look at player.
        m_targetRotation = Quaternion.LookRotation((m_playerTransform.position - m_cameraTransform.position).normalized);
        Quaternion newRotation = Quaternion.Lerp(m_cameraTransform.rotation, m_targetRotation, m_rotLerpFactor * Time.deltaTime);
        m_cameraTransform.rotation = newRotation;
    }

    //protected override void LateUpdate()
    //{
    //    base.LateUpdate();
    //}

    // public void UpdateMovement(InputAction.CallbackContext context)
    // {
    //     Vector2 input = context.ReadValue<Vector2>();
    //     input.x *= m_camera.aspect;
    //     //input.y *= Screen.height;
    //     m_cameraAngleChange = input;
    // }
    //
    // public void UpdateRightClick(InputAction.CallbackContext context)
    // {
    //     float input = context.ReadValue<float>();
    //     m_isAdjustingLookAngle = input > 0.0f;
    //     Cursor.lockState = m_isAdjustingLookAngle ? CursorLockMode.Locked : CursorLockMode.Confined;
    //     m_cameraAngleChange = Vector2.zero;
    // }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_targetPosition, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(nextPos, 0.1f);
    }
}
