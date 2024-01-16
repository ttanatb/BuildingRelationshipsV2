using System.Collections.Generic;
using System.Linq;
using Animation.SerializedDict;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Utilr.SoGameEvents;

[RequireComponent(typeof(Animator))]
public class SlTriggerAnimation : MonoBehaviour
{
    [SerializeField] private SoGameEventToAnimParamDict m_animsToTrigger = null;
    private readonly Dictionary<SoGameEvent, int> m_animDict = new Dictionary<SoGameEvent, int>();
    private readonly Dictionary<SoGameEvent, UnityAction> m_actionToRemove = new Dictionary<SoGameEvent, UnityAction>();

    private Animator m_animator = null;
    
    // Start is called before the first frame update
    private void Start()
    {
        TryGetComponent(out m_animator);

        m_animDict.Clear();
        m_actionToRemove.Clear();
        var parameters = m_animator.parameters;
        foreach (var pairs in m_animsToTrigger)
        {
            var param = parameters.First(p => p.name == pairs.Value);
            Assert.IsTrue(param.type == AnimatorControllerParameterType.Trigger, 
                $"Found {param} of type {param.type}, only Triggers supported");
            m_animDict.Add(pairs.Key, param.nameHash);
            m_actionToRemove.Add(pairs.Key, () => OnAnimEventTriggered(param.nameHash));
            
            pairs.Key.Event.AddListener(m_actionToRemove[pairs.Key]);
        }
    }

    private void OnDestroy()
    {
        foreach (var pairs in m_animDict)
        {
            pairs.Key.Event.RemoveListener(m_actionToRemove[pairs.Key]);
        }
    }
    
    private void OnAnimEventTriggered(int hash)
    {
        m_animator.SetTrigger(hash);
    }
}
