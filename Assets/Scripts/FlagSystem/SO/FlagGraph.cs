using System.Collections.Generic;
using System.Linq;
using FlagSystem.SerializedDict;
using FlagSystem.Structs;
using NaughtyAttributes;
using UnityEngine;
using Utilr.Attributes;

namespace FlagSystem.SO
{
    [CreateAssetMenu(fileName = "FlagGraph", menuName = "br/FlagGraph", order = 1)]
    public class FlagGraph : ScriptableObject
    {
        [SerializeField] private FlagCompletionEvent m_flagCompletionEvent = null;
        [SerializeField] private FlagItemPreReqUpdateEvent m_itemPreReqUpdateEvent = null;
        [SerializeField] private FlagGraphUpdateEvent m_flagGraphUpdateEvent = null;
        
        [IncludeAllAssetsWithType(OnAssignedCb = "OnNodesAssigned")]
        [SerializeField] private FlagNode[] m_nodes = new FlagNode[]{};
        [SerializeField] private FlagNode m_rootNode = null;
        [SerializeField] private FlagNodeToCompletionStatusDict m_dict = new FlagNodeToCompletionStatusDict();

        [SerializeField] private FlagNode[] m_unconnectedNodes = new FlagNode[]{};
        [SerializeField] private FlagNode m_testFlagNode = null;

        private FlagNode[] m_completedNodes = new FlagNode[]{};
        private readonly List<FlagNode> m_currentNodes = new List<FlagNode>();
        private readonly List<FlagNode> m_nextNodes = new List<FlagNode>();

        public void OnFlagCompleted(FlagNode node)
        {
            if (m_unconnectedNodes.Contains(node))
            {
                Debug.LogError($"Completed node, despite being disconnected from graph: {node}");
                return;
            }
            
            TraverseDfs(step => {
                if (node != step.Node) return;

                m_completedNodes = step.Path;
                m_currentNodes.Remove(node);
                foreach (var conn in node.Connections)
                {
                    m_currentNodes.Add(conn.Node);
                }
            }, step => step.Node == node);

            InvokeFlagGraphUpdateEvent();
        }

        private void InvokeFlagGraphUpdateEvent()
        {
            m_nextNodes.Clear();
            foreach (var conn in m_currentNodes.SelectMany(curr => curr.Connections))
            {
                m_nextNodes.Add(conn.Node);
            }

            var status = new FlagGraphUpdateStatus()
            {
                CompletedNodes = m_completedNodes.ToArray(),
                CurrentNodes = m_currentNodes.ToArray(),
                NextNodes = m_nextNodes.ToArray(),
            };
            
            m_flagGraphUpdateEvent.Invoke(status);
        }

        /// <summary>
        /// Called by the AssetPostProcessor after assigning all Nodes to field.
        /// </summary>
        public void OnNodesAssigned()
        {
            if (!NeedToRecreateDict()) return;

            var dict = new FlagNodeToCompletionStatusDict();
            foreach (var node in m_nodes)
            {
                m_dict.TryGetValue(node, out var status);
                dict.Add(node, status);
            }

            m_dict = dict;
        }

        private bool NeedToRecreateDict()
        {
            // Early check if counts are unaligned.
            if (m_dict.Count != m_nodes.Length) return true;

            // Count how many nodes in the list are in the dict.
            int found = m_nodes.Count(node => m_dict.ContainsKey(node));

            // Expect nodes in list to all be in dict.
            return found == m_nodes.Length;
        }

        private struct GraphSearchStep
        {
            public FlagNode Node { get; set; }
            public FlagNode[] Path { get; set; }
        }

        [Button]
        private void ValidateGraph()
        {
            var tracker = new FlagNodeToCompletionStatusDict();
            tracker.CopyFrom(m_dict);

            TraverseDfs(step => {
                tracker.Remove(step.Node);
            });
            m_unconnectedNodes = tracker.Keys.ToArray();
        }

        private void TraverseDfs(System.Action<GraphSearchStep> processNodeCb, 
            System.Func<GraphSearchStep, bool> earlyExitCb = null)
        {
            var stack = new Stack<GraphSearchStep>();
            var found = new HashSet<FlagNode>();
            stack.Push(new GraphSearchStep()
            {
                Node = m_rootNode,
                Path = new[]{m_rootNode}
            });

            while (stack.Count > 0)
            {
                var curr = stack.Pop();
                if (found.Contains(curr.Node))
                {
                    Debug.LogError($"Loop detected!!! at {curr.Node}");
                    return;
                }
                
                processNodeCb.Invoke(curr);
                if (earlyExitCb != null && earlyExitCb.Invoke(curr)) return;
                
                found.Add(curr.Node);
                
                var currPath = new List<FlagNode>(curr.Path);
                foreach (var conn in curr.Node.Connections)
                {
                    stack.Push(
                        new GraphSearchStep()
                        {
                            Node = conn.Node,
                            Path = currPath.Append(conn.Node).ToArray()
                        });
                }
            }

        }

        [Button]
        private void TestCompleteUpToTestFlag()
        {
            ValidateGraph();
            if (m_unconnectedNodes.Contains(m_testFlagNode))
            {
                Debug.LogError($"Test Node {m_testFlagNode} is unconnected from root!");
            }

            TraverseDfs(curr => {
                if (m_testFlagNode != curr.Node) return;

                // Tick every node that led to this spot
                foreach (var node in curr.Path)
                {
                    var res = m_dict[node];
                    
                    if (!res.Completed)
                        m_flagCompletionEvent.Invoke(node);
                    res.Completed = true;
                    m_dict[node] = res;
                }
            }, step => step.Node == m_testFlagNode);
        }

        [Button]
        private void ResetFlag()
        {
            TraverseDfs(curr => {
                var node = curr.Node;
                var res = m_dict[node];
                res.Completed = false;
                m_dict[node] = res;
            });

            m_completedNodes = new FlagNode[] {};
            m_currentNodes.Clear();
            m_currentNodes.Add(m_rootNode);
            InvokeFlagGraphUpdateEvent();
        }
    }
}
