using UnityEngine;

namespace Abduction.Background
{
    public class BackgroundController : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private SpriteRenderer backgroundRenderer;

        [SerializeField]
        private Camera gameCamera;

        [SerializeField]
        private float scrollSpeed;

        #endregion

        #region Member Variables

        private float previewCameraX;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            Sprite backgroundSprite = backgroundRenderer.sprite;

            float spriteWidth = backgroundSprite.rect.width / backgroundSprite.pixelsPerUnit;

            float viewHeight = gameCamera.orthographicSize * 2f;
            float viewWidth = viewHeight / Screen.height * Screen.width;

            float scale = (viewWidth / spriteWidth);

            backgroundRenderer.transform.localScale = new Vector3(scale, 1, 1);
            backgroundRenderer.material.mainTextureScale = backgroundRenderer.transform.localScale;

            previewCameraX = gameCamera.transform.position.x;
        }

        #endregion

        public void Scroll(float dt)
        {
            Vector3 position = transform.position;
            position.x = gameCamera.transform.position.x;
            transform.position = position;

            float delta = previewCameraX - position.x;
            previewCameraX = position.x;

            float scroll = (delta * scrollSpeed) * dt;

            Vector2 offset = backgroundRenderer.material.mainTextureOffset;
            offset.x = (1 + ((offset.x - scroll) % 1)) % 1;
            backgroundRenderer.material.mainTextureOffset = offset;
        }
    }
}
