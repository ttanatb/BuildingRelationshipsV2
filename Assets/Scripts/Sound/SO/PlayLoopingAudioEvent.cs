using Sound.Structs;
using UnityEngine;
using UnityEngine.Events;
using Utilr.SoGameEvents;

namespace Sound.SO
{
    [CreateAssetMenu(fileName = "PlayLoopingAudioEvent", menuName = "br/Audio/PlayLoopingAudioEvent", order = 1)]
    public class PlayLoopingAudioEvent : SoCustomEvent<LoopingAudioData>
    {
        /// <summary>
        /// Event used to stop the looping audio.
        /// </summary>
        public UnityEvent<LoopingAudioData> StopEvent { get; private set; } = new UnityEvent<LoopingAudioData>();

        public UnityEvent<LoopingAudioData, float> SetVolumeEvent { get; private set; } 
            = new UnityEvent<LoopingAudioData, float>();

        public UnityEvent< LoopingAudioData, float> SetPitchEvent { get; private set; } 
            = new UnityEvent<LoopingAudioData, float>();
        
        
        public void InvokeStop()
        {
            StopEvent.Invoke(Data);
        }

        public void InvokeVolume(float volume)
        {
            SetVolumeEvent.Invoke(Data, volume);
        }

        public void InvokePitch(float pitch)
        {
            SetPitchEvent.Invoke(Data, pitch);
        }
    }
}
