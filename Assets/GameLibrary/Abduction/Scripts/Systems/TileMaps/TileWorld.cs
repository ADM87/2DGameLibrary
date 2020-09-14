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

        #region Components

        private Grid grid;

        #endregion

        #region Member Variables

        private TileMapGenerator mapGenerator;

        #endregion

        #region Properties

        public Rect Bounds { get; private set; }

        #endregion

        #region Life Cycle

        private void Awake()
        {
            grid = GetComponent<Grid>();
            mapGenerator = new TileMapGenerator();
            transform.position = new Vector3(mapSettings.WorldOffset.x, mapSettings.WorldOffset.y, 0);

            SetBounds();
        }

        private void Start()
        {
            mapGenerator.GenerateMap(GetComponentInChildren<Tilemap>(), mapSettings);
        }

        #endregion

        #region Editor Methods

        private void OnDrawGizmos()
        {
            if (grid == null)
                grid = GetComponent<Grid>();

            if (mapSettings != null)
            {
                Gizmos.color = Color.white;

                float z = transform.position.z;

                SetBounds();

                Gizmos.DrawLine(new Vector3(Bounds.xMin, Bounds.yMin, z), new Vector3(Bounds.xMax, Bounds.yMin, z));
                Gizmos.DrawLine(new Vector3(Bounds.xMax, Bounds.yMin, z), new Vector3(Bounds.xMax, Bounds.yMax, z));
                Gizmos.DrawLine(new Vector3(Bounds.xMax, Bounds.yMax, z), new Vector3(Bounds.xMin, Bounds.yMax, z));
                Gizmos.DrawLine(new Vector3(Bounds.xMin, Bounds.yMax, z), new Vector3(Bounds.xMin, Bounds.yMin, z));
            }
        }

        #endregion

        private void SetBounds()
        {
            float width = mapSettings.MapSize.x * grid.cellSize.x;
            float height = mapSettings.MapSize.y * grid.cellSize.y;

            float left = transform.position.x - width * 0.5f;
            float top = transform.position.y - height * 0.5f + grid.cellSize.y;

            Bounds = new Rect(left, top, width, height);
        }
    }
}
