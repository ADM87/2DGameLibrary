using Abduction.Data;
using Abduction.Events;
using Abduction.Extensions;
using Abduction.Pooling;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Abduction.Systems.TileMaps
{
    public enum TileWorldEvents { RequestTilePickUp, SetTilePickUp, DespawnPhysicsTile }

    public class TileWorld : MonoBehaviour
    {
        private static Vector2 OFF_SCREEN = new Vector2(-1000, -1000);

        public static EventEmitter<TileWorldEvents, TileWorldEventData> Events = new EventEmitter<TileWorldEvents, TileWorldEventData>();

        #region Serialized Fields

        [SerializeField]
        private GameObject physicsTilePrefab;

        [SerializeField]
        private Tilemap environment;

        [SerializeField]
        private Tilemap background;

        [SerializeField]
        private TileMapSettings environmentSettings;

        [SerializeField]
        private TileMapSettings backgroundSettings;

        #endregion

        #region Components

        private Grid grid;

        #endregion

        #region Member Variables

        private TileMapGenerator mapGenerator;
        private Pool<PhysicsTile> physicsTiles;
        private Transform physicsTileContainer;

        #endregion

        #region Properties

        public Rect Bounds { get; private set; }

        #endregion

        #region Life Cycle

        private void Awake()
        {
            physicsTiles = new Pool<PhysicsTile>(AllocatePhysicsTile, DeallocatePhysicsTile);

            physicsTileContainer = new GameObject("[PhysicsTileContainer]").transform;
            physicsTileContainer.parent = transform;

            grid = GetComponent<Grid>();
            mapGenerator = new TileMapGenerator();
            transform.position = new Vector3(environmentSettings.WorldOffset.x, environmentSettings.WorldOffset.y, 0);

            SetBounds();
        }

        private void Start()
        {
            mapGenerator.GenerateMap(environment, environmentSettings);
            mapGenerator.GenerateMap(background, backgroundSettings);
        }

        private void OnEnable()
        {
            Events.Subscribe(TileWorldEvents.RequestTilePickUp, OnRequestTilePickUp);
            Events.Subscribe(TileWorldEvents.DespawnPhysicsTile, OnDespawnPhysicsTile);
        }

        private void OnDisable()
        {
            Events.Unsubscribe(TileWorldEvents.RequestTilePickUp, OnRequestTilePickUp);
            Events.Unsubscribe(TileWorldEvents.DespawnPhysicsTile, OnDespawnPhysicsTile);
        }

        #endregion

        #region TileWorld Event Handlers

        private void OnRequestTilePickUp(TileWorldEventData data)
        {
            // Make sure we pick up the correct tile base off what direction the request is coming from.
            float radians = Mathf.Atan2(data.TileDirection.y, data.TileDirection.x);
            float x = data.TilePosition.x + Mathf.Cos(radians) * (environment.cellSize.x * 0.5f);
            float y = data.TilePosition.y + Mathf.Sin(radians) * (environment.cellSize.y * 0.5f);

            Vector3Int cell = grid.WorldToCell(new Vector2(x, y));
            TileBase tile = environment.GetTile(cell);

            if (tile != null)
            {
                PhysicsTile physicsTile = physicsTiles.Take();

                physicsTile.transform.Reset();
                physicsTile.transform.parent = physicsTileContainer;
                physicsTile.transform.position = environment.CellToWorld(cell) + (environment.cellSize * 0.5f);

                physicsTile.gameObject.SetActive(true);
                physicsTile.Spawn(environment.GetSprite(cell), environment.cellSize);

                environment.SetTile(cell, null);

                Events.Dispatch(TileWorldEvents.SetTilePickUp, new TileWorldEventData { TileObject = physicsTile });
            }
        }

        private void OnDespawnPhysicsTile(TileWorldEventData data)
        {
            PhysicsTile physicsTile = data.TileObject;

            physicsTile.transform.Reset();
            physicsTile.transform.position = OFF_SCREEN;

            physicsTile.Despawn();
            physicsTile.gameObject.SetActive(false);

            physicsTiles.Put(physicsTile);
        }

        #endregion

        #region Pooling Allocators / Deallocators

        private PhysicsTile AllocatePhysicsTile()
        {
            return Instantiate(physicsTilePrefab).GetComponent<PhysicsTile>();
        }

        private void DeallocatePhysicsTile(PhysicsTile tile)
        {
            Destroy(tile.gameObject);
        }

        #endregion

        #region Editor Methods

        private void OnDrawGizmos()
        {
            if (grid == null)
                grid = GetComponent<Grid>();

            if (environmentSettings != null)
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
            float width = environmentSettings.MapSize.x * grid.cellSize.x;
            float height = environmentSettings.MapSize.y * grid.cellSize.y;

            float left = transform.position.x - width * 0.5f;
            float top = transform.position.y - height * 0.5f + grid.cellSize.y;

            Bounds = new Rect(left, top, width, height);
        }
    }
}
