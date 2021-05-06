using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    public partial class CharacterActor
    {

        RigidbodyComponent attachedRigidbody = null;

        public void SetReferenceRigidbody(Transform target)
        {
            if (target == null)
            {
                attachedRigidbody = null;
                return;
            }

            RigidbodyComponent rigidbodyComponent = groundRigidbodyComponents.GetOrRegisterValue<Transform, RigidbodyComponent>(target);
            attachedRigidbody = rigidbodyComponent;

        }


        protected Vector3 ReferenceRigidbodyDisplacement(Vector3 position, RigidbodyComponent referenceRigidbody)
        {

            if (referenceRigidbody == null)
                return Vector3.zero;

            Vector3 initialPosition = position;

            Quaternion deltaRotation = referenceRigidbody.DesiredRotation * Quaternion.Inverse(referenceRigidbody.Rotation);

            Vector3 centerToCharacter = position - referenceRigidbody.Position;
            Vector3 rotatedCenterToCharacter = deltaRotation * centerToCharacter;

            if (rotateForwardDirection)
            {
                Vector3 up = Up;
                Forward = deltaRotation * Forward;
                Up = up;
            }

            Vector3 finalPosition = referenceRigidbody.DesiredPosition + rotatedCenterToCharacter;

            return finalPosition - initialPosition;

        }

        // Vanilla rigidbodies -------------------------------------------------------------------------------------------------

        protected Vector3 ReferenceRigidbodyDisplacement(Vector3 position, Rigidbody referenceRigidbody, float dt)
        {
            if (referenceRigidbody == null)
                return Vector3.zero;

            return ReferenceRigidbodyDisplacement(position, referenceRigidbody.GetPointVelocity(position), referenceRigidbody.position, dt);
        }

        protected Vector3 ReferenceRigidbodyDisplacement(Vector3 position, Rigidbody2D referenceRigidbody, float dt)
        {
            if (referenceRigidbody == null)
                return Vector3.zero;

            return ReferenceRigidbodyDisplacement(position, (Vector3)referenceRigidbody.GetPointVelocity(position), (Vector3)referenceRigidbody.position, dt);
        }

        protected Vector3 ReferenceRigidbodyDisplacement(Vector3 characterPosition, Vector3 groundPointVelocity, Vector3 groundPosition, float dt)
        {

            Vector3 initialPosition = characterPosition;
            Vector3 finalPosition = characterPosition + groundPointVelocity * dt;

            Vector3 centerToCharacter = characterPosition - groundPosition;
            Vector3 rotatedCenterToCharacter = finalPosition - groundPosition;

            Quaternion deltaRotation = Quaternion.FromToRotation(centerToCharacter, rotatedCenterToCharacter);

            if (rotateForwardDirection)
            {
                Vector3 up = Up;
                Forward = deltaRotation * Forward;
                Up = up;
            }


            return finalPosition - initialPosition;
        }

        // -------------------------------------------------------------------------------------------------------

    }
}