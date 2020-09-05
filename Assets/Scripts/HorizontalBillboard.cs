using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalBillboard : MonoBehaviour
{
    const float TOTAL_ANGLE = 360f;
    const int NUM_SEGMENTS = 8;
    const float ANGLE_PER_SEG = TOTAL_ANGLE / NUM_SEGMENTS;

    [SerializeField]
    Camera m_camera = null;

    [SerializeField]
    float m_angleAlongRight = -60.0f;

    float m_baseAngleAlongUp = 0.0f;

    [SerializeField]
    Animator m_mainAnimator = null;

    [SerializeField]
    Animator m_shadowAnimator = null;

    Dictionary<int, string> m_angleToAnimBool = new Dictionary<int, string>();

    int m_prevSegment = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_baseAngleAlongUp = transform.eulerAngles.y;

        m_angleToAnimBool.Add(0, "faceF");
        m_angleToAnimBool.Add(1, "faceFR");
        m_angleToAnimBool.Add(2, "faceR");
        m_angleToAnimBool.Add(3, "faceBR");
        m_angleToAnimBool.Add(4, "faceB");
        m_angleToAnimBool.Add(5, "faceBL");
        m_angleToAnimBool.Add(6, "faceL");
        m_angleToAnimBool.Add(7, "faceFL");
        m_angleToAnimBool.Add(8, "faceF");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toCamera = m_camera.transform.position - transform.position;
        Vector3 projectedToCam = Vector3.ProjectOnPlane(toCamera, Vector3.up);
        float angleAlongUp =
            Vector3.SignedAngle(-Vector3.forward, projectedToCam, Vector3.up);

        float turnAngle = angleAlongUp - m_baseAngleAlongUp;
        if (turnAngle < 0)
            turnAngle += Mathf.Ceil(-turnAngle / TOTAL_ANGLE) * TOTAL_ANGLE;
        if (turnAngle > TOTAL_ANGLE)
            turnAngle += Mathf.Floor(-turnAngle / TOTAL_ANGLE) * TOTAL_ANGLE;

        int currSegment = Mathf.RoundToInt(turnAngle / ANGLE_PER_SEG);
        if (currSegment != m_prevSegment &&
            currSegment - m_prevSegment != m_angleToAnimBool.Count - 1)
        {
            m_mainAnimator.SetBool(m_angleToAnimBool[m_prevSegment], false);
            m_mainAnimator.SetBool(m_angleToAnimBool[currSegment], true);
            m_shadowAnimator.SetBool(m_angleToAnimBool[m_prevSegment], false);
            m_shadowAnimator.SetBool(m_angleToAnimBool[currSegment], true);
            m_prevSegment = currSegment;
        }

        //Debug.DrawRay(transform.position, Vector3.up, Color.blue);
        //Debug.DrawRay(transform.position, projectedToCam, Color.red);
        //Debug.Log(angleAlongUp);

        Quaternion rotation =
            Quaternion.AngleAxis(angleAlongUp, Vector3.up) *
            Quaternion.AngleAxis(m_angleAlongRight, Vector3.right);

        transform.rotation = rotation;
    }
}
