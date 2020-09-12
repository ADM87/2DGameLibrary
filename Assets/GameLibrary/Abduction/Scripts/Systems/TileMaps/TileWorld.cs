using UnityEngine;
using UnityEngine.Tilemaps;

namespace Abduction.Systems.TileMaps
{
    public class TileWorld : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private TileMapSettings mapSettings;

        #endregion

        #region Member Variables

        private TileMapGenerator mapGenerator;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            mapGenerator = new TileMapGenerator();
        }

        private void Start()
        {
            mapGenerator.GenerateMap(GetComponentInChildren<Tilemap>(), mapSettings);
        }

        #endregion
    }
}
