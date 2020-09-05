using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RandomHouseGenerator : MonoBehaviour
{
    public GameObject[] FrontWallSelection;
    public GameObject[] WallSelection;
    public GameObject[] RoofSelection;

    public Transform m_frontWall;
    public Transform[] m_otherWalls;
    public Transform[] m_roofs;

    public void GenerateHouse()
    {
        m_frontWall = GenerateHouseHelper(FrontWallSelection, m_frontWall);
        for (int i = 0; i < m_otherWalls.Length; i++)
        {
            m_otherWalls[i] = GenerateHouseHelper(WallSelection, m_otherWalls[i]);
        }

        for (int i = 0; i < m_roofs.Length; i++)
        {
            m_roofs[i] = GenerateHouseHelper(RoofSelection, m_roofs[i]);
        }
    }

    private Transform GenerateHouseHelper(GameObject[] selection, Transform wall)
    {
        int i = Random.Range(0, selection.Length);
        var obj = Instantiate(selection[i],
            wall.position, wall.rotation, transform);
        obj.transform.localScale = wall.localScale;
        DestroyImmediate(wall.gameObject);
        return obj.transform;
    }

    public void AssignChildObjects()
    {
        m_frontWall = Env.Utils.FillVar("wallDoor", transform);
        m_roofs = Env.Utils.FillVars("roof", transform);
        m_otherWalls = Env.Utils.FillVars("wall", transform, "wallDoor");
    }

    private void Start()
    {
        enabled = false;
    }
}
