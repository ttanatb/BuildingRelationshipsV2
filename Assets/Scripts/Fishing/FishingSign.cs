using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FishingSign : InteractiblePropject
{
    [SerializeField]
    private bool m_interactible = true;

    [SerializeField]
    private Transform m_playerAnchor = null;

    private SwitchCameraOnTriggerEnter m_camControls = null;
    public bool Interactible
    {
        get { return m_interactible; }
        set { m_interactible = value; }
    }

    public Transform FishingCamera { get; private set; }
    public FishingArea FishingArea { get; private set; }
    public Transform PlayerAnchor { get { return m_playerAnchor; } }

    public void ResetLookat()
    {
        m_camControls.ResetLookat();
    }

    protected override void Start()
    {
        base.Start();
        FishingCamera = GetComponentInChildren<CinemachineVirtualCamera>().transform;
        FishingArea = GetComponentInChildren<FishingArea>();
        m_camControls = GetComponent<SwitchCameraOnTriggerEnter>();
    }
}
