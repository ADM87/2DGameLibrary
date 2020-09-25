using Abduction.Systems.TileMaps;
using UnityEngine;

namespace Abduction.Data
{
    public class TileWorldEventData
    {
        public float TileExplosionRadius { get; set; }

        public Vector2 TilePosition { get; set; }
        public Vector2 TileDirection { get; set; }

        public PhysicsTile TileObject { get; set; }
    }
}
