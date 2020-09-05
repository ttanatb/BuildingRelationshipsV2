using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class WorldGenerator : MonoBehaviour
{
    const float BASE_BLOCK_SIZE = 5.0f;
    public Vector3 m_blockSize = new Vector3(1, 1, 1);
    public Transform[] m_corners = new Transform[2];
    public Transform[] m_waterMarkers = new Transform[2];

    public GameObject m_baseBlock = null;
    public GameObject[] m_waterPathStraight = null;
    public GameObject[] m_waterPathCorner = null;
    public GameObject m_cliffPrefab = null;
    public GameObject m_cornerCliffPrefab = null;

    public Vector3 m_minPos;

    public List<List<Tile>> m_tiles;
    public GameObject[] m_cliffEdges;

    public float m_varAmp = 2;
    public float m_varFreq = 0.5f;
    public float m_varMinThreshold = 0.3f;
    public Vector2 m_noise;

    [System.Serializable]
    public struct Tile
    {
        public enum State
        {
            Dirt = 0,
            Water = 1,
            Cliff = 2,
            Nothing = 3,
        }

        public enum Dir
        {
            Left = 0,
            Down = 1,
            Right = 2,
            Up = 3,
        }

        public GameObject GameObject;
        public Collider Collider;

        public State CurrState;
        public State Up;
        public State Left;
        public State Down;
        public State Right;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format(
                "Curr({0}) U({1}) L({2}) D({3}) R({4})",
                CurrState, Up, Left, Down, Right));
            return sb.ToString();
        }

        public bool TileIsAdjacentTo(Tile.State up, Tile.State left, Tile.State down, Tile.State right)
        {
            return Up == up &&
                Left == left &&
                Down == down &&
                Right == right;
        }
    }

    public void AssignChildObjects()
    {
        m_corners = Env.Utils.FillVars("Corner", transform);
        m_waterMarkers = Env.Utils.FillVars("WaterMarker", transform);
        SortWaterwayObjs();
    }

    private void SortWaterwayObjs()
    {
        List<Transform> sortedList = new List<Transform>();
        List<Transform> unsortedMarkers = new List<Transform>(m_waterMarkers);
        string first = "Start";
        string end = "End";

        sortedList.Add(GetTransformWithName(first, unsortedMarkers));
        sortedList.Add(GetTransformWithName(end, unsortedMarkers));

        Transform current = sortedList[0];

        while (unsortedMarkers.Count > 0)
        {
            float minDist = float.MaxValue;
            Transform closest = null;
            for (int i = 0; i < unsortedMarkers.Count; i++)
            {
                var m = unsortedMarkers[i];
                float sqrDist = (m.position - current.position).sqrMagnitude;
                if (sqrDist < minDist)
                {
                    minDist = sqrDist;
                    closest = m;
                }
            }

            if (closest == null)
            {
                Debug.LogError("Closest is unexpectedly null");
                return;
            }

            sortedList.Insert(sortedList.Count - 1, closest);
            unsortedMarkers.Remove(closest);
            current = closest;
        }

        m_waterMarkers = sortedList.ToArray();
    }

    private Transform GetTransformWithName(string name, List<Transform> transforms)
    {
        for (int i = 0; i < transforms.Count; i++)
        {
            var m = transforms[i];
            if (m.name.Contains(name))
            {
                transforms.Remove(m);
                return m;
            }
        }
        return null;
    }

    public void BuildWaterway()
    {
        for (int m = 1; m < m_waterMarkers.Length; m++)
        {
            var p = m_waterMarkers[m - 1];
            var c = m_waterMarkers[m];
            Ray ray = new Ray(p.position, (c.position - p.position).normalized);
            float maxDist = (c.position - p.position).magnitude;

            for (int i = 0; i < m_tiles.Count; i++)
            {
                var row = m_tiles[i];
                for (int j = 0; j < row.Count; j++)
                {
                    //Debug.Log("curr: row: " + i + " col: " + j);
                    var tile = row[j];
                    if (tile.CurrState != Tile.State.Dirt)
                        continue;

                    var collider = tile.Collider;
                    Debug.Log(tile.CurrState);
                    if (collider.Raycast(ray, out RaycastHit hitInfo, maxDist))
                    {
                        tile.CurrState = Tile.State.Water;
                        //Debug.Log("row: " + i + " col: " + j + " is now w");
                        DestroyImmediate(tile.GameObject);
                    }

                    row[j] = tile;
                }
            }
        }

        for (int i = 0; i < m_tiles.Count; i++)
        {
            var row = m_tiles[i];
            for (int j = 0; j < row.Count; j++)
            {
                row[j] = FillAllAdjData(i, j, m_tiles);
                Debug.Log(row[j]);
            }
        }

        for (int i = 0; i < m_tiles.Count; i++)
        {
            var row = m_tiles[i];
            for (int j = 0; j < row.Count; j++)
            {
                var tile = row[j];
                if (tile.CurrState == Tile.State.Dirt)
                    continue;

                float orientation = 0.0f;
                if (IsStraightAway(tile, out orientation))
                    tile.GameObject =
                        InstantiateAndSetPosRot(i, j, orientation, m_waterPathStraight);
                else if (IsCorner(tile, out orientation))
                    tile.GameObject =
                        InstantiateAndSetPosRot(i, j, orientation, m_waterPathCorner);

                row[j] = tile;
            }
        }
    }

    private GameObject InstantiateAndSetPosRot(int i, int j, float orientation, GameObject prefab, bool isUsingAnchor = true, bool isUsingMeshScale = true)
    {
        Vector3 offset = new Vector3(
            m_blockSize.x * BASE_BLOCK_SIZE * i, 0.0f,
            m_blockSize.z * BASE_BLOCK_SIZE * j);
        var obj = Instantiate(
            prefab, m_minPos + offset,
            Quaternion.identity, transform);

        var rotAnchhor = isUsingAnchor ? obj.transform.GetChild(0) : obj.transform;
        rotAnchhor.rotation = Quaternion.AngleAxis(orientation, Vector3.up);

        var scaleAnchhor = isUsingMeshScale ? obj.transform.GetChild(0).GetChild(0) : obj.transform;
        Vector3 newScale = scaleAnchhor.localScale;
        newScale.x *= m_blockSize.x;
        newScale.y *= m_blockSize.y;
        newScale.z *= m_blockSize.z;
        scaleAnchhor.localScale = newScale;

        obj.isStatic = true;
        return obj;
    }

    private GameObject InstantiateAndSetPosRot(int i, int j, float orientation, GameObject[] prefabs, bool isUsingAnchor = true, bool isUsingMeshScale = true)
    {
        var prefab = prefabs[Random.Range(
            0, prefabs.Length)];
        return InstantiateAndSetPosRot(i, j, orientation, prefab);
    }

    private bool IsStraightAway(Tile tile, out float orientaiton)
    {
        orientaiton = 0.0f;
        if (tile.TileIsAdjacentTo(
            /*U=*/Tile.State.Water,
            /*L=*/Tile.State.Dirt,
            /*D=*/Tile.State.Water,
            /*R=*/Tile.State.Dirt))
        {
            orientaiton = 90.0f;
            return true;
        }
        if (tile.TileIsAdjacentTo(
            /*U=*/Tile.State.Dirt,
            /*L=*/Tile.State.Water,
            /*D=*/Tile.State.Dirt,
            /*R=*/Tile.State.Water))
        {
            return true;
        }


        return false;
    }

    private bool IsCorner(Tile tile, out float orientaiton)
    {
        orientaiton = 0.0f;
        if (tile.TileIsAdjacentTo(
            /*U=*/Tile.State.Water,
            /*L=*/Tile.State.Water,
            /*D=*/Tile.State.Dirt,
            /*R=*/Tile.State.Dirt))
        {
            return true;
        }
        if (tile.TileIsAdjacentTo(
            /*U=*/Tile.State.Dirt,
            /*L=*/Tile.State.Water,
            /*D=*/Tile.State.Water,
            /*R=*/Tile.State.Dirt))
        {
            orientaiton = 270.0f;
            return true;
        }
        orientaiton = 0.0f;
        if (tile.TileIsAdjacentTo(
            /*U=*/Tile.State.Dirt,
            /*L=*/Tile.State.Dirt,
            /*D=*/Tile.State.Water,
            /*R=*/Tile.State.Water))
        {
            orientaiton = 180.0f;
            return true;
        }
        if (tile.TileIsAdjacentTo(
            /*U=*/Tile.State.Water,
            /*L=*/Tile.State.Dirt,
            /*D=*/Tile.State.Dirt,
            /*R=*/Tile.State.Water))
        {
            orientaiton = 90.0f;
            return true;
        }

        return false;
    }

    private Tile FillAllAdjData(int row, int col, List<List<Tile>> grid)
    {
        var tile = m_tiles[row][col];
        tile.Left = GetAdj(row, col, grid, Tile.Dir.Left);
        tile.Down = GetAdj(row, col, grid, Tile.Dir.Down);
        tile.Right = GetAdj(row, col, grid, Tile.Dir.Right);
        tile.Up = GetAdj(row, col, grid, Tile.Dir.Up);
        return tile;
    }

    private Tile.State GetAdj(int row, int col, List<List<Tile>> grid, Tile.Dir dir)
    {
        int offsetRow = 0;
        int offsetCol = 0;

        switch (dir)
        {
            case Tile.Dir.Left:
                offsetCol = -1;
                break;
            case Tile.Dir.Down:
                offsetRow = 1;
                break;
            case Tile.Dir.Right:
                offsetCol = 1;
                break;
            case Tile.Dir.Up:
                offsetRow = -1;
                break;
        }

        int adjRow = row + offsetRow;
        if (adjRow < 0 || adjRow >= grid.Count)
            return Tile.State.Water;

        int adjCol = col + offsetCol;
        if (adjCol < 0 || adjCol >= grid[adjRow].Count)
            return Tile.State.Water;

        return grid[adjRow][adjCol].CurrState;
    }

    public void GenerateCliff()
    {
        foreach (GameObject o in m_cliffEdges)
            DestroyImmediate(o);

        List<GameObject> cliffs = new List<GameObject>();

        //go through top row
        var topRow = m_tiles[0];
        var bottomRow = m_tiles[m_tiles.Count - 1];
        GenerateCliffHelper(cliffs, topRow, -1, 270, m_cliffPrefab);
        GenerateCliffHelper(cliffs, bottomRow, m_tiles.Count, 90.0f, m_cliffPrefab);

        for (int i = 0; i < m_tiles.Count; i++)
        {
            var row = m_tiles[i];
            //first 
            cliffs.Add(InstantiateAndSetPosRot(i, -1, 180, m_cliffPrefab));

            //last
            cliffs.Add(InstantiateAndSetPosRot(i, row.Count, 0, m_cliffPrefab));
        }

        cliffs.Add(InstantiateAndSetPosRot(-1, -1,
            180, m_cornerCliffPrefab));
        cliffs.Add(InstantiateAndSetPosRot(m_tiles.Count, -1,
            90, m_cornerCliffPrefab));
        cliffs.Add(InstantiateAndSetPosRot(-1, m_tiles[0].Count,
            270, m_cornerCliffPrefab));
        cliffs.Add(InstantiateAndSetPosRot(m_tiles.Count, m_tiles[m_tiles.Count - 1].Count,
            0, m_cornerCliffPrefab));
        m_cliffEdges = cliffs.ToArray();
    }

    private void GenerateCliffHelper(List<GameObject> cliffs, List<Tile> row, int index, float orientation, GameObject cliffPrefab)
    {
        for (int j = 0; j < row.Count; j++)
        {
            cliffs.Add(InstantiateAndSetPosRot(index, j, orientation, cliffPrefab));
        }
    }


    public void GenerateBlocks()
    {

        if (m_tiles == null)
        {
            m_tiles = new List<List<Tile>>();
        }
        else
        {
            foreach (List<Tile> l in m_tiles)
                l.Clear();
            m_tiles.Clear();
        }
        var prevGenObjs = Env.Utils.FillVars(m_baseBlock.name, transform);
        foreach (Transform o in prevGenObjs)
            DestroyImmediate(o.gameObject);

        if (m_waterPathStraight?.Length != 0)
            foreach (GameObject prefab in m_waterPathStraight)
            {
                var objs = Env.Utils.FillVars(prefab.name, transform);
                foreach (Transform o in objs)
                    DestroyImmediate(o.gameObject);
            }

        if (m_waterPathStraight?.Length != 0)
            foreach (GameObject prefab in m_waterPathCorner)
            {
                var objs = Env.Utils.FillVars(prefab.name, transform);
                foreach (Transform o in objs)
                    DestroyImmediate(o.gameObject);
            }

        if (m_cliffPrefab != null)
        {
            var objs = Env.Utils.FillVars(m_cliffPrefab.name, transform);
            foreach (Transform o in objs)
                DestroyImmediate(o.gameObject);
        }

        if (m_corners.Length != 2)
        {
            Debug.LogError("Expected to only have 2 corners");
            return;
        }

        m_minPos =
            new Vector3(Mathf.Infinity, m_corners[0].position.y, Mathf.Infinity);
        Vector3 maxPos =
            new Vector3(-Mathf.Infinity, m_corners[0].position.y, -Mathf.Infinity);

        foreach (Transform t in m_corners)
        {
            if (Mathf.Abs(t.position.y - m_minPos.y) > Mathf.Epsilon)
            {
                Debug.LogWarning("Corners not at same height? Diff: "
                    + Mathf.Abs(t.position.y - m_minPos.y));
            }

            if (t.position.x < m_minPos.x)
                m_minPos.x = t.position.x;
            if (t.position.x > maxPos.x)
                maxPos.x = t.position.x;

            if (t.position.z < m_minPos.z)
                m_minPos.z = t.position.z;
            if (t.position.z > maxPos.z)
                maxPos.z = t.position.z;
        }

        // Debug.Log(string.Format("minPos: {0}, maxPos: {1}", m_minPos, maxPos));

        Vector2Int baseFieldSize = new Vector2Int(
            Mathf.RoundToInt(((maxPos.x - m_minPos.x) / BASE_BLOCK_SIZE) * m_blockSize.x),
            Mathf.RoundToInt((maxPos.z - m_minPos.z) / (BASE_BLOCK_SIZE * m_blockSize.z)));


        for (int i = 0; i < baseFieldSize.x; i++)
        {
            m_tiles.Add(new List<Tile>());
            var tileRow = m_tiles[i];

            for (int j = 0; j < baseFieldSize.y; j++)
            {
                var obj = InstantiateAndSetPosRot(i, j, 0.0f, m_baseBlock, false, false);
                var tile = new Tile
                {
                    GameObject = obj,
                    Collider = obj.GetComponent<Collider>(),
                    CurrState = Tile.State.Dirt,
                };
                tileRow.Add(tile);
            }
        }

        //m_noise = new Vector2(Random.value, Random.value);
        //int ii = 0;
        //int offset = -1;
        //int maxExtrusion = Mathf.FloorToInt(
        //        m_varAmp *
        //        Mathf.Clamp((1 - m_varMinThreshold) / (1.0f - m_varMinThreshold), 0.0f, 1.0f));
        //Debug.Log("Max Extrusion: " + maxExtrusion);
        //if (maxExtrusion > 1)
        //{
        //    Debug.LogError("max extrusion above 1 not supported pls this is game jam");
        //}
        //var upperRow = new List<Tile>();
        //for (int j = 0; j < baseFieldSize.y; j++)
        //{
        //    float noiseVal = Mathf.PerlinNoise(m_noise.x + j * m_varFreq, m_noise.y + ii * m_varFreq);

        //    int extrusionCountvar = Mathf.FloorToInt(
        //        m_varAmp *
        //        Mathf.Clamp((noiseVal - m_varMinThreshold) / (1.0f - m_varMinThreshold), 0.0f, 1.0f));

        //    Debug.Log(extrusionCountvar);
        //    if (extrusionCountvar == 0)
        //    {
        //        var tile = new Tile
        //        {
        //            GameObject = null,
        //            Collider = null,
        //            CurrState = Tile.State.Nothing,
        //        };
        //        upperRow.Add(tile);
        //    }
        //    for (int e = 0; e < extrusionCountvar; e++)
        //    {
        //        var obj = InstantiateAndSetPosRot(ii + offset, j, 0.0f, m_baseBlock, false, false);
        //        var tile = new Tile
        //        {
        //            GameObject = obj,
        //            Collider = obj.GetComponent<Collider>(),
        //            CurrState = Tile.State.Dirt,
        //        };
        //        upperRow.Add(tile);
        //    }
        //}
        //m_tiles.Insert(0, upperRow);

        BuildWaterway();
        GenerateCliff();
    }

    private void OnDrawGizmos()
    {
        Vector3 offset = Vector3.up * 5.0f;
        for (int i = 1; i < m_waterMarkers.Length; i++)
        {
            var p = m_waterMarkers[i - 1];
            var c = m_waterMarkers[i];
            Gizmos.DrawLine(p.position + offset, c.position + offset);
        }
    }
}
