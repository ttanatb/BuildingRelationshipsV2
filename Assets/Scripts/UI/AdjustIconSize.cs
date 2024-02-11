using UnityEngine;
using UnityEngine.UI;

public class AdjustIconSize : MonoBehaviour
{
    [SerializeField] private Sprite m_bigSprite = null;
    [SerializeField] private RectTransform m_resizedElement = null;
    [SerializeField] private Vector2 m_bigSize = new Vector2(70, 70);
    [SerializeField] private Sprite m_smallSprite = null;
    [SerializeField] private Vector2 m_smallSize = new Vector2(45, 70);
    [SerializeField] private RectTransform m_parentToRedraw = null;

    public void ResizeFor(Sprite sprite)
    {
        if (sprite == m_bigSprite)
        {
            m_resizedElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_bigSize.x);
            m_resizedElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_bigSize.y);
        } 
        else if (sprite == m_smallSprite)
        {
            m_resizedElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, m_smallSize.x);
            m_resizedElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, m_smallSize.y);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_parentToRedraw);
    }
}
