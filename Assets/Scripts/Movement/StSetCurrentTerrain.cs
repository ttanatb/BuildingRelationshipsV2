using Movement.SO;
using UnityEngine;

public class StSetCurrentTerrain : MonoBehaviour
{
    [SerializeField] private GetCurrentTerrainEvent m_getCurrentTerrainEvent = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        TryGetComponent(out Terrain terrain);
        m_getCurrentTerrainEvent.Invoke(terrain);
    }
}
