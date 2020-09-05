using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FishSpawnCountPair
{
    public GameObject Prefab;
    public int SpawnCount;
}

public class FishingArea : MonoBehaviour
{
    [SerializeField]
    private GameObject[] m_fishPrefabs = null;

    [SerializeField]
    private float m_spawnRadius = 1.0f;

    [SerializeField]
    private float m_maxWanderRadius = 5.0f;

    [SerializeField]
    TerrainCollider m_terrainCollider = null;

    [SerializeField]
    FishingReticle m_fishingReticle = null;

    [SerializeField]
    Fish[] m_fishes = null;

    [SerializeField]
    NavmeshWanderer[] m_wanderers = null;

    [SerializeField]
    FishSpawnCountPair[] m_fishSpawnTable = null;


    // Start is called before the first frame update
    void Start()
    {
        if (m_terrainCollider == null)
            m_terrainCollider = FindObjectOfType<TerrainCollider>();

        if (m_fishingReticle == null)
            m_fishingReticle = FindObjectOfType<FishingReticle>();

        List<Fish> fishes = new List<Fish>();
        List<NavmeshWanderer> wanderers = new List<NavmeshWanderer>();
        foreach (var pair in m_fishSpawnTable)
        {
            for (int i = 0; i < pair.SpawnCount; i++)
            {
                Vector3 pos = transform.position + (m_spawnRadius * UnityEngine.Random.insideUnitSphere);
                pos.y = 0.0f;

                Vector3 transformPos = transform.position;
                transformPos.y = 0.0f;

                var fishObj = Instantiate(pair.Prefab, pos, Quaternion.identity, transform);
                var wanderer = fishObj.GetComponent<NavmeshWanderer>();
                wanderer.TerrainCollider = m_terrainCollider;
                wanderer.MaxRadius = m_maxWanderRadius;
                wanderer.StartPos = transformPos;
                wanderers.Add(wanderer);
                fishes.Add(fishObj.GetComponent<Fish>());
            }
        }

        m_fishes = fishes.ToArray();
        m_wanderers = wanderers.ToArray();
    }

    public void ActivateFish(bool shouldActivate)
    {
        foreach (NavmeshWanderer w in m_wanderers)
        {
            w.SeekTarget = shouldActivate ? m_fishingReticle : null;
        }
    }

    public void RemoveFish(Fish fish)
    {
        fish.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, m_spawnRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, m_maxWanderRadius);
    }
}
