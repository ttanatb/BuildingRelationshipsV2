using System;
using System.Collections;
using System.Collections.Generic;
using Fishing.SO;
using Fishing.Structs;
using NaughtyAttributes;
using Sound.SO;
using UnityEngine;
using UnityEngine.AI;
using Utilr.SoGameEvents;
using Utilr.Utility;

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

    [SerializeField] private SoGameEvent m_startFish = null;
    [SerializeField] private SoGameEvent m_castRod = null;
    [SerializeField] private SoGameEvent m_quitFishing = null;
    [SerializeField] private PlayLoopingAudioEvent m_moveAudioNorth = null;
    [SerializeField] private PlayLoopingAudioEvent m_moveAudioEast = null;
    [SerializeField] private PlayLoopingAudioEvent m_moveAudioWest = null;
    [SerializeField] private PlayLoopingAudioEvent m_moveAudioSouth = null;
    [SerializeField] private AnimationCurve m_distanceVolume = null;
    
    [SerializeField] private SoCurrentCamera m_currentCamera = null;
    [SerializeField] private PlayOneShotRandomAudioClipEvent m_splashAudio = null;
    [SerializeField] private float m_splashSoundDelay = 0.3f;

    private void OnStartFish()
    {
        m_moveAudioNorth.Invoke();
        m_moveAudioEast.Invoke();
        m_moveAudioWest.Invoke();
        m_moveAudioSouth.Invoke();
    }

    private void OnNoLongerAiming()
    {
        m_moveAudioNorth.InvokeStop();
        m_moveAudioEast.InvokeStop();
        m_moveAudioWest.InvokeStop();
        m_moveAudioSouth.InvokeStop();
    }

    
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
        float dist = (transform.position - m_camera.transform.position).magnitude;
        float volumeFactor = m_distanceVolume.Evaluate(dist);
        var data = m_splashAudio.Data;
        var prevVolume = data;
        data.Volume *= volumeFactor;
        
        StopAllCoroutines();
        StartCoroutine(Helper.ExecuteAfter(() => {
            m_splashAudio.Invoke(data);
            
            // Reset volume.
            m_splashAudio.Data = prevVolume;
        }, m_splashSoundDelay));
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
        m_startFish.Event.AddListener(OnStartFish);
        m_castRod.Event.AddListener(OnNoLongerAiming);
        m_quitFishing.Event.AddListener(OnNoLongerAiming);
        
        SetTarget(m_moveAudioNorth);
        SetTarget(m_moveAudioEast);
        SetTarget(m_moveAudioSouth);
        SetTarget(m_moveAudioWest);
    }
    
    private void SetTarget(PlayLoopingAudioEvent evt)
    {
        var data = evt.Data;
        data.Target = transform;
        evt.Data = data;
    }

    private void OnDestroy()
    {
        m_startFish.Event.RemoveListener(OnStartFish);
        m_castRod.Event.RemoveListener(OnNoLongerAiming);
        m_quitFishing.Event.RemoveListener(OnNoLongerAiming);
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
        
        transform.position = posNextFrame;
    }

    private void Update()
    {
        if (m_currVel.sqrMagnitude < Mathf.Epsilon)
            return;

        float right = Vector3.Dot(transform.right, m_currVel);
        float forward = Vector3.Dot(transform.forward, m_currVel);
        float dist = (transform.position - m_camera.transform.position).magnitude;
        float volumeFactor = m_distanceVolume.Evaluate(dist);
        
        m_moveAudioNorth.InvokeVolume(volumeFactor * Mathf.Clamp01(forward / m_maxSpeed));
        m_moveAudioSouth.InvokeVolume(volumeFactor * Mathf.Clamp01(-forward / m_maxSpeed));
        m_moveAudioEast.InvokeVolume(volumeFactor * Mathf.Clamp01(right / m_maxSpeed));
        m_moveAudioWest.InvokeVolume(volumeFactor * Mathf.Clamp01(-right / m_maxSpeed));
    }
}
