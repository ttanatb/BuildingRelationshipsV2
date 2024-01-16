using Cam.Events;
using Cinemachine;
using UnityEngine;
using Utilr.Attributes;

namespace Cam
{
    [RequireComponent(typeof(CinemachineVirtualCameraBase))]
    public class SlSwitchToPrevCam : MonoBehaviour
    {
        [IncludeAllAssetsWithType]
        [SerializeField]
        private SwitchToPrevCamEvent[] m_switchToPrevCamEvents = null;

        private CinemachineVirtualCameraBase m_cam = null;
        
        private void Start()
        {
            foreach (var e in m_switchToPrevCamEvents)
            {
                e.Event.AddListener(OnSwitchToPrevCam);
            }

            TryGetComponent(out m_cam);
        }

        private void OnDestroy()
        {
            foreach (var e in m_switchToPrevCamEvents)
            {
                e.Event.RemoveListener(OnSwitchToPrevCam);
            }
        }

        private void OnSwitchToPrevCam()
        {
            m_cam.Priority -= 2;
        }
    }
}
