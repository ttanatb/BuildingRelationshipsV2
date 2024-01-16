using System.Collections.Generic;
using Dialogue.SO;
using Febucci.UI.Core;
using Sound.Struct;
using UnityEngine;

namespace Dialogue
{
    [RequireComponent(typeof(TypewriterCore))]
    public class DialogueAudioHandler : MonoBehaviour
    {
        private static readonly HashSet<char> SKIP_AUDIO_CHARS = new HashSet<char>() { ' ', ','}; 
        
        [SerializeField] private AudioClip m_clip = null;
        
        [NaughtyAttributes.MinMaxSlider(0f, 2f)]
        [SerializeField] private Vector2 m_pitchRange = new Vector2(0.9f, 1.1f);
        
        [NaughtyAttributes.MinMaxSlider(0f, 2f)]
        [SerializeField] private Vector2 m_volumeRange = new Vector2(0.2f, 0.22f);
        
        [SerializeField] private PlayOneShotAudioEvent m_playOneShotAudioEvent = null;
        [SerializeField] private ChangeDialogueAudioEvent m_changeAudioEvent = null;

        private TypewriterCore m_typewriter = null;
        
        private void Start()
        {
            TryGetComponent(out m_typewriter);
            m_typewriter.onCharacterVisible.AddListener(OnCharacterVisible);
            m_changeAudioEvent.Event.AddListener(OnChangeAudioClip);
        }


        private void OnDestroy()
        {
            m_typewriter.onCharacterVisible.RemoveListener(OnCharacterVisible);
            m_changeAudioEvent.Event.RemoveListener(OnChangeAudioClip);
        }

        private void OnCharacterVisible(char character)
        {
            if (SKIP_AUDIO_CHARS.Contains(character)) return;
            
            m_playOneShotAudioEvent.Invoke(new OneShotAudioData()
            {
                Clip = m_clip,
                Pitch = Random.Range(m_pitchRange.x, m_pitchRange.y),
                Volume = Random.Range(m_volumeRange.x, m_volumeRange.y),
            });
        }
        
        private void OnChangeAudioClip(AudioClip clip)
        {
            m_clip = clip;
        }
    }
}
