using System.Collections;
using NaughtyAttributes;
using Settings.SO;
using Settings.Structs;
using Sound.SO;
using Sound.Struct;
using UnityEngine;
using Util;
using Utilr;
using Utilr.Attributes;

public class AudioManager : MonoBehaviour
{
    [Expandable] [SerializeField] private AudioConfig m_audioConfig = null;
    [SerializeField] private int m_audioPoolCount = 32;
    [SerializeField] private GameObject m_audioSourcePrefab = null;

    [SerializeField] private PlayOneShotAudioEvent m_playOneShotAudioEvent = null;
    [SerializeField] [IncludeAllAssetsWithType]
    private PlayOneShotRandomAudioClipEvent[] m_playOneShotRandomAudioClipEvents = null;
    [SerializeField] private SettingsUpdateEvent m_settingsUpdateEvent = null;
    
    private ObjectPool<AudioSource> m_audioPool = null;

    private void Awake()
    {
        m_audioPool = new ObjectPool<AudioSource>(m_audioSourcePrefab, m_audioPoolCount, transform);
    }

    private void Start()
    {
        m_playOneShotAudioEvent.Event.AddListener(OnOneShotAudio);
        m_settingsUpdateEvent.Event.AddListener(OnSettingsUpdated);
        foreach (var e in m_playOneShotRandomAudioClipEvents)
            e.Event.AddListener(OnOneShotRandomClip);
    }


    private void OnDestroy()
    {
        m_playOneShotAudioEvent.Event.RemoveListener(OnOneShotAudio);
        m_settingsUpdateEvent.Event.RemoveListener(OnSettingsUpdated);
        foreach (var e in m_playOneShotRandomAudioClipEvents)
            e.Event.RemoveListener(OnOneShotRandomClip);
    }
    
    private void OnSettingsUpdated(SettingsData data)
    {
        m_audioConfig.Data = data.AudioSettingsData;
    }
    
    private void OnOneShotRandomClip(OneShotRandomAudioClipData data)
    {
        OnOneShotAudio(new OneShotAudioData()
        {
            Clip = data.Clip[Random.Range(0, data.Clip.Length)],
            Pitch = data.Pitch.RandomInRange(),
            Volume = data.Volume.RandomInRange(),
        });
    }

    private void OnOneShotAudio(OneShotAudioData data)
    {
        (var audioSrc, int index) = m_audioPool.GetNextAvailable();
        audioSrc.enabled = true;
        audioSrc.clip = data.Clip;
        audioSrc.loop = false;
        audioSrc.volume = data.Volume * m_audioConfig.Data.SfxVolume * m_audioConfig.Data.MasterVolume;
        audioSrc.pitch = data.Pitch;
        audioSrc.Play();

        // Debug.Log($"Playing audio: {data.Clip} at volume {audioSrc.volume} pool index {index}");
        StartCoroutine(ReleaseToObjPool(data.Clip.length, index));
    }

    private IEnumerator ReleaseToObjPool(float waitSec, int index)
    {
        yield return new WaitForSeconds(waitSec);
        m_audioPool.Get(index).enabled = false;
        m_audioPool.SetAvailable(index);
    }
}
