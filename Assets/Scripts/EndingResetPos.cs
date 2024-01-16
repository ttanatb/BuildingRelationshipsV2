using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingResetPos : MonoBehaviour
{
    [SerializeField]
    string eventName = "resetPlayerPosition";
    [SerializeField]
    GameObject player = null;

    [SerializeField]
    GameObject blackScreen = null;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddEventFlagListener(RestPosOnFlag);
    }

    private void RestPosOnFlag(string name)
    {
        if (name != eventName)
            return;

        player.transform.position = transform.position;
        blackScreen.SetActive(false);
    }
}
