using Abduction.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Abduction.UI
{
    public class PillarBox : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private List<RectTransform> pillars;

        #endregion

        #region Member Variables

        private CanvasScaler canvasScaler;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            canvasScaler = GetComponentInParent<CanvasScaler>();
        }

        private void OnEnable()
        {
            ScreenUtils.Events.Subscribe(ScreenEvents.Resize, OnResize);
        }

        private void OnDisable()
        {
            ScreenUtils.Events.Unsubscribe(ScreenEvents.Resize, OnResize);
        }

        #endregion

        #region Resize

        private void OnResize(ScreenEventData data)
        {
            // This assumes that the game is scaling by height.

            float width = data.Width * (canvasScaler.referenceResolution.y / data.Height);
            float pillarWidth = Mathf.Max((width - canvasScaler.referenceResolution.x) * 0.5f, 0);

            foreach (RectTransform rect in pillars)
                rect.sizeDelta = new Vector2(pillarWidth, rect.sizeDelta.y);
        }

        #endregion
    }
}
