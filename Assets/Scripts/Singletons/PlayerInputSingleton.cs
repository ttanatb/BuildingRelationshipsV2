using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputSingleton : Singleton<PlayerInputSingleton>
{
    public PlayerInput PlayerInput { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        PlayerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
