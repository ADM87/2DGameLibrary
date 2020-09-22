using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Abduction.Systems.TileMaps
{
    public class PhysicsTile : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float lifeSpan;

        [SerializeField]
        private float fadeOutTime;

        #endregion

        #region Member Variables

        private WaitForSeconds lifeWait;
        private WaitUntil fadeWait;

        private Coroutine lifeRoutine;
        private Coroutine fadeRoutine;

        private float fadeElapsed;

        #endregion

        #region Components

        private SpriteRenderer tileRenderer;
        private BoxCollider2D tileCollider;
        private ShadowCaster2D tileShadowCaster;

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
            tileShadowCaster = GetComponent<ShadowCaster2D>();

            lifeWait = new WaitForSeconds(lifeSpan);
            fadeWait = new WaitUntil(() =>
            {
                fadeElapsed = Mathf.Clamp(fadeElapsed + Time.deltaTime, 0, fadeOutTime);
                tileRenderer.material.SetFloat("_Fade", 1 - (fadeElapsed / fadeOutTime));
                return fadeElapsed == fadeOutTime;
            });
        }

        #endregion

        #region Spawning / Despawning

        public void Spawn(Sprite sprite, Vector2 size)
        {
            tileCollider.enabled = true;
            TileBody.simulated = true;
            tileShadowCaster.enabled = true;

            tileRenderer.sprite = sprite;
            tileCollider.size = size * 0.9f;

            tileRenderer.material.SetFloat("_Fade", 1);
        }

        public void Despawn()
        {
            tileCollider.enabled = false;
            TileBody.simulated = false;
            tileShadowCaster.enabled = false;

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

            tileShadowCaster.enabled = false;

            FadeOut();
        }

        #endregion

        #region Fade Out

        private void FadeOut()
        {
            if (fadeRoutine != null)
                StopCoroutine(fadeRoutine);

            fadeRoutine = StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            fadeElapsed = 0;

            yield return fadeWait;

            tileCollider.enabled = false;
            TileBody.simulated = false;

            TileWorld.Events.Dispatch(TileWorldEvents.DespawnPhysicsTile, new Data.TileWorldEventData { TileObject = this });
        }

        #endregion
    }
}
