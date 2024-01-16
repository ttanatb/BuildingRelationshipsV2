using System.Text;
using FlagSystem.SO;
using FlagSystem.Structs;
using TMPro;
using UnityEngine;

public class BookJournal : MonoBehaviour
{
    [SerializeField] private FlagGraphUpdateEvent m_flagGraphUpdateEvent = null;
    [SerializeField] private TextMeshPro m_text = null;
    private readonly StringBuilder m_stringBuilder = new StringBuilder();

    // Start is called before the first frame update
    private void Start()
    {
        m_flagGraphUpdateEvent.Event.AddListener(OnGraphUpdate);
    }

    private void OnDestroy()
    {
        m_flagGraphUpdateEvent.Event.RemoveListener(OnGraphUpdate);
    }

    private void OnGraphUpdate(FlagGraphUpdateStatus status)
    {
        m_stringBuilder.Clear();
        m_stringBuilder.Append("<b>");
        foreach (var node in status.CurrentNodes)
        {
            m_stringBuilder.Append($"- {node.name}\n");
        }
        m_stringBuilder.Append("</b>\n");
        m_stringBuilder.Append("<s>");
        foreach (var node in status.CompletedNodes)
        {
            m_stringBuilder.Append($"- {node.name}\n");
        }
        m_stringBuilder.Append("</s>");

        m_text.text = m_stringBuilder.ToString();
    }
}
