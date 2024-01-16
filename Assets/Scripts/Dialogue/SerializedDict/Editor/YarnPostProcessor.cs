using Dialogue.SO;
using NUnit.Framework;
using UnityEditor;
using Utilr.Editor;

namespace Dialogue.SerializedDict.Editor
{
    /// <summary>
    /// Whenever a yarn file is saved, call function in YarnEventDb to update its event listing
    /// </summary>
    public class YarnPostProcessor : AssetPostprocessor
    {
        private static readonly string[] YARN_EXTS = {
            ".yarn", ".yarnproject",
        };
        
        private static void OnPostprocessAllAssets(string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!importedAssets.ContainsTrackedExt(YARN_EXTS) &&
                !deletedAssets.ContainsTrackedExt(YARN_EXTS))
                return;

            ProcessAllObjects();
        }

        private static void ProcessAllObjects()
        {
            object[] soFiles = Helper.GetAllAssetsOfType(typeof(YarnEventDb));
            foreach (object obj in soFiles)
            {
                var listener = obj as YarnEventDb;
                Assert.IsNotNull(listener);
                listener.OnYarnFileSaved();
                listener.ProcessSavedAssets(FindAsset);
            }
        }

        private static object[] FindAsset(string filter, string[] folder, System.Type type)
        {
            string[] guids = AssetDatabase.FindAssets(filter);
            object[] a = new object[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath(path, type);
            }

            return a;
        }
    }
}

