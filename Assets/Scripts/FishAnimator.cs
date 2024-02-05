using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Util;

public class FishAnimator : MonoBehaviour
{
    [SerializeField] private Animator m_animator = null;
    [AnimatorParam("m_animator")] [SerializeField]
    private int m_idleBounceTrigger = 1;
    
    [AnimatorParam("m_animator")] [SerializeField]
    private int m_idleSwerveTrigger = 1;

    [AnimatorParam("m_animator")] [SerializeField]
    private int m_caughtTrigger = 1;
    
    [SerializeField] private Vector2 m_swapIdleDurRange = new Vector2(2.0f, 10.0f);
    
    [SerializeField]
    private float m_timer = 0.0f;
    [SerializeField]
    private float m_currDuration = 1.0f;
    [SerializeField]
    private bool m_isBouncing = true;

    public void StartReelingAnim()
    {
        m_animator.ResetTrigger(m_idleBounceTrigger);
        m_animator.ResetTrigger(m_idleSwerveTrigger);
        m_animator.SetTrigger(m_caughtTrigger);
        
        // Effectively make it so that idle won't trigger
        m_timer = float.MinValue;
    }
    
    public void StopReelingAnim()
    {
        m_animator.SetTrigger(m_idleBounceTrigger);
        m_isBouncing = true;
        m_timer = 0;
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer < m_currDuration)
            return;
        
        m_timer = 0.0f;
        m_currDuration = m_swapIdleDurRange.RandomInRange();
        m_isBouncing = !m_isBouncing;
        m_animator.SetTrigger(m_isBouncing ?  m_idleBounceTrigger : m_idleSwerveTrigger);
    }

}
