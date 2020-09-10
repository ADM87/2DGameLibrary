using Abduction.Systems.Projectiles;
using UnityEngine;

namespace Abduction.Data
{
    public class ProjectileEventData
    {
        public string SenderTag { get; set; }
        public Projectile ProjectileObj { get; set; }

        public Sprite ProjectileSprite { get; set; }
        public Sprite ProjectileBurst { get; set; }

        public Vector2 ProjectileOrigin { get; set; }
        public Vector2 ProjectileDirection { get; set; }
    }
}
