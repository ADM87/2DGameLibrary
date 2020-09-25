using Abduction.Data;
using Abduction.Events;
using Abduction.Interfaces;
using Abduction.Systems.TileMaps;
using UnityEngine;

namespace Abduction.Player
{
    public class PlayerBeam : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Transform beamSprite;

        [SerializeField]
        private float maxStretchLength;

        [SerializeField]
        private LayerMask collisionLayers;

        #endregion

        #region Components

        private SpriteRenderer beamRenderer;
        private BoxCollider2D beamCollider;
        private SpringJoint2D beamConnector;

        #endregion

        #region Member Variables

        private IGrabbable currentGrabbable;
        private bool isActive;

        #endregion

        #region Properties

        public Vector2 Aim { get; set; }

        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (!value && currentGrabbable != null)
                    DropTile();

                isActive = value;
                beamCollider.enabled = isActive;
                beamRenderer.enabled = isActive;
                beamConnector.enabled = isActive;
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

            beamConnector = GetComponent<SpringJoint2D>();
            beamConnector.enabled = false;
        }

        #endregion

        #region Updates

        private void Update()
        {
            if (isActive)
            {
                if (currentGrabbable == null)
                {
                    Vector2 direction = Aim.normalized;

                    float degrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, degrees + 90);
                }
                else
                {
                    Vector3 delta = currentGrabbable.transform.position - transform.position;
                    float degrees = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

                    beamRenderer.transform.rotation = Quaternion.Euler(0, 0, degrees + 90);

                    float magnitude = delta.magnitude;

                    if (magnitude > maxStretchLength)
                    {
                        DropTile();
                    }
                    else
                    {
                        Vector3 scale = beamRenderer.transform.localScale;
                        beamRenderer.transform.localScale = new Vector3(scale.x, magnitude, scale.z);

                        Color color = beamRenderer.color;
                        color.a = 1 - (magnitude / maxStretchLength);

                        beamRenderer.color = color;
                        beamConnector.distance = Mathf.Clamp(beamConnector.distance, 0, beamConnector.connectedAnchor.magnitude);
                    }
                }
            }
        }

        #endregion

        #region Trigger and Collision Handlers

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if ((collisionLayers.value & (1 << collider.gameObject.layer)) > 0)
                GlobalEvents.OnPlayerBeamTrigger?.Invoke(this, collider.gameObject.layer);
        }

        #endregion

        #region Pick Up / Drop

        public void PickUp(IGrabbable grabbable)
        {
            if (currentGrabbable != null)
            {
                // TODO - What should happen if the beam is told to pick something up when it is already holding something?
                grabbable.OnDropped();
                return;
            }
            grabbable.OnPickedUp();

            beamConnector.connectedBody = grabbable.GrabbableBody;
            currentGrabbable = grabbable;

            beamCollider.enabled = false;
        }

        private void DropTile()
        {
            currentGrabbable.OnDropped();
            currentGrabbable = null;

            beamRenderer.transform.localScale = Vector3.one;
            beamRenderer.transform.localRotation = Quaternion.identity;

            beamConnector.connectedBody = null;

            Color color = beamRenderer.color;
            color.a = 1;

            beamRenderer.color = color;
        }

        #endregion
    }
}
