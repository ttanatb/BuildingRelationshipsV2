using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Yarn.Unity.DialogueRunner))]
public class DialogueManager : MonoBehaviour
{
    private static Yarn.Unity.DialogueRunner dialogueRunner_ = null;
    public static Yarn.Unity.DialogueRunner DialogueRunner
    {
        get
        {
            if (dialogueRunner_ != null)
            {
                return dialogueRunner_;
            }

            dialogueRunner_ = FindObjectOfType<Yarn.Unity.DialogueRunner>();
            return dialogueRunner_;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
}
