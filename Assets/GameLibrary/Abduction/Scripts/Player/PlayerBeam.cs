﻿using Abduction.Data;
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
        private LayerMask collisionLayers;

        #endregion

        #region Components

        private SpriteRenderer beamRenderer;
        private BoxCollider2D beamCollider;

        #endregion

        #region Member Variables

        private bool isActive;
        private bool isHoldingTile;

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

        private void OnEnable()
        {
            TileWorld.Events.Subscribe(TileWorldEvents.SetTilePickUp, OnSetTilePickUp);
        }

        private void OnDisable()
        {
            TileWorld.Events.Unsubscribe(TileWorldEvents.SetTilePickUp, OnSetTilePickUp);
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

        #region Trigger and Collision Handlers

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if ((collisionLayers.value & (1 << collider.gameObject.layer)) > 0)
                PickUpTile();
        }

        #endregion

        #region Tile Pick Up 

        private void PickUpTile()
        {
            if (isHoldingTile)
                return;

            RaycastHit2D hitInfo = Physics2D.CircleCast(transform.position, 0.3f, Aim, 1.5f, collisionLayers.value);
            if (hitInfo.collider != null)
            {
                TileWorld.Events.Dispatch(TileWorldEvents.RequestTilePickUp, new TileWorldEventData
                {
                    TilePosition = hitInfo.point,
                    TileDirection = Aim
                });
            }
        }

        private void OnSetTilePickUp(TileWorldEventData data)
        {
            //isHoldingTile = true;
        }

        #endregion
    }
}
