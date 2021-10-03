using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.CharacterControllerPro.Demo
{

    public partial class NormalMovement
    {
        public LookingDirectionParameters lookingDirectionParameters = new LookingDirectionParameters();

        protected Vector3 targetLookingDirection = default(Vector3);


        void HandleLookingDirection(float dt)
        {
            if (lookingDirectionParameters.followExternalReference)
            {
                targetLookingDirection = CharacterStateController.MovementReferenceForward;
            }
            else
            {
                switch (CharacterActor.CurrentState)
                {
                    case CharacterActorState.NotGrounded:

                        if (CharacterActor.PlanarVelocity != Vector3.zero)
                            targetLookingDirection = CharacterActor.PlanarVelocity;

                        break;
                    case CharacterActorState.StableGrounded:

                        if (CharacterStateController.InputMovementReference != Vector3.zero)
                            targetLookingDirection = CharacterStateController.InputMovementReference;
                        else
                            targetLookingDirection = CharacterActor.Forward;


                        break;
                    case CharacterActorState.UnstableGrounded:

                        if (CharacterActor.PlanarVelocity != Vector3.zero)
                            targetLookingDirection = CharacterActor.PlanarVelocity;

                        break;
                }

            }


            Quaternion targetDeltaRotation = Quaternion.FromToRotation(CharacterActor.Forward, targetLookingDirection);
            Quaternion currentDeltaRotation = Quaternion.Slerp(Quaternion.identity, targetDeltaRotation, 10 * dt);



            {
                float angle = Vector3.Angle(CharacterActor.Forward, targetLookingDirection);

                if (CustomUtilities.isCloseTo(angle, 180f, 0.5f))
                {

                    CharacterActor.Forward = Quaternion.Euler(0f, 1f, 0f) * CharacterActor.Forward;
                }

                CharacterActor.Forward = currentDeltaRotation * CharacterActor.Forward;

            }



        }


        protected virtual void HandleRotation(float dt)
        {
            HandleLookingDirection(dt);
        }

    }

}