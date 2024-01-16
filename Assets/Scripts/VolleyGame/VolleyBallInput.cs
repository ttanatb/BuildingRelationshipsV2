using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using Utilr;
using VolleyGame.Events;

namespace VolleyGame
{
    public class VolleyBallInput : MonoBehaviour
    {
        [SerializeField] private LayerMask m_playerLayer = 0;
        [SerializeField] private InputActionReference m_hitInput = null;
        [SerializeField] private HitVolleyEvent m_hitVolleyEvent = null;

        private bool m_isPlayerInTrigger = false;
        
        private void Start()
        {
            m_hitInput.action.performed += OnHitAction;
        }

        private void OnDestroy()
        {
            m_hitInput.action.performed -= OnHitAction;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!m_playerLayer.ContainsLayer(other.gameObject.layer))
                return;

            m_isPlayerInTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!m_playerLayer.ContainsLayer(other.gameObject.layer))
                return;

            m_isPlayerInTrigger = false;
        }

        private void OnHitAction(InputAction.CallbackContext ctx)
        {
            if (!m_isPlayerInTrigger)
                return;
            
            m_hitVolleyEvent.Invoke();
        }
    }
}
