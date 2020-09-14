using UnityEngine;

namespace Abduction.Player
{
    public class PlayerBeam : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Transform beamSprite;

        #endregion

        #region Components

        private SpriteRenderer beamRenderer;
        private BoxCollider2D beamCollider;

        #endregion

        #region Member Variables

        private bool isActive;

        #endregion

        #region Properties

        public Vector2 Aim { get; set; }

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

        #endregion

        #region Life Cycle

        private void Awake()
        {
            beamRenderer = beamSprite.GetComponent<SpriteRenderer>();
            beamRenderer.enabled = false;

            beamCollider = GetComponent<BoxCollider2D>();
            beamCollider.enabled = false;
        }

        #endregion

        #region Updates

        private void Update()
        {
            if (isActive)
            {
                Vector2 direction = Aim.normalized;

                float degrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, degrees + 90);
            }
        }

        #endregion
    }
}
