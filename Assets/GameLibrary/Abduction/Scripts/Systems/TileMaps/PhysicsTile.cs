﻿using System.Collections;
using UnityEngine;

namespace Abduction.Systems.TileMaps
{
    public class PhysicsTile : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float lifeSpan;

        #endregion

        #region Member Variables

        private WaitForSeconds lifeWait;
        private Coroutine lifeRoutine;

        #endregion

        #region Components

        private SpriteRenderer tileRenderer;
        private BoxCollider2D tileCollider;

        #endregion

        #region Properties

        public Rigidbody2D TileBody { get; private set; }

        #endregion

        #region Life Cycle

        private void Awake()
        {
            tileRenderer = GetComponent<SpriteRenderer>();
            tileCollider = GetComponent<BoxCollider2D>();
            TileBody = GetComponent<Rigidbody2D>();

            lifeWait = new WaitForSeconds(lifeSpan);
        }

        #endregion

        #region Spawning / Despawning

        public void Spawn(Sprite sprite, Vector2 size)
        {
            tileCollider.enabled = true;
            TileBody.simulated = true;

            tileRenderer.sprite = sprite;
            tileCollider.size = size;
        }

        public void Despawn()
        {
            tileCollider.enabled = false;
            TileBody.simulated = false;

            tileRenderer.sprite = null;
            tileCollider.size = Vector2.one * 0.001f;
        }

        public void Drop()
        {
            BeginLifeCountDown();
        }

        #endregion

        #region Life Span Timer

        private void BeginLifeCountDown()
        {
            if (lifeRoutine != null)
                StopCoroutine(lifeRoutine);

            lifeRoutine = StartCoroutine(LifeCountDown());
        }

        private IEnumerator LifeCountDown()
        {
            yield return lifeWait;

            lifeRoutine = null;

            TileWorld.Events.Dispatch(TileWorldEvents.DespawnPhysicsTile, new Data.TileWorldEventData { TileObject = this });
        }

        #endregion
    }
}