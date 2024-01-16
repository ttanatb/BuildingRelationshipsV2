using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;
using Utilr.SoGameEvents;
using Utilr.Structs;
using Utilr.Utility;

namespace GameEvents
{
    public class SlMoveCameraAlongDolly : MonoBehaviour
    {
        [SerializeField] private SoGameEvent m_gameEvent = null;
        [SerializeField] private LerpAnimData<float> m_anim = new LerpAnimData<float>()
        {
            Duration = 1.2f,
            Curve = AnimationCurve.EaseInOut(0,0,1,1),
            InitialValue = 0,
            FinalValue = 1,
        };
        private CinemachineVirtualCamera m_cam = null;
        private CinemachineTrackedDolly m_dolly = null;

        private void Start()
        {
            TryGetComponent(out m_cam);
            var bodyComponent = m_cam.GetCinemachineComponent(CinemachineCore.Stage.Body);
            Assert.IsTrue(bodyComponent is CinemachineTrackedDolly);
            m_dolly = bodyComponent as CinemachineTrackedDolly;
            
            m_gameEvent.Event.AddListener(OnEventTriggered);
        }
        
        private void OnEventTriggered()
        {
            StopAllCoroutines();
            var anim = m_anim;
            if (m_dolly.m_PathPosition > anim.FinalValue - float.Epsilon)
            {
                anim.InitialValue = m_anim.FinalValue;
                anim.FinalValue = 0.0f;
            }
            StartCoroutine(Helper.LerpOverTime(anim, Mathf.Lerp, value => {
                m_dolly.m_PathPosition = value;
            }, null));
        }


    }
}
