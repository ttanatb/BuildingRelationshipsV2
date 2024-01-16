using UnityEngine;
using Utilr.SoGameEvents;

namespace UI
{
    public class BookPlayerPortrait : BookContent
    {
        [SerializeField] private SoGameEvent m_activateCam = null;
        [SerializeField] private SoGameEvent m_deactivateCam = null;

        [SerializeField] private Renderer[] m_renderers = null;

        private void Start()
        {
        }

        public override void SetVisible(bool isVisible)
        {
            if (isVisible)
                m_activateCam.Invoke();
            else m_deactivateCam.Invoke();

            foreach (var r in m_renderers)
            {
                r.enabled = isVisible;
            }
        }
    }
}
