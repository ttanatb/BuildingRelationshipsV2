using NaughtyAttributes;
using UnityEngine;
using Utilr.Utility;

public class FishingRod : MonoBehaviour
{
    [SerializeField] private Animator m_animator = null;

    [SerializeField] [AnimatorParam("m_animator")]
    private int m_paramStartCast = 0;

    [SerializeField] [AnimatorParam("m_animator")]
    private int m_paramStopCast = 0;
    
    [SerializeField] [AnimatorParam("m_animator")]
    private int m_paramStartReel = 0;
    
    [SerializeField] [AnimatorParam("m_animator")]
    private int m_paramStopReel = 0;


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
    Renderer[] m_renderers = null;

    public Transform FishingAreaTransform { set { m_fishingArea = value; } }

    private void ResetTriggers()
    {
        m_animator.ResetTrigger(m_paramStartCast);
        m_animator.ResetTrigger(m_paramStopCast);
        m_animator.ResetTrigger(m_paramStartReel);
        m_animator.ResetTrigger(m_paramStopReel);
    }

    public void SetAnimTriggCastStart()
    {
        ResetTriggers();
        m_animator.SetTrigger(m_paramStartCast);
    }

    public void SetAnimTriggCastEnd()
    {
        ResetTriggers();
        m_animator.SetTrigger(m_paramStopCast);
    }

    public void SetAnimTriggReelStart()
    {
        ResetTriggers();
        m_animator.SetTrigger(m_paramStartReel);
    }

    public void SetAnimTriggReelEnd()
    {
        ResetTriggers();
        m_animator.SetTrigger(m_paramStopReel);
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
