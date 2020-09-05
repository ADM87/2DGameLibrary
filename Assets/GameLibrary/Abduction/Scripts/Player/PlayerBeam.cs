using UnityEngine;

namespace Abduction.Player
{
    public class PlayerBeam : MonoBehaviour
    {
        private SpriteRenderer beamRenderer;
        private BoxCollider2D beamCollider;

        private bool isActive;

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                beamCollider.enabled = isActive;
                beamRenderer.enabled = isActive;
            }
        }

        public Sprite BeamSprite
        {
            get { return beamRenderer.sprite; }
            set { beamRenderer.sprite = value; }
        }

        private void Awake()
        {
            beamRenderer = GetComponent<SpriteRenderer>();
            beamRenderer.enabled = false;

            beamCollider = GetComponent<BoxCollider2D>();
            beamCollider.enabled = false;
        }
    }
}
