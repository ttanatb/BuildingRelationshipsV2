using System.Collections;
using System.Collections.Generic;
using Inventory.Structs;
using NaughtyAttributes;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public class UiPageContent : MonoBehaviour
{
    public enum Side
    {
        Left,
        Right,
    };

    [SerializeField] private Animator m_animator = null;
    [AnimatorParam("m_animator")] [SerializeField]
    private int m_turnRightTrigger = 1;
    [AnimatorParam("m_animator")] [SerializeField]
    private int m_turnLeftTrigger = 1;

    // Used as helper to initialize fish list
    [SerializeField] private ItemData.ItemID[] m_fishIds = null;
    [SerializeField] private BookFishPanel[] m_fishPanels = null;

    [SerializeField] private SortingGroup m_leftSide = null;
    [SerializeField] private SortingGroup m_rightSide = null;

    [SerializeField] private SortingGroup m_pageSortingGroup = null;
    [SerializeField] private SpriteRenderer m_spriteRenderer = null;

    private const int SORTING_ORDER = 7;

    private TextMeshPro[] m_leftTexts = null;
    private TextMeshPro[] m_rightTexts = null;

    private BookContent[] m_leftContents = null;
    private BookContent[] m_rightContents = null;

    [SerializeField] private bool m_startLeft = false;

    private void Awake()
    {
        TryGetComponent(out m_pageSortingGroup);
        TryGetComponent(out m_spriteRenderer);
        if (m_leftSide != null)
        {
            m_leftTexts = m_leftSide.GetComponentsInChildren<TextMeshPro>();
            m_leftContents = m_leftSide.GetComponentsInChildren<BookContent>();
        }

        if (m_rightSide != null)
        {
            m_rightTexts = m_rightSide.GetComponentsInChildren<TextMeshPro>();
            m_rightContents = m_rightSide.GetComponentsInChildren<BookContent>();
        }
    }

    private void Start()
    {
        Assert.AreEqual(m_fishIds.Length, m_fishPanels.Length);
        for (int i = 0; i < m_fishPanels.Length; i++)
        {
            if (i >= m_fishIds.Length)
            {
                m_fishPanels[i].gameObject.SetActive(false);
                continue;
            }

            m_fishPanels[i].Id = m_fishIds[i];
        }
        
        if (m_startLeft)
            TriggerAnimToLeft();
    }

    public IEnumerator ShowContent(Side side, float delay)
    {
        yield return new WaitForSeconds(delay);

        m_pageSortingGroup.sortingOrder = SORTING_ORDER;
        m_spriteRenderer.sortingOrder = SORTING_ORDER;

        if (side == Side.Left)
        {
            if (m_leftSide)
                m_leftSide.sortingOrder = SORTING_ORDER + 1;

            SetTextsActive(m_leftTexts, m_leftContents, true);
        }
        else
        {
            if (m_rightSide)
                m_rightSide.sortingOrder = SORTING_ORDER + 1;

            SetTextsActive(m_rightTexts, m_rightContents, true);
        }
    }

    public IEnumerator HideContent(Side side, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (side == Side.Left)
        {
            if (m_leftSide)
                m_leftSide.sortingOrder = SORTING_ORDER - 1;

            SetTextsActive(m_leftTexts, m_leftContents, false);
        }
        else
        {
            if (m_rightSide)
                m_rightSide.sortingOrder = SORTING_ORDER - 1;

            SetTextsActive(m_rightTexts, m_rightContents, false);
        }
    }

    public IEnumerator HideContent(float delay)
    {
        yield return new WaitForSeconds(delay);

        m_pageSortingGroup.sortingOrder = SORTING_ORDER - 1;
        m_spriteRenderer.sortingOrder = SORTING_ORDER - 1;

        if (m_leftSide)
            m_leftSide.sortingOrder = SORTING_ORDER - 1;

        SetTextsActive(m_leftTexts, m_leftContents,false);
        if (m_rightSide)
            m_rightSide.sortingOrder = SORTING_ORDER - 1;

        SetTextsActive(m_rightTexts, m_rightContents, false);
    }
    
    public void ReallyHideContent()
    {
        if (m_pageSortingGroup)
            m_pageSortingGroup.sortingOrder = SORTING_ORDER - 2;
        
        if (m_spriteRenderer)
            m_spriteRenderer.sortingOrder = SORTING_ORDER - 2;

        if (m_leftSide)
            m_leftSide.sortingOrder = SORTING_ORDER - 2;

        SetTextsActive(m_leftTexts, m_leftContents, false);
        if (m_rightSide)
            m_rightSide.sortingOrder = SORTING_ORDER - 2;

        SetTextsActive(m_rightTexts, m_rightContents, false);
    }
    
    // public void ReallyShowContent()
    // {
    //     m_pageSortingGroup.sortingOrder = m_sortingOrder + 2;
    //     m_spriteRenderer.sortingOrder = m_sortingOrder + 2;
    // }

    // public IEnumerator SetActive(bool isActive, float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //
    //     SetTextsActive(m_rightTexts, isActive);
    //     SetTextsActive(m_leftTexts, isActive);
    //
    //     m_pageSortingGroup.sortingOrder = isActive ? m_sortingOrder : m_sortingOrder - 1;
    //     m_spriteRenderer.sortingOrder = isActive ? m_sortingOrder : m_sortingOrder - 1;
    //
    //     if (!isActive)
    //     {
    //         if (m_leftSide)
    //             m_leftSide.sortingOrder = m_sortingOrder - 1;
    //         
    //         if (m_rightSide)
    //             m_rightSide.sortingOrder = m_sortingOrder - 1;
    //     }
    //     else
    //     {
    //         if (m_isLeft)
    //         {
    //             if (m_rightSide)
    //                 m_rightSide.sortingOrder = isActive ? m_sortingOrder + 1 : m_sortingOrder - 1;
    //         }
    //         else
    //         {
    //             if (m_leftSide)
    //                 m_leftSide.sortingOrder = isActive ? m_sortingOrder + 1 : m_sortingOrder - 1;
    //         }
    //     }
    // }

    public IEnumerator SwitchContentSide(Side side, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (side == Side.Left)
        {
            if (m_rightSide)
            {
                m_rightSide.sortingOrder = SORTING_ORDER - 1;
            }
            if (m_leftSide)
            {
                m_leftSide.sortingOrder = SORTING_ORDER + 1;
            }
            
            SetTextsActive(m_leftTexts, m_leftContents, true);
            SetTextsActive(m_rightTexts, m_rightContents, false);
        }
        else
        {
            if (m_leftSide)
            {
                m_leftSide.sortingOrder = SORTING_ORDER - 1;
            }
            if (m_rightSide)
            {
                m_rightSide.sortingOrder = SORTING_ORDER + 1;
            }
            
            SetTextsActive(m_rightTexts, m_leftContents, true);
            SetTextsActive(m_leftTexts, m_rightContents, false);
        }
    }

    [Button]
    public void TriggerAnimToLeft(bool skipAnim = false)
    {
        if (skipAnim)
        {
            transform.rotation = Quaternion.identity;
        } 
        else m_animator.SetTrigger(m_turnLeftTrigger);
        m_pageSortingGroup.sortingOrder = SORTING_ORDER + 2;
        m_spriteRenderer.sortingOrder = SORTING_ORDER + 2;
        
        if (m_leftSide)
        {
            m_leftSide.sortingOrder = SORTING_ORDER + 3;
        }
    }

    [Button]
    public void TriggerAnimToRight(bool skipAnim = false)
    {
        if (skipAnim)
        {
            transform.rotation = Quaternion.Euler(0, -180,0);
        } 
        else m_animator.SetTrigger(m_turnRightTrigger);
        m_pageSortingGroup.sortingOrder = SORTING_ORDER + 2;
        m_spriteRenderer.sortingOrder = SORTING_ORDER + 2;
        
        if (m_rightSide)
        {
            m_rightSide.sortingOrder = SORTING_ORDER + 3;
        }
    }

    private static void SetTextsActive(IEnumerable<TextMeshPro> texts, IEnumerable<BookContent> bookContents, bool isActive)
    {
        if (texts != null)
        {

            foreach (var text in texts)
            {
                text.gameObject.SetActive(isActive);
            }
        }

        if (bookContents != null)
        {
            foreach (var content in bookContents)
            {  
                content.SetVisible(isActive);
            }
        }
    }
}
