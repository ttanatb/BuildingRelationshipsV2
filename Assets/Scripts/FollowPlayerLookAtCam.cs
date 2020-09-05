using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerLookAtCam : MonoBehaviour
{
    [SerializeField]
    Transform m_player;
    Transform m_camera;

    [SerializeField]
    float m_rotOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_player.position;
        transform.rotation =
            Quaternion.AngleAxis(m_rotOffset, Vector3.right) *
            Quaternion.LookRotation((
                m_camera.position - m_player.position).normalized);
    }
}
