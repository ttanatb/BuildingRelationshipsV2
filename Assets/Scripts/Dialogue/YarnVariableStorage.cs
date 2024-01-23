using System.Collections.Generic;
using Dialogue.SerializedDict;
using Dialogue.SO;
using Dialogue.Struct;
using Saves.SO;
using Saves.Structs;
using UnityEngine;
using Yarn.Unity;

namespace Dialogue
{
    public class YarnVariableStorage : InMemoryVariableStorage
    {
        [SerializeField] private LoadSaveEvent m_loadSaveEvent = null;
        [SerializeField] private YarnVariableUpdateEvent m_yarnVariableUpdateEvent = null;

        private StringToFloatDict m_floatDict = new StringToFloatDict();
        private StringToStringDict m_stringDict = new StringToStringDict();
        private StringToBoolDict m_boolDict = new StringToBoolDict();
        
        private void Start()
        {
            m_loadSaveEvent.Event.AddListener(OnSaveDataLoaded);
        }
        
        private void OnDestroy()
        {
            m_loadSaveEvent.Event.RemoveListener(OnSaveDataLoaded);
        }

        private void InvokeEventHelper()
        {
            m_yarnVariableUpdateEvent.Invoke(new YarnVariableData()
            {
                FloatDict = m_floatDict,
                StringDict = m_stringDict,
                BoolDict = m_boolDict,
            });
        }
        
        private void OnSaveDataLoaded(SaveData data)
        {
            var yarnVariableData = data.YarnVariableData;

            m_floatDict = yarnVariableData.FloatDict;
            m_stringDict = yarnVariableData.StringDict;
            m_boolDict = yarnVariableData.BoolDict;
            
            SetAllVariables(
                new Dictionary<string, float>(m_floatDict),
                new Dictionary<string, string>(m_stringDict),
                new Dictionary<string, bool>(m_boolDict));
        }

        public override void SetValue(string variableName, float floatValue)
        {
            base.SetValue(variableName, floatValue);
            
            m_floatDict[variableName] = floatValue;
            InvokeEventHelper();
        }
        
        public override void SetValue(string variableName, string stringValue)
        {
            base.SetValue(variableName, stringValue);
                        
            m_stringDict[variableName] = stringValue;
            InvokeEventHelper();
        }
        
        public override void SetValue(string variableName, bool boolValue)
        {
            base.SetValue(variableName, boolValue);
                        
            m_boolDict[variableName] = boolValue;
            InvokeEventHelper();
        }
    }
}
