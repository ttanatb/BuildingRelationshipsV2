using System.Collections.Generic;
using FlagSystem.Structs;
using NaughtyAttributes;
using UnityEngine;

namespace FlagSystem.SO
{
    [CreateAssetMenu(fileName = "FlagNode", menuName = "br/FlagNode", order = 1)]
    public class FlagNode : ScriptableObject
    {
        [field: SerializeField]
        public FlagNodeConnection[] Connections { get; set; }

        [SerializeField] private FlagNode m_node = null;
        [Button]
        private void AddNewConnection()
        {
            if (m_node == this) return;
            var list = new List<FlagNodeConnection>(Connections)
            {
                new FlagNodeConnection(){Node = m_node}
            };
            Connections = list.ToArray();
        }
    }
}
