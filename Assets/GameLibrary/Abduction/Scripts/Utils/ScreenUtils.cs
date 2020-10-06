using Abduction.Events;
using UnityEngine;

namespace Abduction.Utils
{
    public enum ScreenEvents { Resize }

    public struct ScreenEventData
    {
        public float Width { get; set; }
        public float Height { get; set; }
    }

    public class ScreenUtils : MonoBehaviour
    {
        #region Singleton Instance

        private static ScreenUtils instance;
        public static ScreenUtils Instance
        {
            get { return instance ?? (instance = new GameObject("ScreenUtils").AddComponent<ScreenUtils>()); }
        }

        #endregion

        public static EventEmitter<ScreenEvents, ScreenEventData> Events = new EventEmitter<ScreenEvents, ScreenEventData>();

        #region Member Variables

        private Vector2 screenSize;

        #endregion

        #region Life Cycle

        private void Awake()
        {
            screenSize = Vector2.zero;
        }

        private void Start()
        {
            DontDestroyOnLoad(instance.gameObject);
        }

        #endregion

        #region Updates

        private void Update()
        {
            if (screenSize.x != Screen.width || screenSize.y != Screen.height)
            {
                screenSize.Set(Screen.width, Screen.height);
                Events.Dispatch(ScreenEvents.Resize, new ScreenEventData { Width = Screen.width, Height = Screen.height });
            }
        }

        #endregion
    }
}
