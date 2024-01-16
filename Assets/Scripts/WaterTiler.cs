using UnityEngine;

public class WaterTiler : MonoBehaviour
{
    string[] TEXT_NAMES = { "_SurfaceNoise", "_SurfaceDistortion" };
    Material m_material = null;

    [SerializeField]
    Vector2 m_baseScale = new Vector2(10, 10);

    int[] m_proertyIds = null;

    void Start()
    {
        var renderer = GetComponent<Renderer>();
        m_material = renderer.material;

        m_proertyIds = new int[TEXT_NAMES.Length];
        for (int i = 0; i < TEXT_NAMES.Length; i++)
        {
            m_proertyIds[i] = Shader.PropertyToID(TEXT_NAMES[i]);
        }
    }

    void Update()
    {
        Vector3 localScale = transform.localScale;
        Vector2 textScale = new Vector2(localScale.x / m_baseScale.x, localScale.y / m_baseScale.y);

        // foreach (int s in m_proertyIds)
            // m_material.SetTextureScale(s, textScale);
    }
}
