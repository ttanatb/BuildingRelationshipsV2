using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RockyBeachData", menuName = "br/RockyBeachData", order = 1)]
public class RockyBeachData : ScriptableObject
{
    [field: SerializeField]
    public List<RockObject> Objects { get; set; }
}

[System.Serializable]
public struct RockObject
{
    [field: SerializeField]
    public Vector3 Position { get; set; }
    
    [field: SerializeField]
    public Quaternion Rotation { get; set; }
    
    [field: SerializeField]
    public Vector3 Scale { get; set; }
    
    [field: SerializeField]
    public Mesh Mesh { get; set; }
}
