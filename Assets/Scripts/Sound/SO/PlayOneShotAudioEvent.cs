using Sound.Struct;
using UnityEngine;
using Utilr.SoGameEvents;

// TODO: [Cleanup] Consolidate two one shot audio events
[CreateAssetMenu(fileName = "PlayOneShotAudioEvent", menuName = "br/Audio/PlayOneShotAudioEvent", order = 1)]
public class PlayOneShotAudioEvent : SoCustomEvent<OneShotAudioData>
{

}
