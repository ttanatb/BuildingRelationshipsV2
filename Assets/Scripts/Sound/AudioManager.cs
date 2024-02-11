using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Settings.SO;
using Settings.Structs;
using Sound.SO;
using Sound.Struct;
using Sound.Structs;
using UnityEngine;
using Util;
using Utilr;
using Utilr.Attributes;
using Utilr.Utility;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [Expandable] [SerializeField] private AudioConfig m_audioConfig = null;
    [SerializeField] private int m_audioPoolCount = 32;
    [SerializeField] private GameObject m_audioSourcePrefab = null;

    [SerializeField] private PlayOneShotAudioEvent m_playOneShotAudioEvent = null;
    
    [SerializeField] [IncludeAllAssetsWithType]
    private PlayOneShotRandomAudioClipEvent[] m_playOneShotRandomAudioClipEvents = null;
    
    [SerializeField] [IncludeAllAssetsWithType]
    private PlayLoopingAudioEvent[] m_playLoopingAudioEvents = null;

    private Dictionary<AudioClip, ActiveLoopingAudio> m_activeLoopingBgm =
        new Dictionary<AudioClip, ActiveLoopingAudio>();
    
    [SerializeField] private SettingsUpdateEvent m_settingsUpdateEvent = null;
    
    private ObjectPool<AudioSource> m_audioPool = null;

    private void Awake()
    {
        m_audioPool = new ObjectPool<AudioSource>(m_audioSourcePrefab, m_audioPoolCount, transform);
        m_activeLoopingBgm.EnsureCapacity(m_audioPoolCount);
    }

    private void Start()
    {
        m_playOneShotAudioEvent.Event.AddListener(OnOneShotAudio);
        m_settingsUpdateEvent.Event.AddListener(OnSettingsUpdated);
        foreach (var e in m_playOneShotRandomAudioClipEvents)
            e.Event.AddListener(OnOneShotRandomClip);
        foreach (var e in m_playLoopingAudioEvents)
        {
            e.Event.AddListener(OnPlayLoopingClip);
            e.StopEvent.AddListener(OnStopLoopingClip);
            e.SetVolumeEvent.AddListener(OnLoopingClipSetVolume);
            e.SetPitchEvent.AddListener(OnLoopingClipSetPitch);
        }
    }


    private void OnDestroy()
    {
        m_playOneShotAudioEvent.Event.RemoveListener(OnOneShotAudio);
        m_settingsUpdateEvent.Event.RemoveListener(OnSettingsUpdated);
        foreach (var e in m_playOneShotRandomAudioClipEvents)
            e.Event.RemoveListener(OnOneShotRandomClip);
        foreach (var e in m_playLoopingAudioEvents)
        {
            e.Event.RemoveListener(OnPlayLoopingClip);
            e.StopEvent.RemoveListener(OnStopLoopingClip);    
            e.SetVolumeEvent.RemoveListener(OnLoopingClipSetVolume);
            e.SetPitchEvent.RemoveListener(OnLoopingClipSetPitch);
        }
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
            Position = data.Position,
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
        audioSrc.transform.position = data.Position;

        #if UNITY_EDITOR
        audioSrc.gameObject.name = data.Clip.name;
        #endif
        
        // Debug.Log($"Playing audio: {data.Clip} at volume {audioSrc.volume} pool index {index}");
        StartCoroutine(ReleaseToObjPool(data.Clip.length, index));
    }
    
    private void OnPlayLoopingClip(LoopingAudioData data)
    {
        (var audioSrc, int index) = m_audioPool.GetNextAvailable();
        audioSrc.enabled = true;
        audioSrc.clip = data.Clip;
        audioSrc.loop = true;
        audioSrc.volume = 0.0f;
        audioSrc.pitch = 1.0f;
        audioSrc.Play();
        if (data.Target)
            audioSrc.transform.position = data.Target.position;

        #if UNITY_EDITOR
        audioSrc.gameObject.name = data.Clip.name;
        #endif

        IEnumerator routine = null;
        if (data.FadeIn.Duration > float.Epsilon)
        {
            var fadeIn = data.FadeIn;
            fadeIn.InitialValue = 0.0f;
            fadeIn.FinalValue = data.Volume * m_audioConfig.Data.BgmVolume * m_audioConfig.Data.MasterVolume;
            routine = Helper.LerpOverTime(fadeIn, Mathf.Lerp, value => {
                audioSrc.volume = value;
            }, null);
            StartCoroutine(routine);
        }
        else
        {
            audioSrc.volume = data.Volume * m_audioConfig.Data.BgmVolume * m_audioConfig.Data.MasterVolume;
        }

        if (!m_activeLoopingBgm.TryAdd(data.Clip, new ActiveLoopingAudio
            {
                AudioSource = audioSrc,
                PoolIndex = index,
                FadeInCoroutine = routine,
            }))
        {
            Debug.LogError($"BGM pool already contains active {data.Clip.name}");
        }
    }

    private void OnStopLoopingClip(LoopingAudioData data)
    {
        if (!m_activeLoopingBgm.TryGetValue(data.Clip, out var result))
        {
            Debug.LogError($"Trying to stop Audio Clip {data.Clip.name} but clip is not playing.");
            return;
        }

        var audioSource = result.AudioSource;
        if (result.FadeInCoroutine != null)
            StopCoroutine(result.FadeInCoroutine);

        if (data.FadeOut.Duration > float.Epsilon)
        {
            var fadeOut = data.FadeOut;
            fadeOut.InitialValue = audioSource.volume;
            fadeOut.FinalValue = 0.0f;
            StartCoroutine(Helper.LerpOverTime(fadeOut, Mathf.Lerp, value => {
                audioSource.volume = value;
            }, () => {
                m_audioPool.SetAvailable(result.PoolIndex);
                m_activeLoopingBgm.Remove(data.Clip);

                audioSource.Stop();
                audioSource.enabled = false;
            }));
        }
        else
        {
            m_audioPool.SetAvailable(result.PoolIndex);
            m_activeLoopingBgm.Remove(data.Clip);
            
            audioSource.Stop();
            audioSource.enabled = false;
        }
    }

    private void OnLoopingClipSetPitch(LoopingAudioData data, float value)
    {
        if (!m_activeLoopingBgm.TryGetValue(data.Clip, out var result))
        {
            Debug.LogWarning($"Trying to stop Audio Clip {data.Clip.name} but clip is not playing.");
            return;
        }
        
        var audioSource = result.AudioSource;
        audioSource.pitch = value;
    }
    
    private void OnLoopingClipSetVolume(LoopingAudioData data, float value)
    {
        if (!m_activeLoopingBgm.TryGetValue(data.Clip, out var result))
        {
            Debug.LogWarning($"Trying to stop Audio Clip {data.Clip.name} but clip is not playing.");
            return;
        }
        
        var audioSource = result.AudioSource;
        if (result.FadeInCoroutine != null)
            StopCoroutine(result.FadeInCoroutine);
        audioSource.volume = value;
    }
    
    private IEnumerator ReleaseToObjPool(float waitSec, int index)
    {
        yield return new WaitForSeconds(waitSec);
        m_audioPool.Get(index).enabled = false;
        m_audioPool.SetAvailable(index);
    }

    private void Update()
    {
        foreach (var pair in m_activeLoopingBgm)
        {
            if (pair.Value.Target is null) continue;
            
            var source = pair.Value.AudioSource;
            source.transform.position = pair.Value.Target.position;
        }
    }
}
