using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{
    public partial class CharacterActor : MonoBehaviour
    {

        void OnDestroy()
        {

        }

        void OnValidate()
        {
            if (CharacterBody == null)
                CharacterBody = GetComponent<CharacterBody>();

            stepUpDistance = Mathf.Clamp(
                stepUpDistance,
                CharacterConstants.ColliderMinBottomOffset + CharacterBody.BodySize.x / 2f,
                0.75f * CharacterBody.BodySize.y
            );

        }

        void FixedUpdate()
        {

            float dt = Time.deltaTime;

            WasGrounded = IsGrounded;
            WasStable = IsStable;

            GetContactsInformation();

            InputVelocity = Velocity;

            HandleSize(dt);
            HandlePosition(dt);
            HandleRotation(dt);

            PreSimulationVelocity = Velocity;

            // ------------------------------------------------------------

            if (IsStable)
            {
                StableGroundedTime += dt;
                UnstableGroundedTime = 0f;
            }
            else
            {
                StableGroundedTime = 0f;
                UnstableGroundedTime += dt;
            }

            if (IsGrounded)
            {
                NotGroundedTime = 0f;
                GroundedTime += dt;

                LastGroundedVelocity = Velocity;

                if (!WasGrounded)
                    if (OnGroundedStateEnter != null)
                        OnGroundedStateEnter(LocalVelocity);

            }
            else
            {
                NotGroundedTime += dt;
                GroundedTime = 0f;

                if (WasGrounded)
                    if (OnGroundedStateExit != null)
                        OnGroundedStateExit();

            }

            if (forceNotGroundedFrames != 0)
                forceNotGroundedFrames--;

            PhysicsComponent.ClearContacts();
        }
        protected virtual void Awake()
        {
            CharacterBody = GetComponent<CharacterBody>();
            targetBodySize = CharacterBody.BodySize;
            BodySize = targetBodySize;

            PhysicsComponent = gameObject.AddComponent<PhysicsComponent3D>();

            SetColliderSize();

            RigidbodyComponent.UseGravity = false;
            RigidbodyComponent.Mass = CharacterBody.Mass;
            RigidbodyComponent.LinearDrag = 0f;
            RigidbodyComponent.AngularDrag = 0f;
            RigidbodyComponent.ContinuousCollisionDetection = false; // false, the character predicts everything, there is no need for continuous detection
            RigidbodyComponent.UseInterpolation = true;
            RigidbodyComponent.Constraints = RigidbodyConstraints.FreezeRotation;

            characterCollisions.Initialize(this, PhysicsComponent);

            StartCoroutine(LateFixedUpdate());


        }
        protected virtual void Start()
        {
            if (forceGroundedAtStart && !alwaysNotGrounded)
                ForceGrounded();
        }



        IEnumerator LateFixedUpdate()
        {
            YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                yield return waitForFixedUpdate;


                Velocity -= GroundVelocity;
                Velocity -= StableProbeGroundVelocity;

                PostSimulationVelocity = Velocity;
                ExternalVelocity = PostSimulationVelocity - PreSimulationVelocity;

                // Velocity assignment ------------------------------------------------------

                if (IsStable)
                {

                    switch (stablePostSimulationVelocity)
                    {
                        case CharacterVelocityMode.UseInputVelocity:

                            Velocity = InputVelocity;

                            break;
                        case CharacterVelocityMode.UsePreSimulationVelocity:

                            Velocity = PreSimulationVelocity;

                            // Take the rigidbody velocity and convert that into planar velocity
                            if (WasStable)
                                PlanarVelocity = Velocity.magnitude * PlanarVelocity.normalized;

                            break;
                        case CharacterVelocityMode.UsePostSimulationVelocity:

                            // Take the rigidbody velocity and convert that into planar velocity
                            if (WasStable)
                                PlanarVelocity = Velocity.magnitude * PlanarVelocity.normalized;

                            break;
                    }


                }
                else
                {
                    switch (unstablePostSimulationVelocity)
                    {
                        case CharacterVelocityMode.UseInputVelocity:

                            Velocity = InputVelocity;

                            break;
                        case CharacterVelocityMode.UsePreSimulationVelocity:

                            Velocity = PreSimulationVelocity;

                            break;
                        case CharacterVelocityMode.UsePostSimulationVelocity:

                            break;
                    }
                }



            }
        }


    }
}