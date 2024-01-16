using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOnEvent : MonoBehaviour
{
    [SerializeField]
    string eventName = "";

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventFlagListener(DisableOnFlag);

    }

    private void DisableOnFlag(string name)
    {
        if (name != eventName)
            return;

        gameObject.SetActive(false);
    }
}
