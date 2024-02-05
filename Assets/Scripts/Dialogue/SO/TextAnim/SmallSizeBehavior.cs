using Febucci.UI.Core;
using Febucci.UI.Effects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialogue.SO.TextAnim
{
    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(menuName = "Text Animator/Animations/Behaviors/SmallSize", fileName = "Small Size Behavior")]
    [EffectInfo("small", EffectCategory.Behaviors)]
    public class SmallSizeBehavior : BehaviorScriptableBase
    {
        private const int MAX_DIRECTIONS = 23;

        [field: SerializeField]
        public float BaseSize { get; set; } = 1.0f;

        [field: SerializeField]
        public float BasePosY { get; set; } = -1.0f;
        
        [field: SerializeField]
        public float BaseAmplitude { get; set; } = 1;
        
        [field: SerializeField]
        public float BaseFrequency { get; set; } = 1;
        
        [field: SerializeField]
        [field: UnityEngine.Range(0,1)] 
        public float BaseWaveSize { get; set; } = .2f;

        private float m_size = 1.0f;
        private float m_amplitude = 1f;
        private float m_frequency = 1f;
        private float m_waveSize = .2f;

        
        private Vector3[] m_directions = null;
        private int m_indexCache;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            
            m_directions = new Vector3[MAX_DIRECTIONS];

            //Calculates a random direction for each character (which won't change)
            for(int i = 0; i < MAX_DIRECTIONS; i++)
            {
                m_directions[i] = TextUtilities.FakeRandoms[Random.Range(0, TextUtilities.fakeRandomsCount - 1)] 
                    * Mathf.Sign(Mathf.Sin(i));
            }
        }

        public override void ResetContext(TAnimCore animator)
        {
            m_size = BaseSize;
            m_amplitude = BaseAmplitude;
            m_frequency = BaseFrequency;
            m_waveSize = BaseWaveSize;
        }
        
        public override void ApplyEffectTo(ref CharacterData character, TAnimCore animator)
        {
            character.current.positions.LerpUnclamped(
                character.current.positions.GetMiddlePos(),
                m_size);
            character.current.positions.MoveChar(Vector3.down * BasePosY);
            
            m_indexCache = character.index % MAX_DIRECTIONS;
            character.current.positions.MoveChar(
                m_directions[m_indexCache] 
                * (Mathf.Sin(animator.time.timeSinceStart * m_frequency + character.index * m_waveSize) 
                    * m_amplitude * character.uniformIntensity));
        }
    }
}
