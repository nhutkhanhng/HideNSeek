using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    public partial class CharacterActor
    {
        public void CollideAndSlide(ref Vector3 position, Vector3 displacement, bool useFullBody)
        {

            Vector3 groundPlaneNormal = GroundStableNormal;
            Vector3 slidingPlaneNormal = Vector3.zero;

            HitInfoFilter filter = new HitInfoFilter(
                PhysicsComponent.CollisionLayerMask,
                false,
                true
            );

            int iteration = 0;


            while (iteration < CharacterConstants.MaxSlideIterations)
            {
                iteration++;

                CollisionInfo collisionInfo;
                bool hit = characterCollisions.CastBody(
                    out collisionInfo,
                    position,
                    displacement,
                    useFullBody ? 0f : StepOffset,
                    filter
                );


                if (hit)
                {
                    //---
                    if (canPushDynamicRigidbodies)
                    {
                        if (collisionInfo.hitInfo.IsRigidbody)
                        {
                            if (!collisionInfo.hitInfo.IsKinematicRigidbody)
                            {
                                bool canPushThisObject = CustomUtilities.BelongsToLayerMask(collisionInfo.hitInfo.transform.gameObject.layer, pushableRigidbodyLayerMask);
                                if (canPushThisObject)
                                {
                                    // Use the entire displacement and stop the collide and slide
                                    position += displacement;
                                    break;
                                }
                            }

                        }
                    }


                    if (slideOnWalls)
                    {
                        position += collisionInfo.displacement;
                        displacement -= collisionInfo.displacement;

                        // Get the new slide plane
                        bool blocked = UpdateSlidingPlanes(
                            collisionInfo,
                            ref slidingPlaneNormal,
                            ref groundPlaneNormal,
                            ref displacement
                        );
                    }
                    else
                    {
                        if (!WallCollision)
                            position += collisionInfo.displacement;

                        break;
                    }



                }
                else
                {
                    position += displacement;
                    break;
                }

            }

        }

        public void CollideAndSlideUnstable(ref Vector3 position, Vector3 displacement)
        {

            HitInfoFilter filter = new HitInfoFilter(
                PhysicsComponent.CollisionLayerMask,
                false,
                true
            );

            int iteration = 0;

            while (iteration < CharacterConstants.MaxSlideIterations || displacement == Vector3.zero)
            {
                iteration++;

                CollisionInfo collisionInfo;
                bool hit = characterCollisions.CastBody(
                    out collisionInfo,
                    position,
                    displacement,
                    0f,
                    filter
                );


                if (hit)
                {

                    position += collisionInfo.displacement;

                    if (canPushDynamicRigidbodies)
                    {
                        if (collisionInfo.hitInfo.IsRigidbody)
                        {
                            if (!collisionInfo.hitInfo.IsKinematicRigidbody)
                            {
                                Vector3 remainingVelocity = displacement - collisionInfo.displacement;
                                Vector3 force = CharacterBody.Mass * (remainingVelocity / Time.deltaTime);

                                collisionInfo.hitInfo.rigidbody3D.AddForceAtPosition(force, position);
                            }

                        }
                    }

                    displacement -= collisionInfo.displacement;

                    displacement = Vector3.ProjectOnPlane(displacement, collisionInfo.hitInfo.normal);
                }
                else
                {
                    position += displacement;
                    break;

                }

            }


            if (!alwaysNotGrounded && forceNotGroundedFrames == 0)
            {

                CollisionInfo groundCheckCollisionInfo;
                characterCollisions.CheckForGround(
                    out groundCheckCollisionInfo,
                    position,
                    stepUpDistance,
                    CharacterConstants.GroundPredictionDistance,
                    filter
                );

                if (groundCheckCollisionInfo.collision)
                {
                    PredictedGround = groundCheckCollisionInfo.hitInfo.transform.gameObject;
                    PredictedGroundDistance = groundCheckCollisionInfo.displacement.magnitude;

                    bool validGround = PredictedGroundDistance <= CharacterConstants.GroundCheckDistance;

                    if (validGround)
                    {
                        ProcessNewGround(groundCheckCollisionInfo.hitInfo.transform, groundCheckCollisionInfo);
                        characterCollisionInfo.SetGroundInfo(groundCheckCollisionInfo, this);

                    }
                    else
                    {
                        characterCollisionInfo.ResetGroundInfo();
                    }

                }
                else
                {
                    PredictedGround = null;
                    PredictedGroundDistance = 0f;

                    characterCollisionInfo.ResetGroundInfo();
                }




            }


        }

        bool UpdateSlidingPlanes(CollisionInfo collisionInfo, ref Vector3 slidingPlaneNormal, ref Vector3 groundPlaneNormal, ref Vector3 displacement)
        {
            Vector3 normal = collisionInfo.hitInfo.normal;

            if (collisionInfo.contactSlopeAngle > slopeLimit)
            {

                if (slidingPlaneNormal != Vector3.zero)
                {
                    float correlation = Vector3.Dot(normal, slidingPlaneNormal);

                    if (correlation > 0)
                        displacement = CustomUtilities.DeflectVector(displacement, groundPlaneNormal, normal);
                    else
                        displacement = Vector3.zero;

                }
                else
                {
                    displacement = CustomUtilities.DeflectVector(displacement, groundPlaneNormal, normal);
                }

                slidingPlaneNormal = normal;
            }
            else
            {
                displacement = CustomUtilities.ProjectVectorOnPlane(
                    displacement,
                    normal,
                    Up
                );

                groundPlaneNormal = normal;
                slidingPlaneNormal = Vector3.zero;

            }

            return displacement == Vector3.zero;
        }
    }
}