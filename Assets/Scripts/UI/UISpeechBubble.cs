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
    RectTransform m_parentCanvas = null;

    [SerializeField]
    RectTransform m_bubbTransform = null;

    [SerializeField]
    RectTransform m_bubbAnchorTransform = null;

    RectTransform m_transform = null;

    [SerializeField]
    TMPro.TextMeshProUGUI m_text = null;

    [SerializeField]
    TMPro.TextMeshProUGUI m_nameText = null;

    [SerializeField]
    DialogueUI m_yarnDialogueUI = null;

    [SerializeField]
    Image m_blinker = null;

    [SerializeField]
    PlayerController m_playerController = null;

    [SerializeField]
    GameObject m_optionsController = null;

    Animator m_animator;
    PlayerControls m_playerControls = null;

    PlayerInput m_playerInput = null;

    [SerializeField]
    float m_textUpdateIntervalSec = 0.085f;

    private List<Button> m_optionButtons;

    bool m_skipToEnd = false;
    bool m_completedText = false;
    Button m_firstButton = null;

    [SerializeField]
    Vector3 m_anchorOffset = Vector3.zero;

    StringBuilder m_stringBuilder = new StringBuilder();
    UIManager m_uiManager = null;
    EventManager m_eventManager = null;
    AudioSource m_audioSource = null;
    [SerializeField]
    AudioClip m_clip = null;

    private Vector3 m_screenPos = Vector3.zero;
    public Vector3 ScreenPos
    {
        get
        {
            return m_screenPos;
        }
        set
        {
            m_screenPos = value;

            value.x *= m_parentCanvas.rect.size.x;
            value.y *= m_parentCanvas.rect.size.y;

            m_transform.anchoredPosition = value + m_anchorOffset;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_uiManager = UIManager.Instance;
        m_transform = GetComponent<RectTransform>();
        m_playerControls = m_playerController.PlayerControls;
        m_playerInput = m_playerController.GetComponent<PlayerInput>();
        m_eventManager = EventManager.Instance;

        var trianglePos = m_bubbAnchorTransform.position;
        var thisPos = m_transform.position;
        //m_anchorOffset = thisPos - trianglePos;
        //Debug.Log(m_anchorOffset);

        if (m_parentCanvas == null)
        {
            // Get top-most parent.
            Transform parent = transform;
            while (parent.parent != null)
                parent = parent.parent;

            m_parentCanvas = parent.GetComponent<RectTransform>();
        }

        m_yarnDialogueUI.onLineUpdate.AddListener(OnLineUpdate);
        m_yarnDialogueUI.onLineStart.AddListener(OnLineStart);
        m_yarnDialogueUI.onDialogueEnd.AddListener(OnDialogueEnd);
        m_yarnDialogueUI.onDialogueStart.AddListener(OnDialogueStart);
        m_yarnDialogueUI.onOptionsStart.AddListener(OnOptionsStart);
        m_yarnDialogueUI.onOptionsEnd.AddListener(OnOptionsEnd);

        m_optionButtons = m_yarnDialogueUI.optionButtons;
        foreach (var button in m_optionButtons)
        {
            button.interactable = false;
        }
        ScreenPos = -Vector3.one * 2.0f;

        m_animator = GetComponent<Animator>();
        m_blinker.enabled = false;

        m_playerControls.UI.ConfirmDialogue.performed += TriggerNextDialogue;
    }


    private void OnDestroy()
    {

        m_yarnDialogueUI.onLineUpdate.RemoveListener(OnLineUpdate);
        m_yarnDialogueUI.onLineStart.RemoveListener(OnLineStart);
        m_yarnDialogueUI.onDialogueEnd.RemoveListener(OnDialogueEnd);
        m_yarnDialogueUI.onDialogueStart.RemoveListener(OnDialogueStart);
        m_yarnDialogueUI.onOptionsStart.RemoveListener(OnOptionsStart);
        m_yarnDialogueUI.onOptionsEnd.RemoveListener(OnOptionsEnd);

        m_playerControls.UI.ConfirmDialogue.performed -= TriggerNextDialogue;
    }

    public void OnOptionsStart()
    {
        m_firstButton = m_optionButtons[0];
        if (m_firstButton == null)
            return;

        foreach (var button in m_optionButtons)
        {
            button.interactable = button.gameObject.activeSelf;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(m_transform);
        m_uiManager.ToggleInstructions("Options");
        Invoke("SelectFirstButton", 0.08335f * 6.0f);
    }

    private void SelectFirstButton()
    {
        m_firstButton.Select();
    }

    public void OnOptionsEnd()
    {
        foreach (var button in m_optionButtons)
        {
            button.interactable = false;
        }
        m_uiManager.ToggleInstructions("Dialogue");
    }

    public void OnLineUpdate(string word)
    {
        int indexOfColon = word.IndexOf(':');
        string name = word.Substring(0, indexOfColon);
        string text = word.Substring(indexOfColon + 2, word.Length - indexOfColon - 2);

        m_nameText.text = name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_nameText.GetComponent<RectTransform>());

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
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_transform);

                break;
            }

            stringBuilder.Append(c);
            m_text.text = stringBuilder.ToString();
            m_audioSource.pitch = Random.Range(0.9f, 1.1f);
            m_audioSource.PlayOneShot(m_clip);
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

    public void OnLineStart()
    {
        m_text.text = "";
        m_blinker.enabled = false;
        m_animator.SetBool(ANIM_BLINKER_SHOW, false);

        foreach (var button in m_optionButtons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void OnDialogueStart()
    {
        m_yarnDialogueUI.MarkLineComplete();
    }

    public void OnDialogueEnd()
    {
        m_playerControls.UI.Disable();
        m_playerInput.SwitchCurrentActionMap("Player");
        m_playerControls.Player.Enable();
        m_eventManager.TriggerDialogueCompletedEvent();
        m_uiManager.ToggleInstructions("");
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
        {
            m_skipToEnd = true;
        }
    }
}
