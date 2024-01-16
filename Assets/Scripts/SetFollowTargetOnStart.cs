using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class SetFollowTargetOnStart : MonoBehaviour
{
    [SerializeField] private Transform m_target;
    
    // Start is called before the first frame update
    public void Start()
    {
        TryGetComponent(out CinemachineVirtualCamera cam);
        cam.Follow = m_target;
    }
}
