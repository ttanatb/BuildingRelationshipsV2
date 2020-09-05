using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchCameraOnTriggerEnter : MonoBehaviour
{
    [SerializeField]
    LayerMask m_playerLayerMask = 1 << 10;

    [SerializeField]
    CinemachineVirtualCamera m_camera = null;

    [SerializeField]
    int m_targetPrioirty = 15;

    Transform m_cameraTarget = null;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponentInChildren<CinemachineVirtualCamera>();
        m_camera.enabled = false;

        var fishingArea = GetComponentInChildren<FishingArea>();
        m_cameraTarget = fishingArea == null ? null : fishingArea.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_playerLayerMask == (m_playerLayerMask | (1 << other.gameObject.layer)))
        {
            SetWithinProximity(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (m_playerLayerMask == (m_playerLayerMask | (1 << other.gameObject.layer)))
        {
            SetWithinProximity(false);
        }
    }

    public void ResetLookat()
    {
        m_camera.LookAt = m_cameraTarget;
    }

    private void SetWithinProximity(bool isWithinProximity)
    {
        if (m_cameraTarget != null)
            m_camera.LookAt = isWithinProximity ? m_cameraTarget : null;

        m_camera.enabled = isWithinProximity;
        m_camera.Priority = isWithinProximity ? m_targetPrioirty : 0;
    }
}
