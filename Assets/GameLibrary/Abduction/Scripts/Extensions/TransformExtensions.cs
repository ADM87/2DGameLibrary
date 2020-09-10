using UnityEngine;

namespace Abduction.Extensions
{
    public static class TransformExtensions
    {
        public static Transform Reset(this Transform transform)
        {
            transform.position = transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.rotation = Quaternion.identity;

            return transform;
        }
    }
}
