using Abduction.Utils;
using UnityEngine;

namespace Abduction
{
    public class Main
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            // TODO - revisit
            _ = ScreenUtils.Instance;
        }
    }
}
