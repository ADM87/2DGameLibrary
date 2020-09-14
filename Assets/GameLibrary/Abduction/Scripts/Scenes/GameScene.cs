using Abduction.Player;
using Abduction.Systems.TileMaps;
using UnityEngine;

namespace Abduction.Scenes
{
    public class GameScene : MonoBehaviour
    {
        #region Serialized Fields

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

            Vector3 playerPosition = playerController.transform.position;
            playerPosition.x = Mathf.Clamp(playerPosition.x, worldBounds.xMin, worldBounds.xMax);
            playerPosition.y = Mathf.Clamp(playerPosition.y, worldBounds.yMin, worldBounds.yMax);

            Vector3 playerVelocity = new Vector3(playerController.Velocity.x, playerController.Velocity.y, 0);
            Vector3 cameraPosition = gameCamera.transform.position;

            cameraPosition = Vector3.SmoothDamp(cameraPosition, playerPosition, ref playerVelocity, 0.1f);
            cameraPosition.z = gameCamera.transform.position.z;

            float vertExtent = gameCamera.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            cameraPosition.x = Mathf.Clamp(cameraPosition.x, worldBounds.xMin + horzExtent, worldBounds.xMax - horzExtent);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, worldBounds.yMin + vertExtent, worldBounds.yMax - vertExtent);

            playerController.transform.position = playerPosition;
            gameCamera.transform.position = cameraPosition;
        }

        #endregion
    }
}
