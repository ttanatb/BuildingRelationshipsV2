using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class UnderwaterPPController : MonoBehaviour
{
    [SerializeField]
    private float m_waterLevelPos = 3.0f;

    [SerializeField]
    private float m_startLerpThresh = 1.0f;

    [SerializeField]
    private PostProcessVolume m_underwaterVolume = null;

    // Update is called once per frame
    void Update()
    {


        if (transform.position.y < m_waterLevelPos + m_startLerpThresh)
        {
            float r = (transform.position.y - m_waterLevelPos) / m_startLerpThresh;
            m_underwaterVolume.weight = Mathf.Lerp(1.0f, 0.0f, r);
        }
        else
        {
            m_underwaterVolume.weight = 0.0f;
        }
    }
}
