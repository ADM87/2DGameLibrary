using UnityEngine;

namespace Abduction.Data
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "Abduction/Character/Config", order = 1)]
    public class CharacterConfig : ScriptableObject
    {
        [SerializeField]
        private Sprite characterSprite;
        public Sprite CharacterSprite { get { return characterSprite; } }

        [SerializeField]
        private Sprite characterBeam;
        public Sprite CharacterBeam { get { return characterBeam; } }

        [SerializeField]
        private Sprite characterLaser;
        public Sprite CharacterLaser { get { return characterLaser; } }

        [SerializeField]
        private Sprite characterBurst;
        public Sprite CharacterBurst { get { return characterBurst; } }

        [SerializeField]
        private Sprite characterGroundBurst;
        public Sprite CharacterGroundBurst { get { return characterGroundBurst; } }

        [SerializeField]
        private Sprite characterRing;
        public Sprite CharacterRing { get { return characterRing; } }
    }
}
