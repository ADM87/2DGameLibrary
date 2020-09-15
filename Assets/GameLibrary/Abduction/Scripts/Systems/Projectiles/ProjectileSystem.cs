using Abduction.Data;
using Abduction.Events;
using Abduction.Extensions;
using Abduction.Pooling;
using UnityEngine;

namespace Abduction.Systems.Projectiles
{
    public enum ProjectileEvents { Spawn, Despawn }

    public class ProjectileSystem : MonoBehaviour
    {
        private static Vector2 OFF_SCREEN = new Vector2(-1000, -1000);

        public static EventEmitter<ProjectileEvents, ProjectileEventData> Events = new EventEmitter<ProjectileEvents, ProjectileEventData>();

        #region Serialized Fields

        [SerializeField]
        private GameObject projectilePrefab;

        #endregion

        #region Member Variables

        private Pool<Projectile> projectilePool;
        private Transform projectileContainer;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            projectilePool = new Pool<Projectile>(CreateProjectile, DestroyProjectile);

            projectileContainer = new GameObject("[ProjectileContainer]").transform;
            projectileContainer.parent = transform;
        }

        private void OnEnable()
        {
            Events.Subscribe(ProjectileEvents.Spawn, OnSpawn);
            Events.Subscribe(ProjectileEvents.Despawn, OnDespawn);
        }

        private void OnDisable()
        {
            Events.Unsubscribe(ProjectileEvents.Spawn, OnSpawn);
            Events.Unsubscribe(ProjectileEvents.Despawn, OnDespawn);
        }

        #endregion

        #region Pool Allocation/Deallocation

        private Projectile CreateProjectile()
        {
            return Instantiate(projectilePrefab).GetComponent<Projectile>();
        }

        private void DestroyProjectile(Projectile projectile)
        {
            Destroy(projectile.gameObject);
        }

        #endregion

        #region Projectile Spawn/Despawn Event Handlers

        private void OnSpawn(ProjectileEventData data)
        {
            Projectile projectile = projectilePool.Take();

            projectile.transform.Reset();
            projectile.transform.parent = projectileContainer;

            projectile.gameObject.SetActive(true);
            projectile.Spawn(
                data.SenderTag,
                data.ProjectileSprite,
                data.ProjectileBurst,
                data.ProjectileOrigin,
                data.ProjectileDirection
            );
        }

        private void OnDespawn(ProjectileEventData data)
        {
            Projectile projectile = data.ProjectileObj;

            projectile.transform.Reset();
            projectile.transform.position = OFF_SCREEN;

            projectile.Despawn();
            projectile.gameObject.SetActive(false);

            projectilePool.Put(projectile);
        }

        #endregion
    }
}
