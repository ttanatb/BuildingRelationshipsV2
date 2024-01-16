using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dialogue.SerializedDict;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Utilr.SoGameEvents;
using Yarn.Unity;
using Yarn;

namespace Dialogue.SO
{
    /// <summary>
    /// Connects Yarn Events to respective game events.
    /// </summary>
    [CreateAssetMenu(fileName = "YarnEvents", menuName = "dialogue/YarnEvents", order = 1)]
    public class YarnEventDb : ScriptableObject
    {
        [SerializeField] private string m_yarnInterfaceName = "YarnEventManager";
        [SerializeField] private string m_eventCommandString = "fireEvent";
        [SerializeField] private string m_camEventPrefix = "cam_";
        [SerializeField] private string m_folderFormat = "ScriptableObjects/Cutscenes/{0}";
        [SerializeField] private string[] m_folderSearch = new string[] {"ScriptableObjects/Cutscenes"};

        [field: SerializeField]
        public YarnEventNameToGameEventDict YarnEventNameToGameEventDict { get; private set; }
            = new YarnEventNameToGameEventDict() {};
        
        /// <summary>
        /// Called by `YarnPostProcessor` to update the database whenever a yarn file is modified.
        /// </summary>
        [Button()]
        public void OnYarnFileSaved()
        {
            var sb = new StringBuilder();
            var runners = FindObjectsOfType<DialogueRunner>();
            var set = new HashSet<string>();
            foreach (var runner in runners)
            {
                ProcessRunner(runner, sb, set);
            }

            foreach (var pair in YarnEventNameToGameEventDict)
            {
                if (set.Contains(pair.Key))
                    continue;
                
                Debug.LogError($"Database contains {pair.Key}, not referenced in yarn script");
            }
            
            // Sort
            YarnEventNameToGameEventDict.CopyFrom(
                YarnEventNameToGameEventDict.OrderBy(pair => pair.Key)
                    .ToDictionary(pair => pair.Key, pair => pair.Value));

            if (sb.Length > 0)
                Debug.Log(sb.ToString());
        }

        public void ProcessSavedAssets(System.Func<string, string[], System.Type, object[]> findAssetsCb)
        {
            var copyDict = new YarnEventNameToGameEventDict();
            copyDict.CopyFrom(YarnEventNameToGameEventDict);
            
            foreach (var pair in YarnEventNameToGameEventDict)
            {
                if (!pair.Key.StartsWith(m_camEventPrefix))
                    continue;

                if (pair.Value != null) 
                    continue;

                string formattedName = ToUpperUnderscoreCase(pair.Key);
                object[] results = findAssetsCb.Invoke($"\"{formattedName}\"", m_folderSearch, typeof(SoGameEventBase));
                Debug.Log($"Found results for {pair.Key} ({formattedName}): count {results.Length}");
                foreach (object res in results)
                {
                    var gameEvent = res as SoGameEventBase;
                    Assert.IsNotNull(gameEvent);
                    if (gameEvent.name != formattedName)
                        continue;

                    copyDict[pair.Key] = gameEvent;
                }
            }
            
            YarnEventNameToGameEventDict.CopyFrom(copyDict);
        }

        private static string ToUpperUnderscoreCase(string value)
        {
            string result = "";
            string[] split = value.Split('_');
            
            for (int i = 0; i < split.Length; i++)
            {
                string copy = split[i];
                string upperFirst = copy[..1].ToUpper();
                
                // Remove first character, insert upper case version
                copy = copy.Remove(0, 1);
                copy = copy.Insert(0, upperFirst);
                
                result += copy;
                if (i < split.Length - 1)
                    result += "_";
            }
            
            return result;
        } 

        private void ProcessRunner(DialogueRunner runner, StringBuilder sb, HashSet<string> set)
        {
            var program = runner.yarnProject.Program;
            var programNodes = program.Nodes;
            foreach (var node in programNodes.Values)
            {
                ProcessNode(node, sb, set);
            }
        }

        private void ProcessNode(Node node, StringBuilder sb, HashSet<string> set)
        {
            foreach (var instruction in node.Instructions)
            {
                ProcessInstruction(instruction, sb, set);
            }
        }

        private void ProcessInstruction(Instruction instruction, StringBuilder sb, HashSet<string> set)
        {
            if (instruction.Opcode != Instruction.Types.OpCode.RunCommand)
                return;

            string commandLine = instruction.Operands[0].StringValue;
            if (!commandLine.Contains(m_eventCommandString))
                return;

            string[] parsed = commandLine.Split(' ');
            if (!parsed[1].Equals(m_yarnInterfaceName))
            {
                var errorSb = new StringBuilder();
                foreach (var op in instruction.Operands)
                {
                    errorSb.Append($"{op}, ");
                }
                if (errorSb.Length > 0)
                    errorSb.Remove(errorSb.Length - 2, 2);

                Debug.LogError($"Expected {m_yarnInterfaceName}, found: {errorSb}");
                return;
            }

            string eventName = parsed[2].TrimStart('\"').TrimEnd('\"');
            set.Add(eventName);

            if (!YarnEventNameToGameEventDict.TryAdd(eventName, null))
                return;

            sb.Append($"[Added {eventName}]");
        }
    }
}
