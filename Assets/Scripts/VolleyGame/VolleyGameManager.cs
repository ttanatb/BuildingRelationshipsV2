using NaughtyAttributes;
using UnityEngine;
using Utilr.SoGameEvents;
using Utilr.Structs;
using VolleyGame.Events;
using VolleyGame.SO;
using VolleyGame.Structs;

namespace VolleyGame
{
    public class VolleyGameManager : Singleton<VolleyGameManager>
    {
        [Expandable]
        [SerializeField] private VolleyConfig m_config = null;
        // When the game starts
        [SerializeField] private StartVolleyGameEvent m_onStartVolleyGameEvent = null;
        
        // Manager invokes this to fire a shot
        [SerializeField] private FireVolleyShotEvent m_fireVolleyShotEvent = null;
        
        // When player hits the ball
        [SerializeField] private HitVolleyEvent m_onVolleyHitEvent = null;

        [SerializeField] private MoveVolleyCharacterEvent m_moveOpponentEvent = null;
        [SerializeField] private SoGameEvent m_fpsCamEvent = null;
        [SerializeField] private SoGameEvent m_wideCamEvent = null;
        [SerializeField] private SoGameEvent m_opponentCloseUpCamEvent = null;

        [SerializeField] private DisplayVolleyTextEvent m_displayOpponentTextEvent = null;
        [SerializeField] private DisplayVolleyTextEvent m_displayPlayerTextEvent = null;
        [SerializeField] private DisplayVolleyTextEvent m_displayFocusTextEvent = null;

        [SerializeField] private string m_yarnNode = "test_volley";

        [SerializeField] private Transform m_playerTransform = null;
        [SerializeField] private Transform m_opponentTransform = null;

        [Button]
        private void FireShotToPlayer()
        {
            m_fireVolleyShotEvent.Invoke(new VolleyShotData()
            {
                StartPos = m_opponentTransform.position,
                EndPos = m_playerTransform.position,
                PeakHeight = m_config.VolleyShotHeightLow,
                Duration = m_config.VolleyShotMediumDuration,
                HorizontalCurve = m_config.VolleyShotHorizontalCurve,
                VerticalCurve = m_config.VolleyShotVerticalCurveSlow,
            });
        }
        
        [Button]
        private void FireShotToOpponent()
        {
            var endPos = m_opponentTransform.position + Vector3.right * 2.0f;
            m_fireVolleyShotEvent.Invoke(new VolleyShotData()
            {
                StartPos = m_playerTransform.position,
                EndPos = endPos,
                PeakHeight = m_config.VolleyShotHeightLow,
                Duration = m_config.VolleyShotMediumDuration,
                HorizontalCurve = m_config.VolleyShotHorizontalCurve,
                VerticalCurve = m_config.VolleyShotVerticalCurveSlow,
            });
            
            m_moveOpponentEvent.Invoke(new VolleyCharacterMovementData()
            {
                AnimData = new LerpAnimData<Vector3>()
                {
                    Duration = m_config.VolleyShotMediumDuration,
                    Curve = m_config.VolleyOpponentMoveCurve,
                    FinalValue = endPos,
                }
            });
        }
    }
}
