 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEndgameIfCollideWithPlayer : MonoBehaviour
{
    [SerializeField]
    LayerMask playerMask;

    [SerializeField]
    GameObject gameObjectToAct;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerMask ==
            (playerMask | (1 << other.gameObject.layer)))
        {
            gameObjectToAct.SetActive(true);
        }
    }

}
