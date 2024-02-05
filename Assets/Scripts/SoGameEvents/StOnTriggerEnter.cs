﻿using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Utilr;
using Utilr.SoGameEvents;

namespace GameEvents
{
    public class StOnTriggerEnter : MonoBehaviour
    {
        [SerializeField] private LayerMask m_mask = new LayerMask();
        [SerializeField] private SoGameEventBase m_eventToFire = null;

        // Start is called before the first frame update
        private void Start()
        {
            var colliders = GetComponentsInChildren<Collider>(true);
            Assert.IsTrue(colliders.Length > 0);

            bool containsTrigger = colliders.Any(col => col.isTrigger);
            Assert.IsTrue(containsTrigger);

            TryGetComponent(out Renderer renderer);
            if (renderer)
                renderer.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            Assert.IsNotNull(m_eventToFire);

            if (!m_mask.ContainsLayer(other.gameObject.layer))
                return;

            m_eventToFire.Invoke();
        }
    }
}
