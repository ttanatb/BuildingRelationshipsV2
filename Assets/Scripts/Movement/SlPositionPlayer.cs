using Dialogue.SO;
using Dialogue.Struct;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using Utilr.Structs;
using Utilr.Utility;

namespace Movement
{
    public class SlPositionPlayer : MonoBehaviour
    {
        [SerializeField]
        [Utilr.Attributes.IncludeAllAssetsWithType]
        private PositionPlayerEvent[] m_positionPlayerEvents = null;

        private PlayerMovement m_playerMovement = null;
        

        private void Start()
        {
            TryGetComponent(out m_playerMovement);
            
            foreach (var evt in m_positionPlayerEvents)
            {
                evt.Event.AddListener(OnPositionPlayer);
            }
        }

        private void OnDestroy()
        {
            foreach (var evt in m_positionPlayerEvents)
            {
                evt.Event.RemoveListener(OnPositionPlayer);
            }
        }
        
        private void OnPositionPlayer(PositionPlayerData data)
        {
            StopAllCoroutines();
            var thisTransform = transform;
            var lerpAnimData = new LerpAnimData<(Vector3 Position, Quaternion Rotation)>()
            {
                Curve = AnimationCurve.EaseInOut(0, 0, 1, 1),
                Duration = data.Duration,
                FinalValue = (data.Transform.position, data.Transform.rotation),
                InitialValue = (thisTransform.position, thisTransform.rotation)
            };
            
            m_playerMovement.StopMovement();

            if (data.Duration < float.Epsilon)
            {
                thisTransform.position = data.Transform.position;
                thisTransform.rotation = data.Transform.rotation;
                StartCoroutine(Helper.ExecuteNextFrame(() => {
                    m_playerMovement.SetFrozen(false);
                }));
                return;
            }
            
            StartCoroutine(
                Helper.LerpOverTime(lerpAnimData, (initial, final, factor) =>
                    (
                        Vector3.Lerp(initial.Position, final.Position, factor), 
                        Quaternion.Lerp(initial.Rotation, final.Rotation, factor)
                    ), 
                    result => {
                thisTransform.position = result.Position;
                thisTransform.rotation = result.Rotation;
            }, () => {
                thisTransform.position = data.Transform.position;
                thisTransform.rotation = data.Transform.rotation;
                StartCoroutine(Helper.ExecuteNextFrame(() => {
                    m_playerMovement.SetFrozen(false);
                }));
            }));
        }
    }
}
