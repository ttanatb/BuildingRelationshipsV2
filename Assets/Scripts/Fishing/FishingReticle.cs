using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingReticle : MonoBehaviour
{
    const string ANIM_NAME_TRIGG_CAST_START = "cast";
    const string ANIM_NAME_TRIGG_CAST_END = "stopCast";
    const string ANIM_NAME_TRIGG_REEL_START = "reel";
    const string ANIM_NAME_TRIGG_REEL_END = "stopReel";


    Camera m_camera = null;
    Renderer[] m_renderers = null;

    [SerializeField]
    Vector3 m_currVel = Vector3.zero;

    [SerializeField]
    Vector2 m_currInput = Vector3.zero;

    [SerializeField]
    Terrain m_terrain = null;

    [SerializeField]
    float m_stoppingThreshold = 0.001f;

    private Animator m_animator = null;


    public void SetAnimTriggReelStart()
    {
        m_animator.SetTrigger(ANIM_NAME_TRIGG_REEL_START);
    }

    public void SetAnimTriggReelEnd()
    {
        m_animator.SetTrigger(ANIM_NAME_TRIGG_REEL_END);
    }

    public void SetAnimTriggCastStart()
    {
        m_animator.SetTrigger(ANIM_NAME_TRIGG_CAST_START);
    }

    public void SetAnimTriggCastEnd()
    {
        m_animator.SetTrigger(ANIM_NAME_TRIGG_CAST_END);
    }


    public void SetActive(bool isActive)
    {
        foreach (var r in m_renderers)
            r.enabled = isActive;
    }

    public void SetPosition(Vector3 pos)
    {
        m_currVel = Vector3.zero;
        transform.position = pos;
    }

    public void SetVelocityX(float x)
    {
        m_currInput.x = x;
    }

    public void SetVelocityY(float y)
    {
        m_currInput.y = y;
    }

    private void Awake()
    {
        m_renderers = GetComponentsInChildren<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main;
        m_terrain = FindObjectOfType<Terrain>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 y = Vector3.ProjectOnPlane(m_camera.transform.forward, Vector3.up) * m_currInput.y;
        Vector3 x = Vector3.ProjectOnPlane(m_camera.transform.right, Vector3.up) * m_currInput.x;

        m_currVel = y + x;
        if (m_currVel.sqrMagnitude < Mathf.Epsilon)
            return;

        Vector3 newPos = transform.position + m_currVel * Time.deltaTime;
        if (m_terrain.SampleHeight(newPos) < transform.position.y)
        {
            transform.position = newPos;
        }
    }
}
