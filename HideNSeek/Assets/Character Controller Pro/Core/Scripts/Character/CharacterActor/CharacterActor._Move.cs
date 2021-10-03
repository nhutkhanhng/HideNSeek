using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    public partial class CharacterActor
    {
        private bool moveFlag = false;


        void HandlePosition(float dt)
        {
            Vector3 position = Position;

            if (alwaysNotGrounded)
                ForceNotGrounded();

            if (IsKinematic)
            {
                // Process Attached Rigidbody
                if (!IsGrounded)
                {
                    // Check if a "Move" call has been made, if so then use the desiredPosition (RigidbodyComponent)
                    if (moveFlag)
                        position = RigidbodyComponent.DesiredPosition;

                    // WIP
                    //ReferenceRigidbodyDisplacement( ref position , attachedRigidbody );

                    Move(position);
                }


            }
            else
            {

                if (IsStable)
                {

                    VerticalVelocity = Vector3.zero;


                    // Weight ----------------------------------------------------------------------------------------------
                    ApplyWeight(GroundContactPoint);


                    Vector3 displacement = CustomUtilities.ProjectVectorOnPlane(
                        Velocity * dt,
                        GroundStableNormal,
                        Up
                    );

                    CollideAndSlide(ref position, displacement, false);

                    ProbeGround(ref position, dt);

                    ProcessDynamicGround(ref position, dt);

                    if (!IsStable)
                    {
                        CurrentTerrain = null;
                        groundRigidbodyComponent = null;
                    }
                }
                else
                {


                    CollideAndSlideUnstable(ref position, Velocity * dt);

                }

                Move(position);
            }

            moveFlag = false;
        }
        void HandleRotation(float dt)
        {
            if (!constraintRotation)
                return;

            if (upDirectionReference != null)
            {
                Vector3 targetPosition = Position + Velocity * dt;
                float sign = upDirectionReferenceMode == VerticalAlignmentSettings.VerticalReferenceMode.Towards ? 1f : -1f;

                constraintUpDirection = sign * (upDirectionReference.position - targetPosition).normalized;

            }

            Up = constraintUpDirection;

        }

        /// <summary>
        /// Sets the rigidbody velocity based on a target position. The same can be achieved by setting the velocity value manually. Do not confuse this function with the Unity's Character Controller "Move" function.
        /// </summary>
        public void Move(Vector3 position)
        {
            RigidbodyComponent.Move(position);
            moveFlag = true;
        }

        /// <summary>
        /// Sets the rigidbody velocity based on a target position. The same can be achieved by setting the velocity value manually. Do not confuse this function with the Unity's Character Controller "Move" function.
        /// </summary>
        public void Rotate(Quaternion rotation)
        {
            RigidbodyComponent.Rotate(rotation);
        }

        public void CheckVelocityProjection(ref Vector3 displacement)
        {
            bool upwardsDisplacement = transform.InverseTransformVectorUnscaled(displacement).y > 0f;

            if (!upwardsDisplacement)
                return;

            Vector3 modifiedDisplacement = CustomUtilities.ProjectVectorOnPlane(
                displacement,
                Up,
                Up
            );

            // HitInfoFilter filter = new HitInfoFilter( 
            // 	PhysicsComponent.CollisionLayerMask ,
            // 	false ,
            // 	true
            // );

            // HitInfo projectionHitInfo;
            // PhysicsComponent.SphereCast( 
            // 	out projectionHitInfo , 
            // 	OffsettedBottomCenter , 
            // 	BodySize.x - CharacterConstants.SkinWidth ,
            // 	modifiedDisplacement.normalized * ( modifiedDisplacement.magnitude + CharacterConstants.SkinWidth ) ,
            // 	filter
            // );

            // if( projectionHitInfo.hit )
            // 	return;

            displacement = modifiedDisplacement;

            // if( displacement != Vector3.zero )


        }

    }
}
