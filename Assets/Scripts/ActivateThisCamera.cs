using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ActivateThisCamera : MonoBehaviour
{
    
    [SerializeField] private InputAction m_activateCamInput = null;
    private const int PRIORITY = 100000000;

    // Start is called before the first frame update
    private void Start()
    {
        m_activateCamInput.Enable();
        m_activateCamInput.performed += ActivateCam;
    }

    private void OnDestroy()
    {
        m_activateCamInput.performed -= ActivateCam;
    }

    private void ActivateCam(InputAction.CallbackContext ctx)
    {
        TryGetComponent(out CinemachineVirtualCamera cam);
        cam.Priority = cam.Priority == PRIORITY ? 0 : PRIORITY;
    }
}
