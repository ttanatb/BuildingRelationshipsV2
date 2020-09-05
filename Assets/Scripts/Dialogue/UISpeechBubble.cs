using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Yarn.Unity;

public class UISpeechBubble : MonoBehaviour
{
    const string ANIM_BLINKER_SHOW = "isBlinkerShowing";

    [SerializeField]
    RectTransform m_parentCanvas;

    [SerializeField]
    RectTransform m_bubbTransform;

    [SerializeField]
    RectTransform m_bubbAnchorTransform;

    [SerializeField]
    RectTransform m_transform;

    [SerializeField]
    TMPro.TextMeshProUGUI m_text;

    [SerializeField]
    TMPro.TextMeshProUGUI m_nameText;

    [SerializeField]
    DialogueUI m_yarnDialogueUI;

    [SerializeField]
    Image m_blinker;

    [SerializeField]
    PlayerController m_playerController;

    Animator m_animator;
    PlayerControls m_playerControls = null;

    PlayerInput m_playerInput = null;

    [SerializeField]
    float m_textUpdateIntervalSec = 0.085f;

    bool m_skipToEnd = false;
    bool m_completedText = false;

    StringBuilder m_stringBuilder = new StringBuilder();

    private Vector3 m_screenPos;
    public Vector3 ScreenPos
    {
        get
        {
            return m_screenPos;
        }
        set
        {
            m_screenPos = value;

            //Debug.Log(m_parentCanvas.rect.size);
            value.x *= m_parentCanvas.rect.size.x;
            value.y *= m_parentCanvas.rect.size.y;

            m_transform.anchoredPosition = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_transform = GetComponent<RectTransform>();
        m_playerControls = m_playerController.PlayerControls;
        m_playerInput = m_playerController.GetComponent<PlayerInput>();

        if (m_parentCanvas == null)
        {
            // Get top-most parent.
            Transform parent = transform;
            while (parent.parent != null)
                parent = parent.parent;

            m_parentCanvas = parent.GetComponent<RectTransform>();
        }

        m_yarnDialogueUI.onLineUpdate.AddListener(SetDialogueLine);
        m_yarnDialogueUI.onLineStart.AddListener(ClearDisplay);
        m_yarnDialogueUI.onDialogueEnd.AddListener(SwitchToGameplay);
        m_yarnDialogueUI.onDialogueStart.AddListener(SwitchToUI);

        m_animator = GetComponent<Animator>();
        m_blinker.enabled = false;

        m_playerControls.UI.Confirm.started += TriggerNextDialogue;
    }

    private void OnDestroy()
    {
        m_playerControls.UI.Confirm.started -= TriggerNextDialogue;
    }

    public void SetDialogueLine(string word)
    {
        int indexOfColon = word.IndexOf(':');
        string name = word.Substring(0, indexOfColon);
        string text = word.Substring(indexOfColon + 1, word.Length - indexOfColon - 1);

        m_nameText.text = name;
        StartCoroutine(DoUpdateText(text, m_stringBuilder));
    }

    private IEnumerator DoUpdateText(string text, StringBuilder stringBuilder)
    {
        m_completedText = false;
        m_skipToEnd = false;

        stringBuilder.Clear();
        foreach (char c in text)
        {
            if (m_skipToEnd)
            {
                stringBuilder.Clear();
                stringBuilder.Append(text);
                m_text.text = stringBuilder.ToString();

                break;
            }

            stringBuilder.Append(c);
            m_text.text = stringBuilder.ToString();
            yield return new WaitForSeconds(m_textUpdateIntervalSec);
        }

        DisplayBlinker();

        m_completedText = true;
        yield return null;
    }

    public void DisplayBlinker()
    {
        m_blinker.enabled = true;
        m_animator.SetBool(ANIM_BLINKER_SHOW, true);
    }

    public void ClearDisplay()
    {
        m_text.text = "";
        m_blinker.enabled = false;
        m_animator.SetBool(ANIM_BLINKER_SHOW, false);
    }

    public void SwitchToUI()
    {
        m_playerControls.Player.Disable();
        m_playerInput.SwitchCurrentActionMap("UI");
        m_playerControls.UI.Enable();
        m_yarnDialogueUI.MarkLineComplete();
    }

    public void SwitchToGameplay()
    {
        m_playerControls.UI.Disable();
        m_playerInput.SwitchCurrentActionMap("Player");
        m_playerControls.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void TriggerNextDialogue(InputAction.CallbackContext context)
    {
        if (m_completedText)
            m_yarnDialogueUI.MarkLineComplete();
        else if (!m_skipToEnd)
            m_skipToEnd = true;
    }
}
