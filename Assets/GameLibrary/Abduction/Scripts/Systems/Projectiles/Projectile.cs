using Abduction.Data;
using Abduction.Interfaces;
using Abduction.Systems.TileMaps;
using System.Collections;
using UnityEngine;

namespace Abduction.Systems.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float burstTime;

        [SerializeField]
        private float burstRadius;

        [SerializeField]
        private float speed;

        [SerializeField]
        private float lifeSpan;

        [SerializeField]
        private LayerMask collisionLayers;

        #endregion

        #region Components

        private SpriteRenderer projectileRenderer;
        private BoxCollider2D projectileCollider;
        private Rigidbody2D projectileBody;

        #endregion

        #region Member Variables

        private Sprite burstSprite;

        private WaitForSeconds burstWait;
        private WaitForSeconds lifeWait;

        private Coroutine burstRoutine;
        private Coroutine lifeRoutine;

        private string senderTag;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            projectileRenderer = GetComponent<SpriteRenderer>();
            projectileCollider = GetComponent<BoxCollider2D>();
            projectileBody = GetComponent<Rigidbody2D>();

            burstWait = new WaitForSeconds(burstTime);
            lifeWait = new WaitForSeconds(lifeSpan);
        }

        #endregion

        #region Trigger and Collision Handlers

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.tag == senderTag)
                return;

            if ((collisionLayers.value & (1 << collider.gameObject.layer)) > 0)
                Burst();
        }

        #endregion

        #region Spawning / Despawning Methods

        public void Spawn(string sender, Sprite sprite, Sprite burst, Vector2 origin, Vector2 direction)
        {
            senderTag = sender;

            projectileRenderer.sprite = sprite;
            transform.position = origin;
            burstSprite = burst;

            projectileBody.simulated = true;
            projectileCollider.enabled = true;

            float width = sprite.rect.width / sprite.pixelsPerUnit;
            float height = sprite.rect.height / sprite.pixelsPerUnit;

            projectileCollider.size = new Vector2(width, height) * 0.75f;
            projectileBody.velocity = direction.normalized * speed;

            float degrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, degrees + 90);

            BeginLifeCountDown();
        }

        public void Despawn()
        {
            senderTag = null;
            burstSprite = null;
            projectileRenderer.sprite = null;
            projectileCollider.size = Vector2.one * 0.0001f;
            projectileBody.velocity = Vector2.zero;
        }

        #endregion

        #region Life Span Timer

        public void BeginLifeCountDown()
        {
            if (lifeRoutine != null)
                StopCoroutine(lifeRoutine);

            StartCoroutine(LifeCountDown());
        }

        private IEnumerator LifeCountDown()
        {
            yield return lifeWait;
            Burst();
        }

        #endregion

        #region Burst Methods

        private void Burst()
        {
            if (burstRoutine != null)
                StopCoroutine(burstRoutine);

            StartCoroutine(ProjectileBurst());
        }

        private IEnumerator ProjectileBurst()
        {
            if (lifeRoutine != null)
                StopCoroutine(lifeRoutine);

            projectileRenderer.sprite = burstSprite;

            float rotation = Random.Range(0, 360);
            transform.localEulerAngles = new Vector3(0, 0, rotation);

            projectileBody.velocity = Vector2.zero;
            projectileBody.simulated = false;
            projectileCollider.enabled = false;

            yield return burstWait;

            burstRoutine = null;

            // Explode is complete, time to despawn.
            ProjectileSystem.Events.Dispatch(ProjectileEvents.Despawn, new ProjectileEventData { ProjectileObj = this });
        }

        #endregion
    }
}
