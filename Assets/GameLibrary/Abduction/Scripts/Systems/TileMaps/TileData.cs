using UnityEngine;
using UnityEngine.Tilemaps;

namespace Abduction.Systems.TileMaps
{
    [System.Serializable]
    public struct TileData
    {
        [SerializeField]
        private string name;
        public string Name { get { return name; } }

        [SerializeField]
        private TileBase tile;
        public TileBase Tile { get { return tile; } }

        public TileData(string name, TileBase tile)
        {
            this.name = name;
            this.tile = tile;
        }
    }
}
