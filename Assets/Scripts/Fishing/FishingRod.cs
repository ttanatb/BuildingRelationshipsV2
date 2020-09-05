using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRod : MonoBehaviour
{
    const string ANIM_NAME_TRIGG_CAST_START = "castStart";
    const string ANIM_NAME_TRIGG_CAST_END = "castStop";
    const string ANIM_NAME_TRIGG_REEL_START = "reelStart";
    const string ANIM_NAME_TRIGG_REEL_END = "reelStop";

    [SerializeField]
    private FishingReticle m_fishingReticle = null;

    [SerializeField]
    private Transform m_fishingLineAnchor = null;

    [SerializeField]
    private Transform m_fishingArea = null;

    [SerializeField]
    private Transform m_fishingRodPosAnchor = null;

    private Transform m_reticleTransform = null;
    private LineRenderer m_lineRenderer = null;
    private Animator m_animator = null;
    Renderer[] m_renderers = null;

    public Transform FishingAreaTransform { set { m_fishingArea = value; } }

    public void SetAnimTriggCastStart()
    {
        m_animator.SetTrigger(ANIM_NAME_TRIGG_CAST_START);
    }

    public void SetAnimTriggCastEnd()
    {
        m_animator.SetTrigger(ANIM_NAME_TRIGG_CAST_END);
    }

    public void SetAnimTriggReelStart()
    {
        m_animator.SetTrigger(ANIM_NAME_TRIGG_REEL_START);
    }

    public void SetAnimTriggReelEnd()
    {
        m_animator.SetTrigger(ANIM_NAME_TRIGG_REEL_END);
    }

    public void SetActive(bool isActive)
    {
        foreach (var r in m_renderers)
            r.enabled = isActive;
    }

    private void Awake()
    {
        m_renderers = GetComponentsInChildren<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_fishingReticle == null)
            m_fishingReticle = FindObjectOfType<FishingReticle>();

        m_reticleTransform = m_fishingReticle.transform;
        m_lineRenderer = GetComponent<LineRenderer>();
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        m_lineRenderer.SetPosition(0, m_fishingLineAnchor.position);
        m_lineRenderer.SetPosition(1, m_reticleTransform.position);

        if (m_fishingArea != null)
        {
            Vector3 toFishingAreaNorm = (m_fishingArea.position - transform.position).normalized;
            Vector3 toFishProjNorm = Vector3.ProjectOnPlane(toFishingAreaNorm, Vector3.up);
            transform.forward = toFishProjNorm;
        }

        transform.position = m_fishingRodPosAnchor.position;
    }
}
