using Abduction.Player;
using UnityEngine;

namespace Abduction.Events
{
    public delegate void PlayerBeamTrigger(PlayerBeam playerBeam, int layer);

    public static class GlobalEvents
    {
        public static PlayerBeamTrigger OnPlayerBeamTrigger;
    }
}
