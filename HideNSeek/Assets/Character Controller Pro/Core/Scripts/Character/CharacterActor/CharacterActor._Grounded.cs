using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    public partial class CharacterActor : MonoBehaviour
    {

        Vector3 dynamicGroundDisplacement;
        int forceNotGroundedFrames = 0;

        Vector3 groundVelocity = default(Vector3);
        public Vector3 GroundVelocity
        {
            get
            {
                return IsStable ? groundVelocity : Vector3.zero;
            }
        }

        Vector3 stableProbeGroundVelocity = default(Vector3);
        Vector3 StableProbeGroundVelocity
        {
            get
            {
                return IsStable ? stableProbeGroundVelocity : Vector3.zero;
            }
        }

        /// <summary>
        /// Gets the object below the character (only valid if the character is falling). The maximum prediction distance is defined by the constant "GroundPredictionDistance".
        /// </summary>
        public GameObject PredictedGround { get; private set; }

        /// <summary>
        /// Gets the distance to the "PredictedGround".
        /// </summary>
        public float PredictedGroundDistance { get; private set; }


        void ProcessDynamicGround(ref Vector3 position, float dt)
        {
            if (IsStable)
            {
                // Dynamic ground ---------------------------------------------------------------------------------					
                if (supportDynamicGround)
                {
                    groundVelocity = Vector3.zero;

                    // If the ground has a RigidbodyComponent component use this as the ground velocity source (high priority)
                    if (supportRigidbodyComponent)
                    {
                        if (groundRigidbodyComponent != null)
                        {
                            Vector3 groundDisplacement = ReferenceRigidbodyDisplacement(position, groundRigidbodyComponent);
                            groundVelocity = groundDisplacement / dt;

                        }
                    }

                    // ... If not, look for a vanilla Rigidbody/Rigidbody2D component (low priority)
                    if (supportVanillaRigidbodies)
                    {
                        if (groundRigidbodyComponent == null)
                        {
                            if (GroundCollider3D.attachedRigidbody != null)
                            {
                                Vector3 groundDisplacement = ReferenceRigidbodyDisplacement(position, GroundCollider3D.attachedRigidbody, dt);
                                groundVelocity = groundDisplacement / dt;

                            }
                        }

                    }

                    position += groundVelocity * dt;

                }

            }
            else
            {
                groundVelocity = Vector3.zero;
            }
        }
        /// <summary>
        /// Forces the character to be grounded (isGrounded = true) if possible. The detection distance includes the step down distance.
        /// </summary>
        public void ForceGrounded()
        {
            HitInfoFilter filter = new HitInfoFilter(PhysicsComponent.CollisionLayerMask, false, true);

            CollisionInfo collisionInfo;
            bool hit = characterCollisions.CheckForGround(
                out collisionInfo,
                Position,
                BodySize.y * 0.8f, // 80% of the height
                stepDownDistance,
                filter
            );



            if (hit)
            {
                ProcessNewGround(collisionInfo.hitInfo.transform, collisionInfo);

                float slopeAngle = Vector3.Angle(Up, GetGroundSlopeNormal(collisionInfo));

                if (slopeAngle <= slopeLimit)
                {
                    // Save the ground collision info
                    characterCollisionInfo.SetGroundInfo(collisionInfo, this);
                    Position += collisionInfo.displacement;

                }

            }
        }

        /// <summary>
        /// Forces the character to abandon the grounded state (isGrounded = false). 
        /// 
        /// TIP: This is useful when making the character jump.
        /// </summary>
        /// <param name="ignoreGroundContactFrames">The number of FixedUpdate frames to consume in order to prevent the character to 
        /// re-enter grounded state right after a ForceNotGrounded call. For example, if this number is three (default) then the character will be able to enter grounded state 
        /// after four frames since the original call.</param>
        public void ForceNotGrounded(int ignoreGroundContactFrames = 3)
        {
            forceNotGroundedFrames = ignoreGroundContactFrames;

            bool wasGrounded = IsGrounded;

            characterCollisionInfo.ResetGroundInfo();

            groundVelocity = Vector3.zero;

        }


        void ProcessNewGround(Transform newGroundTransform, CollisionInfo collisionInfo)
        {
            bool isThisANewGround = collisionInfo.hitInfo.transform != GroundTransform;
            if (isThisANewGround)
            {
                CurrentTerrain = terrains.GetOrRegisterValue<Transform, Terrain>(newGroundTransform);
                groundRigidbodyComponent = groundRigidbodyComponents.GetOrRegisterValue<Transform, RigidbodyComponent>(newGroundTransform);
            }
        }
        public Vector3 GetGroundSlopeNormal(CollisionInfo collisionInfo)
        {
            if (IsOnTerrain)
                return collisionInfo.hitInfo.normal;

            float contactSlopeAngle = Vector3.Angle(Up, collisionInfo.hitInfo.normal);
            if (collisionInfo.isAnEdge)
            {
                if (contactSlopeAngle < slopeLimit && collisionInfo.edgeUpperSlopeAngle <= slopeLimit && collisionInfo.edgeLowerSlopeAngle <= slopeLimit)
                {
                    return Up;
                }
                else if (collisionInfo.edgeUpperSlopeAngle <= slopeLimit)
                {
                    return collisionInfo.edgeUpperNormal;
                }
                else if (collisionInfo.edgeLowerSlopeAngle <= slopeLimit)
                {
                    return collisionInfo.edgeLowerNormal;
                }
                else
                {
                    return collisionInfo.hitInfo.normal;
                }
            }
            else
            {
                return collisionInfo.hitInfo.normal;
            }



        }

        void ProbeGround(ref Vector3 position, float dt)
        {
            Vector3 preProbeGroundPosition = position;

            float groundCheckDistance = edgeCompensation ?
            BodySize.x / 2f + CharacterConstants.GroundCheckDistance :
            CharacterConstants.GroundCheckDistance;

            Vector3 displacement = -Up * Mathf.Max(groundCheckDistance, stepDownDistance);

            HitInfoFilter filter = new HitInfoFilter(PhysicsComponent.CollisionLayerMask, false, true);


            CollisionInfo collisionInfo;
            bool hit = characterCollisions.CheckForGround(
                out collisionInfo,
                position,
                StepOffset,
                stepDownDistance,
                filter
            );

            if (hit)
            {

                float slopeAngle = Vector3.Angle(Up, GetGroundSlopeNormal(collisionInfo));

                if (slopeAngle <= slopeLimit)
                {
                    // Stable hit ---------------------------------------------------				
                    ProcessNewGround(collisionInfo.hitInfo.transform, collisionInfo);

                    // Save the ground collision info
                    characterCollisionInfo.SetGroundInfo(collisionInfo, this);



                    // Calculate the final position 
                    position += collisionInfo.displacement;


                    if (edgeCompensation && IsAStableEdge(collisionInfo))
                    {
                        // calculate the edge compensation and apply that to the final position
                        Vector3 compensation = Vector3.Project((collisionInfo.hitInfo.point - position), Up);
                        position += compensation;
                    }

                    stableProbeGroundVelocity = (position - preProbeGroundPosition) / dt;
                }
                else
                {
                    // Unstable Hit

                    // If the unstable ground is far enough then force not grounded
                    float castSkinDistance = StepOffset + 2f * CharacterConstants.SkinWidth;
                    if (collisionInfo.hitInfo.distance > castSkinDistance)
                    {

                        ForceNotGrounded();
                        return;
                    }


                    if (preventBadSteps)
                    {
                        // If the unstable ground is close enough then do a new collide and slide
                        if (WasGrounded)
                        {
                            position = Position;

                            Vector3 unstableDisplacement = CustomUtilities.ProjectVectorOnPlane(
                                Velocity * dt,
                                GroundStableNormal,
                                Up
                            );

                            CollideAndSlide(ref position, unstableDisplacement, true);


                        }
                    }


                    characterCollisions.CheckForGroundRay(
                        out collisionInfo,
                        position,
                        StepOffset,
                        stepDownDistance,
                        filter
                    );

                    ProcessNewGround(collisionInfo.hitInfo.transform, collisionInfo);

                    characterCollisionInfo.SetGroundInfo(collisionInfo, this);
                    Debug.DrawRay(collisionInfo.hitInfo.point, collisionInfo.hitInfo.normal);
                    stableProbeGroundVelocity = (position - preProbeGroundPosition) / dt;

                }



            }
            else
            {
                ForceNotGrounded();
            }

        }


    }
}