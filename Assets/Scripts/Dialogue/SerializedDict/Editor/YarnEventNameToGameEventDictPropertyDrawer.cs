#if UNITY_EDITOR
using UnityEditor;

namespace Dialogue.SerializedDict.Editor
{
    [CustomPropertyDrawer(typeof(YarnEventNameToGameEventDict))]
    public class YarnEventNameToGameEventDictPropertyDrawer : SerializableDictionaryPropertyDrawer
    {
        
    }
}

#endif
