using Lightbug.CharacterControllerPro.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Lightbug.CharacterControllerPro.Demo
{
    /// <summary>
    ///  Execute some function of jump action
    /// </summary>
    public partial class NormalMovement
    {
        public enum JumpResult
        {
            Invalid,
            Grounded,
            NotGrounded
        }

        protected bool UnstableGroundedJumpAvailable => 
                            !verticalMovementParameters.canJumpOnUnstableGround 
                            && CharacterActor.CurrentState == CharacterActorState.UnstableGrounded;


        #region Events	
        /// <summary>
        /// Event triggered when the character jumps.
        /// </summary>
        public event System.Action OnJumpPerformed;

        /// <summary>
        /// Event triggered when the character jumps from the ground.
        /// </summary>
        public event System.Action<bool> OnGroundedJumpPerformed;

        /// <summary>
        /// Event triggered when the character jumps while.
        /// </summary>
        public event System.Action<int> OnNotGroundedJumpPerformed;

        #endregion



        protected Vector3 jumpDirection = default(Vector3);




        JumpResult CanJump()
        {
            JumpResult jumpResult = JumpResult.Invalid;

            if (!verticalMovementParameters.canJump)
                return jumpResult;

            if (wantToCrouch)
                return jumpResult;


            switch (CharacterActor.CurrentState)
            {
                case CharacterActorState.StableGrounded:

                    if (CharacterActions.jump.StartedElapsedTime <= verticalMovementParameters.preGroundedJumpTime && groundedJumpAvailable)
                    {
                        jumpResult = JumpResult.Grounded;
                    }

                    break;
                case CharacterActorState.NotGrounded:

                    if (CharacterActions.jump.Started)
                    {

                        // First check if the "grounded jump" is available. If so, execute a "coyote jump".
                        if (CharacterActor.NotGroundedTime <= verticalMovementParameters.postGroundedJumpTime && groundedJumpAvailable)
                        {
                            jumpResult = JumpResult.Grounded;
                        }
                        else if (notGroundedJumpsLeft != 0)  // Do a not grounded jump
                        {
                            jumpResult = JumpResult.NotGrounded;
                        }
                    }

                    break;
                case CharacterActorState.UnstableGrounded:

                    if (CharacterActions.jump.StartedElapsedTime <= verticalMovementParameters.preGroundedJumpTime && verticalMovementParameters.canJumpOnUnstableGround)
                        jumpResult = JumpResult.Grounded;

                    break;
            }

            return jumpResult;
        }


        protected virtual void ProcessJump(float dt)
        {

            if (CharacterActor.IsGrounded)
            {
                notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;

                groundedJumpAvailable = true;
            }


            if (isAllowedToCancelJump)
            {
                if (verticalMovementParameters.cancelJumpOnRelease)
                {
                    if (CharacterActions.jump.StartedElapsedTime >= verticalMovementParameters.cancelJumpMaxTime || CharacterActor.IsFalling)
                    {
                        isAllowedToCancelJump = false;
                    }
                    else if (!CharacterActions.jump.value && CharacterActions.jump.StartedElapsedTime >= verticalMovementParameters.cancelJumpMinTime)
                    {
                        // Get the velocity mapped onto the current jump direction
                        Vector3 projectedJumpVelocity = Vector3.Project(CharacterActor.Velocity, jumpDirection);

                        CharacterActor.Velocity -= projectedJumpVelocity * (1f - verticalMovementParameters.cancelJumpMultiplier);

                        isAllowedToCancelJump = false;
                    }
                }
            }
            else
            {
                JumpResult jumpResult = CanJump();

                switch (jumpResult)
                {
                    case JumpResult.Grounded:
                        groundedJumpAvailable = false;

                        break;
                    case JumpResult.NotGrounded:
                        notGroundedJumpsLeft--;

                        break;
                    case JumpResult.Invalid:
                        return;
                }

                // Events ---------------------------------------------------
                if (CharacterActor.IsGrounded)
                {
                    if (OnGroundedJumpPerformed != null)
                        OnGroundedJumpPerformed(true);
                }
                else
                {
                    if (OnNotGroundedJumpPerformed != null)
                        OnNotGroundedJumpPerformed(notGroundedJumpsLeft);
                }

                if (OnJumpPerformed != null)
                    OnJumpPerformed();

                // Define the jump direction ---------------------------------------------------
                jumpDirection = SetJumpDirection();

                // Force the not grounded state, without this the character will not leave the ground.     
                CharacterActor.ForceNotGrounded();

                // First remove any velocity associated with the jump direction.
                CharacterActor.Velocity -= Vector3.Project(CharacterActor.Velocity, jumpDirection);
                CharacterActor.Velocity += jumpDirection * verticalMovementParameters.JumpSpeed;

                if (verticalMovementParameters.cancelJumpOnRelease)
                    isAllowedToCancelJump = true;

            }


        }

        /// <summary>
        /// Returns the jump direction vector whenever the jump action is started.
        /// </summary>
        protected virtual Vector3 SetJumpDirection()
        {
            return CharacterActor.Up;
        }

    }
}