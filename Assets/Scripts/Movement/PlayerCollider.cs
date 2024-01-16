using System;
using System.Collections.Generic;
using System.Linq;
using Movement.SO;
using Movement.Structs;
using NaughtyAttributes;
using Sound.SerializedDict;
using Sound.Struct;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCollider : MonoBehaviour
{
    private const int CONTACT_POINT_MAX_COUNT = 48;
    private const int DEBUG_LIST_CAP = 48;

    [SerializeField] private PlayOneShotAudioEvent m_playOneShotAudioEvent = null;
    [SerializeField] private TextureToAudioClipsDict m_terrainTextureToAudioClips = null;
    
    [SerializeField]
    private List<DebugContactData> m_debugContactData = new List<DebugContactData>();
    
    [SerializeField]
    private ContactPoint[] m_collisionPoints = new ContactPoint[CONTACT_POINT_MAX_COUNT];

    [Tag] [SerializeField] private string m_terrainTag = "";
    [SerializeField] private GetCurrentTerrainEvent m_getCurrentTerrainEvent = null;
    
    private TerrainData m_terrainData = null;
    private Vector3 m_terrainPos = Vector3.zero;

    [SerializeField] private float m_debugRadius = 0.2f;
    [SerializeField] private Color m_debugColorRecent = Color.red;
    [SerializeField] private Color m_debugColorOldest = Color.black;
    [SerializeField] private Color m_debugColorImpulse = Color.cyan;
    [SerializeField] private Color m_debugColorVelocity = Color.magenta;
    [SerializeField] private Color m_debugColorTerrainHeight = Color.yellow;
    [SerializeField] private Color m_debugColorTerrainNormal = Color.blue;

    private void Awake()
    {
        m_getCurrentTerrainEvent.Event.AddListener(SetTerrainData);
    }

    private void OnDestroy()
    {
        m_getCurrentTerrainEvent.Event.RemoveListener(SetTerrainData);
    }

    private void SetTerrainData(Terrain terrain)
    {
        m_terrainData = terrain.terrainData;
        m_terrainPos = terrain.transform.position;
    }

    private Vector2 WorldPosToTerrainCoordinates(Vector3 worldPos)
    {
        var localPos = worldPos - m_terrainPos;
        return new Vector2()
        {
            x = (localPos.x / m_terrainData.size.x), 
            y = (localPos.z / m_terrainData.size.z)
        };
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collided with {collision.gameObject}");

        bool isTerrain = collision.gameObject.CompareTag(m_terrainTag);
        

        int count = collision.GetContacts(m_collisionPoints);
        var averageContactPoint = Vector3.zero;
        for (int i = 0; i < count; i++)
        {
            var collisionPoint = m_collisionPoints[i];
            var contactData = new DebugContactData()
            {
                Position = collisionPoint.point,
                Normal = collisionPoint.normal,
                Impulse = collision.impulse,
                RelativeVelocity = collision.relativeVelocity,
            };

            if (isTerrain)
            {
                contactData.TerrainData = ProcessTerrainCollision(collisionPoint);
            }
                
            m_debugContactData.Add(contactData);
            averageContactPoint += collisionPoint.point;
        }
        averageContactPoint /= (float)count;

        if (isTerrain)
        {
            PlayAudioClipAtTerrainPoint(averageContactPoint);
        }
        
        if (m_debugContactData.Count > DEBUG_LIST_CAP)
        {
            m_debugContactData.RemoveRange(0, m_debugContactData.Count - DEBUG_LIST_CAP);
        }
    }

    private TerrainCollisionData ProcessTerrainCollision(ContactPoint collisionPoint)
    {
        var coordinates =  WorldPosToTerrainCoordinates(collisionPoint.point);

        var res = new TerrainCollisionData()
        {
            Normal = m_terrainData.GetInterpolatedNormal(coordinates.x, coordinates.y),
            Height = m_terrainData.GetInterpolatedHeight(coordinates.x, coordinates.y) + m_terrainPos.y,
        };
                

        return res;
    }

    private void PlayAudioClipAtTerrainPoint(Vector3 contactPoint)
    {
        var coordinates =  WorldPosToTerrainCoordinates(contactPoint);

        float[,,] alphaMap = m_terrainData.GetAlphamaps(
            (int)(coordinates.x * m_terrainData.alphamapWidth),
            (int)(coordinates.y * m_terrainData.alphamapHeight), 1, 1);
        for (int i = 0; i < alphaMap.Length; i++)
        {
            float visibility = Mathf.Clamp01(alphaMap[0, 0, i]);
            if (visibility < 0.2)
                continue;

            
            var texture = m_terrainData.terrainLayers[i].diffuseTexture;
            Debug.Log($"Texture {texture} at visibility {visibility}");
            Assert.IsTrue(m_terrainTextureToAudioClips.ContainsKey(texture));
            var clips = m_terrainTextureToAudioClips[texture];
            var clip = clips.GetNext(clips.StepClips);
            m_playOneShotAudioEvent.Invoke(new OneShotAudioData()
            {
                Clip = clip,
                Pitch = 1.0f,
                Volume = visibility * 0.12f,
            });
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < m_debugContactData.Count; i++)
        {
            Gizmos.color = Color.Lerp(m_debugColorOldest, m_debugColorRecent, i / (float)DEBUG_LIST_CAP);

            var contact = m_debugContactData[i];
            Gizmos.DrawSphere(contact.Position, m_debugRadius);
            Gizmos.DrawLine(contact.Position, contact.Position + contact.Normal);

            Gizmos.color = m_debugColorImpulse;
            Gizmos.DrawLine(contact.Position, contact.Position + contact.Impulse);
                
            Gizmos.color = m_debugColorVelocity;
            Gizmos.DrawLine(contact.Position, contact.Position + contact.RelativeVelocity);

            if (contact.TerrainData == null) return;

            var terrainData = contact.TerrainData.Value;
            var terrainPos = new Vector3()
            {
                x = contact.Position.x,
                y = terrainData.Height,
                z = contact.Position.z,
            };
            Gizmos.color = m_debugColorTerrainHeight;
            Gizmos.DrawSphere(terrainPos, m_debugRadius);

            Gizmos.color = m_debugColorTerrainNormal;
            Gizmos.DrawLine(terrainPos, terrainPos + terrainData.Normal);
        }
    }
}
