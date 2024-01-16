using NaughtyAttributes;
using UnityEngine;
using Utilr.Utility;
using VolleyGame.Events;
using VolleyGame.Structs;

namespace VolleyGame
{
    public class VolleyOpponentMovement : MonoBehaviour
    {
        [SerializeField] private MoveVolleyCharacterEvent m_onMoveVolleyCharacterEvent = null;
        [SerializeField] private Animator m_animator = null;
        
        [SerializeField] [AnimatorParam("m_animator")]
        private int m_swingAnimTrigger = 1;
        
        private void Start()
        {
            m_onMoveVolleyCharacterEvent.Event.AddListener(MoveCharacter);
        }
        
        private void OnDestroy()
        {
            m_onMoveVolleyCharacterEvent.Event.RemoveListener(MoveCharacter);
        }

        private void MoveCharacter(VolleyCharacterMovementData data)
        {
            var animData = data.AnimData;
            animData.InitialValue = transform.position;
            
            StartCoroutine(Helper.LerpOverTime(animData, Vector3.Lerp, pos => {
                transform.position = pos;
            }, () => {
               m_animator.SetTrigger(m_swingAnimTrigger); 
            }));
        }
    }
}
