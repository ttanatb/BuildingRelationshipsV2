using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInteractText : MonoBehaviour
{
    [SerializeField]
    RectTransform m_parentCanvas = null;

    [SerializeField]
    TextMeshProUGUI m_interactMessage = null;

    RectTransform m_transform = null;

    private Vector3 m_screenPos = Vector3.zero;
    public Vector3 ScreenPos
    {

        set
        {
            m_screenPos = value;

            value.x *= m_parentCanvas.rect.size.x;
            value.y *= m_parentCanvas.rect.size.y;

            m_transform.anchoredPosition = value;
        }
    }

    public string Text
    {
        set
        {
            m_interactMessage.text = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_transform = GetComponent<RectTransform>();
        m_interactMessage = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
