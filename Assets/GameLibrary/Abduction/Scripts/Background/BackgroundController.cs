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
        private Vector2 screenSize;

        #endregion

        #region Life Cycle

        private void Start()
        {
            screenSize = new Vector2(Screen.width, Screen.height);
            AdjustBackgroundSize();
        }

        private void Update()
        {
            if (screenSize.x != Screen.width || screenSize.y != Screen.height)
            {
                screenSize.Set(Screen.width, Screen.height);
                AdjustBackgroundSize();
            }
        }

        #endregion

        private void AdjustBackgroundSize()
        {
            Sprite backgroundSprite = backgroundRenderer.sprite;

            float spriteWidth = backgroundSprite.rect.width / backgroundSprite.pixelsPerUnit;

            float viewHeight = gameCamera.orthographicSize * 2f;
            float viewWidth = viewHeight / Screen.height * Screen.width;

            float scale = (viewWidth / spriteWidth);

            backgroundRenderer.transform.localScale = new Vector3(scale, 1, 1);
            backgroundRenderer.material.SetVector("_Tiling", backgroundRenderer.transform.localScale);

            previewCameraX = gameCamera.transform.position.x;
        }

        public void Scroll(float dt)
        {
            Vector3 position = transform.position;
            position.x = gameCamera.transform.position.x;
            transform.position = position;

            float delta = previewCameraX - position.x;
            previewCameraX = position.x;

            float scroll = (delta * scrollSpeed) * dt;

            Vector2 offset = backgroundRenderer.material.GetVector("_Offset");
            offset.x = (1 + ((offset.x - scroll) % 1)) % 1;
            backgroundRenderer.material.SetVector("_Offset", offset);
        }
    }
}
