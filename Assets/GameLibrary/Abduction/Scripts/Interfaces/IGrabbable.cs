using UnityEngine;

namespace Abduction.Interfaces
{
    public interface IGrabbable
    {
        Transform transform { get; }

        Rigidbody2D GrabbableBody { get; }
        bool AllowPickUp { get; }

        void OnPickedUp();
        void OnDropped();
    }
}
