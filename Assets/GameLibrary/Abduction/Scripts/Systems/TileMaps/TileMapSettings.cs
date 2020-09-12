using UnityEngine;

namespace Abduction.Systems.TileMaps
{
    [CreateAssetMenu(fileName = "TileMapSettings", menuName = "Abduction/Tile Map/Tile Map Settings", order = 1)]
    public class TileMapSettings : ScriptableObject
    {
        [SerializeField]
        private Vector2Int mapSize;
        public Vector2Int MapSize { get { return mapSize; } }

        [SerializeField]
        private float fillPercentage;
        public float FillPercentage { get { return fillPercentage; } }

        [SerializeField]
        private int smoothingIterations;
        public int SmoothingIterations { get { return smoothingIterations; } }

        [SerializeField]
        private TileData[] tileSet;
        public TileData[] TileSet { get { return tileSet; } }
    }
}
