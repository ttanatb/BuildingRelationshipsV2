using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Util;

public class RockyBeachGenerator : MonoBehaviour
{
    [SerializeField] [Expandable]
    private RockyBeachData m_data = null;

    [MinMaxSlider(-2.0f, 2.0f)]
    [SerializeField]
    private Vector2 m_translationRange = new Vector2(-2f, 2f);

    [MinMaxSlider(0.001f, 5.0f)]
    [SerializeField]
    private Vector2 m_scaleRange = new Vector2(0.1f, 1.1f);

    [Button]
    private void WriteToData()
    {
        int childCount = transform.childCount;
        m_data.Objects = new List<RockObject>(childCount);
        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            m_data.Objects.Add(new RockObject()
            {
                Position = child.position,
                Rotation = child.rotation,
                Scale = child.localScale,
                Mesh = child.GetComponent<MeshFilter>().sharedMesh,
            });
        }
    }
    
    [Button]
    private void ReadFromData()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < m_data.Objects.Count; i++)
        {
            if (i >= childCount)
                return;
            
            var child = transform.GetChild(i);
            var data = m_data.Objects[i];
            child.position = data.Position;
            child.rotation = data.Rotation;
            child.localScale = data.Scale;
            child.GetComponent<MeshFilter>().sharedMesh = data.Mesh;
        }
    }

    [Button]
    private void AddRigidBody()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            if (!child.gameObject.GetComponent<Rigidbody>())
                child.gameObject.AddComponent<Rigidbody>();
        }
    }
    
    [Button]
    private void RandomizeTransform()
    {
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            var child = transform.GetChild(i);
            child.Translate(new Vector3()
            {
                x = m_translationRange.RandomInRange(),
                y = Mathf.Abs(m_translationRange.RandomInRange()),
                z = m_translationRange.RandomInRange(),
            });
            child.Rotate(new Vector3()
            {
                x = m_translationRange.RandomInRange(),
                y = m_translationRange.RandomInRange(),
                z = m_translationRange.RandomInRange(),
            }.normalized, Random.Range(0, 360f));
            child.localScale = new Vector3()
            {
                x = m_scaleRange.RandomInRange(),
                y = m_scaleRange.RandomInRange(),
                z = m_scaleRange.RandomInRange(),
            };
            if (!child.gameObject.GetComponent<Rigidbody>())
                child.gameObject.AddComponent<Rigidbody>();
        }
    }
}
