using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutonomousAgent : MonoBehaviour
{
    [SerializeField]
    protected float m_maxSpeed = 5.0f;

    protected float m_baseMaxSpeed = 5.0f;

    [SerializeField]
    protected float m_frictionFactor = 0.1f;

    protected Rigidbody m_rigidbody = null;

#if UNITY_EDITOR
    Vector3 m_currSeekTarget = Vector3.zero;
#endif

    protected virtual void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_baseMaxSpeed = m_maxSpeed;
    }

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

        m_rigidbody.AddForce((desiredVelocity - m_rigidbody.velocity) * seekingFactor);

#if UNITY_EDITOR
        m_currSeekTarget = targetPos;
#endif
    }

    protected virtual void LateUpdate()
    {
        // Clamp X/Z velocity, keep Y-velocity.
        var vel = m_rigidbody.velocity;
        float yVel = vel.y;
        vel.y = 0.0f;
        vel = Vector3.ClampMagnitude(vel, m_maxSpeed);
        vel.y = yVel;
        m_rigidbody.velocity = vel;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_currSeekTarget, 0.1f);
    }
#endif
}
