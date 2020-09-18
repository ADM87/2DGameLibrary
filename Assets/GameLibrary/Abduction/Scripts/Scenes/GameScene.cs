using Abduction.Player;
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

        #endregion

        #region Updates

        private void FixedUpdate()
        {
            Rect worldBounds = tileWorld.Bounds;

            ClampPlayerToBounds(worldBounds);
            MoveCameraToPosition(playerController.transform.position, worldBounds);
            FadeLights();
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
