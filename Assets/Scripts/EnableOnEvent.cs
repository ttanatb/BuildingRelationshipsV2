using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnEvent : MonoBehaviour
{
    [SerializeField]
    string eventName = "";
    [SerializeField]
    GameObject gameObj = null;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventFlagListener(EnableOnFlag);
        Invoke("DisableGameObject", 1);
    }

    void DisableGameObject()
    {
        gameObj.SetActive(false);
    }

    private void EnableOnFlag(string name)
    {
        if (name != eventName)
            return;

        gameObj.SetActive(true);
    }
}
