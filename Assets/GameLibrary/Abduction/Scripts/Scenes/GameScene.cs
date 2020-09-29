using Abduction.Background;
using Abduction.Events;
using Abduction.Interfaces;
using Abduction.Player;
using Abduction.Systems.Projectiles;
using Abduction.Systems.TileMaps;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Abduction.Scenes
{
    public class GameScene : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private Light2D globalLight;

        [SerializeField]
        private float globalLightFadeDepth;

        [SerializeField]
        private float globalLightOffDepth;

        [SerializeField]
        private TileWorld tileWorld;

        [SerializeField]
        private Camera gameCamera;

        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private BackgroundController backgroundController;

        #endregion

        #region Life Cycle

        private void OnEnable()
        {
            GlobalEvents.OnPlayerBeamTrigger += OnPlayerBeamTriggered;
            GlobalEvents.OnProjectBurstTrigger += OnProjectileBurstTrigged;
        }

        private void OnDisable()
        {
            GlobalEvents.OnPlayerBeamTrigger -= OnPlayerBeamTriggered;
            GlobalEvents.OnProjectBurstTrigger -= OnProjectileBurstTrigged;
        }

        #endregion

        #region Updates

        private void FixedUpdate()
        {
            Rect worldBounds = tileWorld.Bounds;

            ClampPlayerToBounds(worldBounds);
            MoveCameraToPosition(playerController.transform.position, worldBounds);
            FadeLights();

            backgroundController.Scroll(Time.fixedDeltaTime);
        }

        #endregion

        #region Global Event Handlers

        private void OnPlayerBeamTriggered(PlayerBeam playerBeam, int layer)
        {
            Vector3 origin = playerBeam.transform.position;
            Vector3 direction = playerBeam.Aim;

            RaycastHit2D hitInfo = Physics2D.CircleCast(origin, 0.3f, direction, 1.5f, 1 << layer);
            IGrabbable grabbable = null;

            if (hitInfo.collider == null)
                return;

            string layerName = LayerMask.LayerToName(layer);
            switch (layerName)
            {
                case "Environment":
                    grabbable = tileWorld.GetTileInDirection(hitInfo.point, direction);
                    break;

                case "PhysicsTiles":
                    PhysicsTile tile = hitInfo.rigidbody.GetComponent<PhysicsTile>();

                    if (tile.AllowPickUp)
                        grabbable = tile;
                    break;
            }

            if (grabbable != null)
                playerBeam.PickUp(grabbable);
        }

        private void OnProjectileBurstTrigged(Projectile projectile, int layers)
        {
            Vector3 origin = projectile.transform.position;

            float radius = projectile.BurstRadius;
            float burstStrength = 10; // TODO - Remove HC

            Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, radius, layers);

            foreach (Collider2D collider in colliders)
            {
                int layer = collider.gameObject.layer;

                string layerName = LayerMask.LayerToName(layer);
                switch (layerName)
                {
                    case "Environment":
                        PhysicsTile[] tiles = tileWorld.GetNearByTiles(origin, (int)radius);

                        for (int i = 0; i < tiles.Length; i++)
                            tiles[i].ApplyBurstImpact(origin, burstStrength);

                        break;
                }
            }
        }

        #endregion

        private void ClampPlayerToBounds(Rect bounds)
        {
            Vector3 playerPosition = playerController.transform.position;
            playerPosition.x = Mathf.Clamp(playerPosition.x, bounds.xMin, bounds.xMax);
            playerPosition.y = Mathf.Clamp(playerPosition.y, bounds.yMin, bounds.yMax);
            playerController.transform.position = playerPosition;
        }

        private void MoveCameraToPosition(Vector3 position, Rect bounds)
        {
            Vector3 cameraPosition = gameCamera.transform.position;

            cameraPosition = Vector3.MoveTowards(cameraPosition, position, 1f);
            cameraPosition.z = gameCamera.transform.position.z;

            float vertExtent = gameCamera.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            cameraPosition.x = Mathf.Clamp(cameraPosition.x, bounds.xMin + horzExtent, bounds.xMax - horzExtent);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, bounds.yMin + vertExtent, bounds.yMax - vertExtent);

            cameraPosition.x = Mathf.RoundToInt(cameraPosition.x * 100) * 0.01f;
            cameraPosition.y = Mathf.RoundToInt(cameraPosition.y * 100) * 0.01f;

            gameCamera.transform.position = cameraPosition;
        }

        private void FadeLights()
        {
            float depth = playerController.transform.position.y;
            float lightIntensity = (depth - globalLightFadeDepth) / globalLightOffDepth;

            lightIntensity = Mathf.Clamp(lightIntensity, 0, 1);

            globalLight.intensity = 1 - lightIntensity;
            playerController.PlayerLight.intensity = lightIntensity;

            playerController.AdjectDropShadow(lightIntensity);
        }
    }
}
