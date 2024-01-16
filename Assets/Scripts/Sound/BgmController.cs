using System.Collections.Generic;
using NaughtyAttributes;
using Sound.Struct;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Sound
{
    public class BgmController : MonoBehaviour
    {
        [SerializeField] private PlayJumpSoundEvent m_jumpSoundEvent = null;
        [SerializeField] private List<NotesAtPointInSong> m_jumpClips = null;

        [SerializeField] private PlayOneShotAudioEvent m_playOneShotAudioEvent = null;

        [MinMaxSlider(0f, 2f)]
        [SerializeField] private Vector2 m_volumeRange = new Vector2(0.9f, 1.1f);
        
        [MinMaxSlider(0f, 2f)]
        [SerializeField] private Vector2 m_pitchRange = new Vector2(0.9f, 1.1f);

        private int m_clipIndex = 0;
        private AudioSource m_bgmSource = null;
        
        private void Start()
        {
            TryGetComponent(out m_bgmSource);
            m_jumpSoundEvent.Event.AddListener(OnJumpSound);

            for (int i = 0; i < m_jumpClips.Count - 1; i++)
            {
                Assert.IsTrue(m_jumpClips[i].TimestampSec < m_jumpClips[i + 1].TimestampSec);
            }
        }

        private void OnDestroy()
        {
            m_jumpSoundEvent.Event.RemoveListener(OnJumpSound);
        }

        private void OnJumpSound()
        {
            float time = m_bgmSource.time;
            int index = 0;
            for (int i = 0; i < m_jumpClips.Count; i++)
            {
                var curr = m_jumpClips[i];
                if (time < curr.TimestampSec)
                    break;

                index = i;
            }

            var clips = m_jumpClips[index].Clips;
            m_playOneShotAudioEvent.Invoke(new OneShotAudioData()
            {
                Clip = clips[m_clipIndex++ % clips.Length],
                Volume = Random.Range(m_volumeRange.x, m_volumeRange.y),
                Pitch = Random.Range(m_pitchRange.x, m_pitchRange.y)
            });
        }
    }
}
