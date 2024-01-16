using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRotator : MonoBehaviour
{

    [SerializeField] private InputAction m_rotateInput = null;
    [SerializeField] private float m_rotateFactor = 1.0f;
    
    // Start is called before the first frame update
    private void Start()
    {
        m_rotateInput.Enable();
        m_rotateInput.performed += Rotate;
    }

    private void OnDestroy()
    {
        m_rotateInput.performed -= Rotate;
    }

    private void Rotate(InputAction.CallbackContext ctx)
    {
        var inputValue = ctx.ReadValue<Vector2>();
        Debug.Log($"Rotate: {inputValue}");
        transform.Rotate(Vector3.up, inputValue.x * m_rotateFactor * Time.deltaTime);
    }
}
