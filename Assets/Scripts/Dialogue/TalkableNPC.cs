using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkableNPC : MonoBehaviour
{
    [SerializeField]
    Transform m_dialogueBoxAnchor = null;

    [SerializeField]
    Camera m_activeCamera = null;

    [SerializeField]
    UISpeechBubble m_testUiElement = null;

    [SerializeField]
    string m_dialogueNodeName = "";

    public string DialogueNodeName { get { return m_dialogueNodeName; } }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        m_testUiElement.ScreenPos =
            m_activeCamera.WorldToViewportPoint(m_dialogueBoxAnchor.position);
    }

    public void SetInteractable(bool isInteractable)
    {
        //interactbleText_.text = isInteractable ? "!" : "?";
    }
}
