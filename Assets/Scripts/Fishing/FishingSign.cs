using System;
using UnityEngine;
using Cinemachine;
using Utilr.SoGameEvents;

public class FishingSign : InteractablePropject
{
    [SerializeField]
    private Transform m_playerAnchor = null;
    [SerializeField] private LayerMask m_waterLayerMask = 1 << 4;
    [SerializeField] private FishingReticle m_fishingReticle = null;

    public bool Interactable
    {
        get => m_interactable;
        set => m_interactable = value;
    }

    [SerializeField]
    private CinemachineVirtualCamera m_aimCam = null;

    public void ResetReticlePosition()
    {
        var aimCamTransform = m_aimCam.transform;
        var ray = new Ray(aimCamTransform.position, aimCamTransform.forward);

        if (Physics.Raycast(ray, out var hitInfo, float.MaxValue, m_waterLayerMask))
        {
            m_fishingReticle.SetStartingTransform(hitInfo.point + Vector3.up * 0.1f, hitInfo.normal);
        }
    }
    
    public FishingArea FishingArea { get; private set; }
    public Transform PlayerAnchor => m_playerAnchor;

    [field: SerializeField]
    public SoGameEvent AimCamEvent { get; set; }
    
    [field: SerializeField]
    public SoGameEvent ReelCamEvent { get; set; }


    protected override void Start()
    {
        base.Start();
        FishingArea = GetComponentInChildren<FishingArea>();
    }
}
