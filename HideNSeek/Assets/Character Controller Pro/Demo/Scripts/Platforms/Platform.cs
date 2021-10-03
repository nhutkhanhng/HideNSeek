using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{



    /// <summary>
    /// This abstract class represents a basic platform. It provides a RigidgodyComponent (2D or 3D) for the rigidbody
    /// </summary>
    public abstract class Platform : MonoBehaviour
    {

        /// <summary>
        /// Gets the RigidbodyComponent component associated to the character.
        /// </summary>
        public RigidbodyComponent RigidbodyComponent { get; protected set; }

        protected virtual void Awake()
        {
            Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();

            Rigidbody rigidbody3D = GetComponent<Rigidbody>();

            if (rigidbody3D != null)
            {
                RigidbodyComponent = gameObject.GetOrAddComponent<RigidbodyComponent3D>();
            }



            if (RigidbodyComponent == null)
                this.enabled = false;

        }

    }

}
