using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringAgent : MonoBehaviour
{
    [SerializeField]
    protected float m_maxSpeed = 5.0f;

    [SerializeField]
    protected float m_maxForce = 5.0f;

    [SerializeField]
    protected Vector3 m_velocity = Vector3.zero;

    protected Vector3 m_currForce = Vector3.zero;

    protected void Seek(Vector3 targetPos, float seekingFactor = 1.0f, float slowDistSqr = 0.0f)
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

        m_currForce += ((desiredVelocity - m_velocity) * seekingFactor);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void LateUpdate()
    {
        m_currForce = Vector3.ClampMagnitude(m_currForce, m_maxForce);
        m_velocity += m_currForce * Time.deltaTime;
        m_currForce = Vector3.zero;

        transform.position += m_velocity * Time.deltaTime;
        if (m_velocity.sqrMagnitude > Mathf.Epsilon)
            transform.forward = m_velocity.normalized;
    }
}
