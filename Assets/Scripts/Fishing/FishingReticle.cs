using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using Utilr.SoGameEvents;

public class FishingReticle : MonoBehaviour
{
    [SerializeField] private Animator m_animator = null;

    [AnimatorParam("m_animator")] [SerializeField]
    private int m_animStartCast = 0;
    
    [AnimatorParam("m_animator")] [SerializeField]
    private int m_animStopCast = 0;
    
    [AnimatorParam("m_animator")] [SerializeField]
    private int m_animStartReel = 0;

    [AnimatorParam("m_animator")] [SerializeField]
    private int m_animStopReel = 0;

    private Camera m_camera = null;
    private Renderer[] m_renderers = null;

    [SerializeField] private Vector3 m_currVel = Vector3.zero;
    [SerializeField] private Vector2 m_currInput = Vector3.zero;
    [SerializeField] private float m_bounceCoefficient = 0.1f;
    [SerializeField] private float m_frictionCoefficient = 0.2f;
    [SerializeField] private float m_maxSpeed = 20f;

    [SerializeField] private SoCurrentCamera m_currentCamera = null;


    public void AnimateStartReel()
    {
        m_animator.SetTrigger(m_animStartReel);
    }

    public void AnimateStopReel()
    {
        m_animator.SetTrigger(m_animStopReel);
    }

    public void AnimateStartCast()
    {
        m_animator.SetTrigger(m_animStartCast);
    }

    public void AnimateStopCast()
    {
        m_animator.SetTrigger(m_animStopCast);
    }


    public void SetActive(bool isActive)
    {
        foreach (var r in m_renderers)
            r.enabled = isActive;
    }

    public void SetStartingTransform(Vector3 pos, Vector3 normal)
    {
        m_currVel = Vector3.zero;
        var thisTransform = transform;
        
        thisTransform.position = pos;
        thisTransform.up = normal;
    }


    public void Freeze()
    {
        m_currVel = Vector3.zero;
        m_currInput = Vector2.zero;
    }
    
    public void SetAccX(float x)
    {
        m_currInput.x = x;
    }

    public void SetAccY(float y)
    {
        m_currInput.y = y;
    }

    private void Awake()
    {
        m_renderers = GetComponentsInChildren<Renderer>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_camera = m_currentCamera.Cam;
    }

    private bool CheckIfOutsideWater(Vector3 newPos, out NavMeshHit hit)
    {
        NavMesh.SamplePosition(newPos, out hit, Mathf.Infinity, NavMesh.AllAreas);
        var posOnNavMesh = hit.position;
        posOnNavMesh.y = newPos.y;

        return Vector3.Distance(newPos, posOnNavMesh) > float.Epsilon;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        var up = transform.up;
        var acc = Vector3.ProjectOnPlane(m_camera.transform.forward, up) * m_currInput.y 
            + Vector3.ProjectOnPlane(m_camera.transform.right, up) * m_currInput.x;
        acc += -m_currVel * m_frictionCoefficient;
        
        m_currVel += acc * Time.fixedDeltaTime;
        m_currVel = Vector3.ClampMagnitude(m_currVel, m_maxSpeed);
        
        var posNextFrame = transform.position + m_currVel * Time.fixedDeltaTime;
        
        if (m_currVel.sqrMagnitude < Mathf.Epsilon)
            return;

        if (CheckIfOutsideWater(posNextFrame, out var hit))
        {
            posNextFrame = transform.position;
            m_currVel = -m_currVel * m_bounceCoefficient;
        }
        else
        {
            posNextFrame.x = hit.position.x;
            posNextFrame.z = hit.position.z;
        }

        // var newPos = transform.position + m_currVel * Time.fixedDeltaTime;
        // var posOnNavMesh = hit.position;
        // posOnNavMesh.y = newPos.y;
        //
        // // Check if new position is outside of nav mesh
        // if (Vector3.Distance(newPos, posOnNavMesh) > float.Epsilon)
        // {
        //     // Bounce it back.
        //     var negativeDir = posOnNavMesh - newPos;
        //     negativeDir = Vector3.ProjectOnPlane(negativeDir, up);
        //
        //     newPos = posOnNavMesh + negativeDir * m_bounceCoefficient;
        // }
        // else
        // {
        //     newPos = posOnNavMesh;
        // }
        
        transform.position = posNextFrame;
    }
}
