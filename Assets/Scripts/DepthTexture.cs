using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthTexture : MonoBehaviour
{

    Camera m_camera = null;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_camera.depthTextureMode = DepthTextureMode.DepthNormals;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
