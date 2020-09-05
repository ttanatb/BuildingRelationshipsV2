#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;
using UnityEditor;

[ExecuteInEditMode]
public class LoadItemDatabase : MonoBehaviour
{
    [SerializeField]
    private string m_inputFilePath = "Assets/Resources/items.tsv";

    [SerializeField]
    private string m_outputItemFilePath = "Assets/ScriptableObjects/items.asset";

    [SerializeField]
    private string m_outputFishFilePath = "Assets/ScriptableObjects/fish.asset";

    [SerializeField]
    private string m_spritesfilePath = "Assets/Textures/Items";

    private Dictionary<string, Sprite> m_sprites = null;

    public void ReadCSV()
    {
        LoadSprites();

        StreamReader reader = new StreamReader(m_inputFilePath);

        // Skip first 2 lines.
        reader.ReadLine();
        reader.ReadLine();

        CollectibleSO itemSO = (CollectibleSO)ScriptableObject.CreateInstance(typeof(CollectibleSO));
        FishDatabaseSO fishSO = (FishDatabaseSO)ScriptableObject.CreateInstance(typeof(FishDatabaseSO));
        List<CollectibleItem> items = new List<CollectibleItem>();
        List<FishStats> fishStats = new List<FishStats>();
        try
        {
            while (true)
            {
                if (reader.EndOfStream) break;

                string line = reader.ReadLine();
                string[] splitLine = line.Split('\t');

                var currItem = new CollectibleItem
                {
                    id = (CollectibleItem.ItemID)ConvertToInt(splitLine[0]),
                    displayName = ConvertToString(splitLine[1]),
                    displayNamePlural = ConvertToString(splitLine[2], ConvertToString(splitLine[1])),
                    description = ConvertToString(splitLine[3]),
                    sprite = ConvertToSprite(splitLine[4]),
                };

                if (splitLine[5] != "")
                {
                    FishStats fish = new FishStats
                    {
                        id = currItem.id,
                        DecayRate = 1.0f / ConvertToFloat(splitLine[5]),
                        CompletionRate = 1.0f / ConvertToFloat(splitLine[6]),
                        JumpIntervalSec = ConvertToFloat(splitLine[7]),
                        FishLerpRate = 1.0f / ConvertToFloat(splitLine[8]),
                        MinJumpDistance = ConvertToVec2(splitLine[9], splitLine[11], Vector2.zero),
                        MaxJumpDistance = ConvertToVec2(splitLine[10], splitLine[12], Vector2.zero),
                        MinBounds = ConvertToVec2(splitLine[13], splitLine[15], Vector2.zero),
                        MaxBounds = ConvertToVec2(splitLine[14], splitLine[16], Vector2.zero),
                    };

                    fishStats.Add(fish);
                }

                items.Add(currItem);
            }
        }
        finally
        {
            itemSO.items = items.ToArray();
            fishSO.fishStats = fishStats.ToArray();
            reader.Close();
            AssetDatabase.CreateAsset(itemSO, m_outputItemFilePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.CreateAsset(fishSO, m_outputFishFilePath);
            AssetDatabase.SaveAssets();
        }

    }
    private void LoadSprites()
    {
        m_sprites = new Dictionary<string, Sprite>();
        string[] folders = { m_spritesfilePath };
        string[] guids = AssetDatabase.FindAssets("t:sprite", folders);

        StringBuilder sb = new StringBuilder();
        sb.Append("Loaded sprites include: ");

        foreach (var id in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(id);
            UnityEngine.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            sb.AppendFormat("{0}, ", path);

            Sprite sprite = null;
            foreach (var asset in assets)
            {
                if (!(asset is Sprite)) continue;
                sprite = (Sprite)asset;
            }

            m_sprites.Add(sprite.name, sprite);
        }

        Debug.Log(sb.ToString());

    }

    private int ConvertToInt(string str, int def = 0)
    {
        if (str != "" && int.TryParse(str, out int outInt))
            return outInt;

        return def;
    }

    private float ConvertToFloat(string str, float def = 0.0f)
    {
        if (str != "" && float.TryParse(str, out float outFloat))
            return outFloat;

        return def;
    }

    private Vector2 ConvertToVec2(string x, string y, Vector2 def)
    {
        return new Vector2(ConvertToFloat(x, def.x), ConvertToFloat(y, def.y));
    }


    private string ConvertToString(string str, string def = "")
    {
        return str != "" ? str : def;
    }

    private Sprite ConvertToSprite(string str)
    {
        if (!m_sprites.ContainsKey(str))
        {
            Debug.LogError("Couldn't find sprite (" + str + ")");
            return null;
        }

        return m_sprites[str];
    }
}
#endif        
