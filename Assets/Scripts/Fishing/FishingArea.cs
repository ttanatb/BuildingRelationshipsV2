using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct FishSpawnCountPair
{
    [field: SerializeField]
    public GameObject Prefab { get; set; }
    [field: SerializeField]
    public int SpawnCount { get; set; }
}

public class FishingArea : MonoBehaviour
{
    [SerializeField]
    private float m_spawnRadius = 1.0f;

    [SerializeField]
    private float m_maxWanderRadius = 5.0f;

    [SerializeField] private TerrainCollider m_terrainCollider = null;
    [SerializeField] private FishingReticle m_fishingReticle = null;
    [SerializeField] private Fish[] m_fishes = null;
    [SerializeField] private NavmeshWanderer[] m_wanderers = null;
    [SerializeField] private FishSpawnCountPair[] m_fishSpawnTable = null;


    // Start is called before the first frame update
    private void Start()
    {
        if (m_terrainCollider == null)
            m_terrainCollider = FindObjectOfType<TerrainCollider>();

        if (m_fishingReticle == null)
            m_fishingReticle = FindObjectOfType<FishingReticle>();

        var fishes = new List<Fish>();
        var wanderers = new List<NavmeshWanderer>();
        var fishingAreaTransform = transform;
        foreach (var pair in m_fishSpawnTable)
        {
            for (int i = 0; i < pair.SpawnCount; i++)
            {
                var position = fishingAreaTransform.position;
                var fishPos = position + (m_spawnRadius * UnityEngine.Random.insideUnitSphere);
                NavMesh.SamplePosition(fishPos, out var hit, Mathf.Infinity, NavMesh.AllAreas);

                fishPos = hit.position;

                var fishObj = Instantiate(pair.Prefab, fishPos, Quaternion.identity, fishingAreaTransform);
                var wanderer = fishObj.GetComponent<NavmeshWanderer>();
                wanderer.TerrainCollider = m_terrainCollider;
                wanderer.MaxRadius = m_maxWanderRadius;
                wanderer.StartPos = fishPos;
                wanderers.Add(wanderer);
                fishes.Add(fishObj.GetComponent<Fish>());
            }
        }

        m_fishes = fishes.ToArray();
        m_wanderers = wanderers.ToArray();
    }

    public void ActivateFish(bool shouldActivate)
    {
        foreach (var w in m_wanderers)
        {
            w.SeekTarget = shouldActivate ? m_fishingReticle : null;
        }
    }

    public static void RemoveFish(Fish fish)
    {
        fish.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, m_spawnRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, m_maxWanderRadius);
    }
}
