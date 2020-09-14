using Abduction.Data;
using Abduction.Systems.Projectiles;
using System.Collections;
using UnityEngine;

namespace Abduction.Player
{
    public class PlayerLaser : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private float cooldown;

        #endregion

        #region Member Variables

        private WaitForSeconds cooldownYield;
        private Coroutine cooldownRoutine;

        private bool isFiring;
        private bool onCooldown;

        #endregion

        #region Properties

        public Sprite LaserSprite { get; set; }
        public Sprite BurstSprite { get; set; }

        public Vector2 Aim { get; set; }

        public string SenderTag { get; set; }

        public bool IsFiring
        {
            get { return isFiring; }
            set
            {
                if (Aim.magnitude == 0)
                    return;

                if (!onCooldown && value)
                    Fire();
            }
        }

        #endregion

        #region Life Cycle

        private void Awake()
        {
            cooldownYield = new WaitForSeconds(cooldown);
        }

        #endregion

        #region Firing Methods

        private void Fire()
        {
            ProjectileSystem.Events.Dispatch(ProjectileEvents.Spawn, new ProjectileEventData
            {
                SenderTag = SenderTag,
                ProjectileSprite = LaserSprite,
                ProjectileBurst = BurstSprite,
                ProjectileOrigin = transform.position,
                ProjectileDirection = Aim
            });

            if (cooldownRoutine != null)
                StopCoroutine(cooldownRoutine);

            cooldownRoutine = StartCoroutine(LaserCooldown());
        }

        private IEnumerator LaserCooldown()
        {
            // Laser is now firing.
            isFiring = true;
            onCooldown = true;

            yield return cooldownYield;

            // Laser is no longer firing.
            isFiring = false;
            onCooldown = false;

            cooldownRoutine = null;
        }

        #endregion
    }
}
