using Abduction.Player;
using Abduction.Systems.Projectiles;
using UnityEngine;

namespace Abduction.Events
{
    public delegate void PlayerBeamTrigger(PlayerBeam playerBeam, int layer);
    public delegate void ProjectileBurstTrigger(Projectile projectile, int layers);

    public static class GlobalEvents
    {
        public static PlayerBeamTrigger OnPlayerBeamTrigger;
        public static ProjectileBurstTrigger OnProjectBurstTrigger;
    }
}
