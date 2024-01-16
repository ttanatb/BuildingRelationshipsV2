using System.Collections;
using NaughtyAttributes;
using Sound.SO;
using Sound.Struct;
using UnityEngine;
using Utilr;

public class AudioManager : MonoBehaviour
{
    [Expandable] [SerializeField] private AudioConfig m_audioConfig = null;
    [SerializeField] private int m_audioPoolCount = 32;
    [SerializeField] private GameObject m_audioSourcePrefab = null;

    [SerializeField] private PlayOneShotAudioEvent m_playOneShotAudioEvent = null; 
    
    private ObjectPool<AudioSource> m_audioPool = null;

    private void Awake()
    {
        Debug.Log($"{typeof(AudioSource).IsSubclassOf(typeof(MonoBehaviour))}");
        m_audioPool = new ObjectPool<AudioSource>(m_audioSourcePrefab, m_audioPoolCount, transform);
    }

    private void Start()
    {
        m_playOneShotAudioEvent.Event.AddListener(OnOneShotAudio);
    }

    private void OnDestroy()
    {
        m_playOneShotAudioEvent.Event.RemoveListener(OnOneShotAudio);
    }

    private void OnOneShotAudio(OneShotAudioData data)
    {
        (var audioSrc, int index) = m_audioPool.GetNextAvailable();
        audioSrc.enabled = true;
        audioSrc.clip = data.Clip;
        audioSrc.loop = false;
        audioSrc.volume = data.Volume * m_audioConfig.SfxVolume * m_audioConfig.MasterVolume;
        audioSrc.pitch = data.Pitch;
        audioSrc.Play();

        Debug.Log($"Playing audio: {data.Clip} at volume {audioSrc.volume} pool index {index}");
        StartCoroutine(ReleaseToObjPool(data.Clip.length, index));
    }

    private IEnumerator ReleaseToObjPool(float waitSec, int index)
    {
        yield return new WaitForSeconds(waitSec);
        m_audioPool.Get(index).enabled = false;
        m_audioPool.SetAvailable(index);
    }
}
