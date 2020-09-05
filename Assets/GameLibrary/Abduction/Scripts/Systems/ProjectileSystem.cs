using Abduction.Data;
using Abduction.Events;
using UnityEngine;

namespace Abduction.Systems
{
    public enum ProjectileEvents { Spawn }

    public class ProjectileSystem : MonoBehaviour
    {
        public static EventEmitter<ProjectileEvents, ProjectileEventData> Events = new EventEmitter<ProjectileEvents, ProjectileEventData>();

        #region Life Cycle

        private void OnEnable()
        {
            Events.Subscribe(ProjectileEvents.Spawn, OnSpawn);
        }

        private void OnDisable()
        {
            Events.Unsubscribe(ProjectileEvents.Spawn, OnSpawn);
        }

        #endregion

        private void OnSpawn(ProjectileEventData data)
        {

        }
    }
}
