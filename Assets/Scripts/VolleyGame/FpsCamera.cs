using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using VolleyGame.SO;

namespace VolleyGame
{
    public class FpsCamera : MonoBehaviour
    {
        [SerializeField] [Expandable] private VolleyConfig m_config = null;
        [SerializeField] private InputActionReference m_lookInput = null;

        private Vector2 m_currAngles = Vector2.zero;

        private void Start()
        {
            m_lookInput.action.performed += OnLookAction;
        }

        private void OnDestroy()
        {
            m_lookInput.action.performed -= OnLookAction;
        }
        
        private void OnLookAction(InputAction.CallbackContext ctx)
        {
            var input = ctx.ReadValue<Vector2>();
            m_currAngles = input * m_config.FpsCamSensitivity * m_config.FpsCamInputFactor;
            transform.rotation = Quaternion.AngleAxis(m_currAngles.x, Vector3.right) *
                Quaternion.AngleAxis(m_currAngles.y, Vector3.up);
        }
    }
}
