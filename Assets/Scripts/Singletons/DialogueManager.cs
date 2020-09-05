using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    public Yarn.Unity.DialogueRunner DialogueRunner { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        DialogueRunner = GetComponent<Yarn.Unity.DialogueRunner>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
