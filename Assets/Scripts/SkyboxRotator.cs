using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [SerializeField] private Material m_skyboxMat = null;
    [SerializeField] private float m_rotationRate = 0.2f;

    private float m_currRotation = 0.0f;
    private static readonly int ROTATION_ID = Shader.PropertyToID("_Rotation");

    // Update is called once per frame
    void Update()
    {
        m_currRotation+= m_rotationRate * Time.deltaTime;
        m_skyboxMat.SetFloat(ROTATION_ID, m_currRotation);
    }
}
