using System.Collections;
using UnityEngine;
using VolleyGame.Events;
using VolleyGame.Structs;

namespace VolleyGame
{
    public class VolleyBallMovement : MonoBehaviour
    {
        [SerializeField] private FireVolleyShotEvent m_fireVolleyShotEvent = null;

        private void Start()
        {
            m_fireVolleyShotEvent.Event.AddListener(OnFire);
        }

        private void OnDestroy()
        {
            m_fireVolleyShotEvent.Event.RemoveListener(OnFire);
        }

        private void OnFire(VolleyShotData data)
        {
            StartCoroutine(PositionBall(data));
        }

        private IEnumerator PositionBall(VolleyShotData data)
        {
            for (float timer = 0; timer < data.Duration; timer += Time.deltaTime)
            {
                float percent = timer / data.Duration;
                float sampledHorizontalValue = data.HorizontalCurve.Evaluate(percent);
                float sampledVerticalValue = data.VerticalCurve.Evaluate(percent > 0.5f ?
                    2.0f * Mathf.Abs(1.0f - percent) : percent * 2.0f);
                var res = Vector3.Lerp(data.StartPos, data.EndPos, sampledHorizontalValue);
                res.y = Mathf.Lerp(data.StartPos.y, data.StartPos.y + data.PeakHeight, sampledVerticalValue);

                transform.position = res;
                yield return new WaitForEndOfFrame();
            }
            
            transform.position = data.EndPos;
        }
    }
}
