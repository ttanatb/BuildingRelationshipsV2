using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiPageFlipper : MonoBehaviour
{
    [SerializeField] private UiPageContent[] m_pages = null;
    [SerializeField] private int m_leftPageIndex = 0;
    [SerializeField] private AnimationClip m_turnLeftAnimClip = null;
    [SerializeField] private float m_hideContentDelay = 0.1f;
    [SerializeField] private float m_showContentDelay = 0.1f;
    [SerializeField] private float m_flipHideContentDelay = 0.0f;
    [SerializeField] private float m_flipShowContentDelay = 0.0f;

    [SerializeField] private InputActionReference m_action = null;

    private bool m_isCurrentlyFlipping = false;    
    // TODO: separate SwitchContentSide -- hide with less delay, show with more delay

    [Button]
    private void TurnRightToLeft()
    {
        if (m_leftPageIndex == m_pages.Length - 2) return;
        m_isCurrentlyFlipping = true;
        float delay = m_turnLeftAnimClip.length / 2.0f;

        m_pages[m_leftPageIndex + 1].TriggerAnimToLeft();
        StartCoroutine(m_pages[m_leftPageIndex + 1].HideContent(UiPageContent.Side.Left, delay + m_flipHideContentDelay));
        StartCoroutine(m_pages[m_leftPageIndex + 1].ShowContent(UiPageContent.Side.Right, delay + m_flipShowContentDelay));
        
        // StartCoroutine(m_pages[m_leftPageIndex].ShowContent(UiPageContent.Side.Right ,0));
        StartCoroutine(m_pages[m_leftPageIndex].HideContent(delay + m_hideContentDelay));

        if (m_leftPageIndex + 2 < m_pages.Length)
        {
            StartCoroutine(m_pages[m_leftPageIndex + 2].ShowContent(UiPageContent.Side.Left, delay + m_showContentDelay));
        }

        for (int i = 0; i < m_pages.Length; i++)
        {
            if (i == m_leftPageIndex || i == m_leftPageIndex + 1 || i == m_leftPageIndex + 2) continue;
            // m_pages[i].ReallyHideContent();
        }

        StartCoroutine(ReEnableInputAfter(delay));
        m_leftPageIndex++;
    }

    [Button]
    private void TurnLeftToRight()
    {
        if (m_leftPageIndex == 0) return;
        m_isCurrentlyFlipping = true;
        float delay = m_turnLeftAnimClip.length / 2.0f;

        if (m_leftPageIndex + 1 < m_pages.Length)
        {
            // StartCoroutine(m_pages[m_leftPageIndex + 1].ShowContent(UiPageContent.Side.Left ,0));
            StartCoroutine(m_pages[m_leftPageIndex + 1].HideContent(delay + m_hideContentDelay));
        }
        
        m_pages[m_leftPageIndex].TriggerAnimToRight();
        StartCoroutine(m_pages[m_leftPageIndex].HideContent(UiPageContent.Side.Right, delay + m_flipHideContentDelay));
        StartCoroutine(m_pages[m_leftPageIndex].ShowContent(UiPageContent.Side.Left, delay + m_flipShowContentDelay));
        
        StartCoroutine(m_pages[m_leftPageIndex - 1].ShowContent(UiPageContent.Side.Right, delay + m_showContentDelay));
        
        for (int i = 0; i < m_pages.Length; i++)
        {
            if (i == m_leftPageIndex || i == m_leftPageIndex + 1 || i == m_leftPageIndex - 1) continue;
            // m_pages[i].ReallyHideContent();
        }
        
        StartCoroutine(ReEnableInputAfter(delay));
        m_leftPageIndex--;
    }

    private IEnumerator ReEnableInputAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_isCurrentlyFlipping = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        m_action.action.performed += OnNavigateEvent;

        // for (int i = 0; i < m_pages.Length - 2; i++)
        // {
        //     StartCoroutine(m_pages[i + 1].HideContent(UiPageContent.Side.Left, 0));
        //     StartCoroutine(m_pages[i + 1].ShowContent(UiPageContent.Side.Right, 0));
        //     m_pages[i + 1].TriggerAnimToLeft(true);
        //
        //     StartCoroutine(m_pages[i].HideContent(0));
        //     
        //     if (i + 2 < m_pages.Length)
        //         StartCoroutine(m_pages[i + 2].ShowContent(UiPageContent.Side.Left, 0));
        //
        //     m_leftPageIndex++;
        // }
        
        for (int i = m_pages.Length - 1; i > 0; i--)
        {
            StartCoroutine(m_pages[i].HideContent(0));
            StartCoroutine(m_pages[i].HideContent(UiPageContent.Side.Left, 0));
            StartCoroutine(m_pages[i].HideContent(UiPageContent.Side.Right, 0));
            m_pages[i].TriggerAnimToRight(true);
            m_pages[i].transform.rotation = Quaternion.AngleAxis(-180, Vector3.up);
            // m_pages[i].transform.localScale = Vector3.zero;
        }
        
        StartCoroutine(m_pages[0].ShowContent(UiPageContent.Side.Right, 0));
        StartCoroutine(m_pages[1].ShowContent(UiPageContent.Side.Left, 0));
        m_leftPageIndex = 0;
        }

    private void OnDestroy()
    {
        m_action.action.performed -= OnNavigateEvent;
    }

    private void OnNavigateEvent(InputAction.CallbackContext ctx)
    {
        if (m_isCurrentlyFlipping) return;
        
        if (ctx.ReadValue<Vector2>().x < 0)
        {
            TurnLeftToRight();
        } else if (ctx.ReadValue<Vector2>().x > 0)
        {
            TurnRightToLeft();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
