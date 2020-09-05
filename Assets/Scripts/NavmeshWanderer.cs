using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavmeshWanderer : MonoBehaviour
{
    NavMeshAgent m_agent = null;

    [SerializeField]
    TerrainCollider m_terrainCollider = null;
    float m_maxRadius = 5.0f;
    Vector3 m_startPos = Vector3.zero;

    [SerializeField]
    float m_lookAheadDist = 1.0f;

    [SerializeField]
    float m_lookAheadRadius = 1.0f;

    [SerializeField]
    Vector2 m_wanderAngleUpdateRate = Vector2.one;

    [SerializeField]
    Vector2 m_lookAheadEulerAngles = Vector2.zero;

    [SerializeField]
    float m_reseekDuration = 2.0f;

    [SerializeField]
    float m_radiusToStartFishReelEvent = 0.1f;

    Fish m_fish = null;

    Vector3 m_wanderTarget = Vector3.zero;
    Vector3 m_lookAheadPos = Vector3.zero;
    Vector3 m_reflectedLookAheadPos = Vector3.zero;
    Quaternion m_lookAheadRot = Quaternion.identity;

    Vector3 m_prevReflectedLookAheadPos = Vector3.zero;

    bool m_stopped = false;

    float m_newPosTimer = 0.0f;
    public TerrainCollider TerrainCollider { set { m_terrainCollider = value; } }
    public float MaxRadius { set { m_maxRadius = value; } }
    public Vector3 StartPos { set { m_startPos = value; } }

    public FishingReticle SeekTarget { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_radiusToStartFishReelEvent += m_agent.radius;
        m_fish = GetComponent<Fish>();
    }

    public void SetStopped(bool shouldStop)
    {
        m_stopped = shouldStop;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_stopped) return;

        if (SeekTarget != null)
        {
            Vector3 seekTarget = SeekTarget.transform.position;
            seekTarget.y = transform.position.y;
            m_agent.SetDestination(seekTarget);

            if ((seekTarget - transform.position).sqrMagnitude <
                (m_radiusToStartFishReelEvent * m_radiusToStartFishReelEvent))
            {
                m_fish.TriggerFishEvent();
                SetStopped(true);
            }
            return;
        }

        m_lookAheadEulerAngles.x += Random.value * m_wanderAngleUpdateRate.x;
        m_lookAheadEulerAngles.y += Random.value * m_wanderAngleUpdateRate.y;
        if (m_newPosTimer > m_reseekDuration)
        {
            m_lookAheadPos = transform.forward * m_lookAheadDist + transform.position;
            m_lookAheadRot = Quaternion.AngleAxis(m_lookAheadEulerAngles.x, Vector3.up) *
                Quaternion.AngleAxis(m_lookAheadEulerAngles.y, Vector3.right);

            m_wanderTarget = m_lookAheadPos + ((m_lookAheadRot * Vector3.forward) * m_lookAheadRadius);
            Vector3 dirToTarget = m_wanderTarget - transform.position;

            Vector3 seekDir = dirToTarget;
            Ray toWanderTarget = new Ray(transform.position, dirToTarget);
            if (m_terrainCollider.Raycast(toWanderTarget, out RaycastHit hitinfo, m_lookAheadDist))
            {
                Vector3 newDir = Vector3.Reflect(seekDir, transform.forward);
                seekDir = newDir;

                m_reflectedLookAheadPos = transform.position + newDir;
            }

            Vector3 seekPos = seekDir + transform.position;
            if ((seekPos - m_startPos).sqrMagnitude > (m_maxRadius * m_maxRadius))
            {
                seekPos = m_startPos;
            }

            m_agent.SetDestination(seekPos);
            m_newPosTimer = 0.0f;
        }

        m_newPosTimer += Time.deltaTime;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, m_lookAheadPos);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(m_lookAheadPos, m_lookAheadRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(m_lookAheadPos, m_wanderTarget);

        if ((m_prevReflectedLookAheadPos - m_reflectedLookAheadPos).sqrMagnitude > Mathf.Epsilon)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(m_lookAheadPos, m_reflectedLookAheadPos);
        }

        m_prevReflectedLookAheadPos = m_reflectedLookAheadPos;
    }

}
