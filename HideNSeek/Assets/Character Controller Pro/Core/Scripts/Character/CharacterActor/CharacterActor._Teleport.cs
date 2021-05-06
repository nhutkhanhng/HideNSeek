using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    public partial class CharacterActor
    {
        protected virtual void OnEnable()
        {
            OnTeleport += OnTeleportMethod;
        }

        protected virtual void OnDisable()
        {
            OnTeleport -= OnTeleportMethod;
        }


        void OnTeleportMethod(Vector3 position, Quaternion rotaiton)
        {
            Velocity = Vector3.zero;
            groundVelocity = Vector3.zero;
            stableProbeGroundVelocity = Vector3.zero;
        }


        /// <summary>
        /// Sets the teleportation position and rotation using an external Transform reference. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Transform reference)
        {
            Teleport(reference.position, reference.rotation);
        }

        /// <summary>
        /// Sets the teleportation position and rotation. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;

            transform.SetPositionAndRotation(position, rotation);

            if (OnTeleport != null)
                OnTeleport(Position, Rotation);
        }

        /// <summary>
        /// Sets the teleportation position. 
        /// The character will move/rotate internally using its own internal logic.
        /// </summary>
        public void Teleport(Vector3 position)
        {
            Position = position;

            transform.position = position;

            if (OnTeleport != null)
                OnTeleport(Position, Rotation);

        }
    }
}