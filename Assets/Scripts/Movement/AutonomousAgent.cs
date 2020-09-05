using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAgent : MonoBehaviour
{
    [SerializeField]
    protected Vector3 m_currVelocity = Vector3.zero;

    [SerializeField]
    protected float m_mass = 1.0f;

    [SerializeField]
    protected float m_maxForce = 10.0f;

    [SerializeField]
    protected float m_maxSpeed = 5.0f;

    [SerializeField]
    protected Vector3 m_currForce = Vector3.zero;

    [SerializeField]
    protected float m_frictionFactor = 0.1f;

    [SerializeField]
    protected float m_stoppingDistSqrThreshold = 0.01f;

    [SerializeField]
    protected float m_rotAdjustmentRate = 10f;

#if UNITY_EDITOR
    Vector3 m_currSeekTarget = Vector3.zero;
#endif

    public void Seek(Vector3 targetPos, float seekingFactor = 1.0f, float slowDistSqr = 0.0f)
    {
        Vector3 toTargetPos = targetPos - transform.position;
        Vector3 toTargetPosNorm = toTargetPos.normalized;

        Vector3 desiredVelocity = toTargetPosNorm * m_maxSpeed;
        if (slowDistSqr > 0.0f)
        {
            float toTargetDistSqr = toTargetPos.sqrMagnitude;
            if (toTargetDistSqr < slowDistSqr)
            {
                desiredVelocity *= toTargetPos.sqrMagnitude / slowDistSqr;
            }
        }

        m_currForce += (desiredVelocity - m_currVelocity) * seekingFactor;

#if UNITY_EDITOR
        m_currSeekTarget = targetPos;
#endif
    }

    public void AddForce(Vector3 force)
    {

    }

    protected virtual void LateUpdate()
    {
        // Apply friction.
        m_currForce += -m_currVelocity * m_frictionFactor;

        // Clamp friction and convert to acc.
        Vector3 totalForce = Vector3.ClampMagnitude(m_currForce, m_maxForce);
        Vector3 acc = totalForce / m_mass;

        // Apply acc to vel, clamp, if necessary.
        m_currVelocity += acc * Time.deltaTime;
        if (m_currVelocity.sqrMagnitude < m_stoppingDistSqrThreshold)
            m_currVelocity = Vector3.zero;

        // Apply vel to pos.
        transform.position += m_currVelocity * Time.deltaTime;

        // Reset forces for next update.
        m_currForce = Vector3.zero;

        Vector3 resultantForward = Vector3.Lerp(transform.forward,
            m_currVelocity.normalized, m_rotAdjustmentRate * Time.deltaTime);
        if (resultantForward == Vector3.zero)
            resultantForward = Vector3.forward;
        transform.forward = resultantForward;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_currSeekTarget, 0.1f);
    }
#endif
}
