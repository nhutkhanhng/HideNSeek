using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Core;
using Lightbug.Utilities;
using Lightbug.CharacterControllerPro.Implementation;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lightbug.CharacterControllerPro.Demo
{



    [AddComponentMenu("Character Controller Pro/Demo/Character/States/Normal Movement")]
    public partial class NormalMovement : CharacterState
    {

        [Space(10)]

        public PlanarMovementParameters planarMovementParameters = new PlanarMovementParameters();

        public VerticalMovementParameters verticalMovementParameters = new VerticalMovementParameters();

        [Header("Animation")]

        [SerializeField]
        protected string groundedParameter = "Grounded";

        [SerializeField]
        protected string stableParameter = "Stable";

        [SerializeField]
        protected string verticalSpeedParameter = "VerticalSpeed";

        [SerializeField]
        protected string planarSpeedParameter = "PlanarSpeed";

        [SerializeField]
        protected string horizontalAxisParameter = "HorizontalAxis";

        [SerializeField]
        protected string verticalAxisParameter = "VerticalAxis";

        [SerializeField]
        protected string heightParameter = "Height";


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────



        protected MaterialController materialController = null;
        protected int notGroundedJumpsLeft = 0;
        protected bool isAllowedToCancelJump = false;
        protected bool wantToRun = false;
        protected float currentPlanarSpeedLimit = 0f;

        protected bool groundedJumpAvailable = true;


        protected float targetHeight = 1f;

        protected PlanarMovementParameters.PlanarMovementProperties currentMotion = new PlanarMovementParameters.PlanarMovementProperties();

        protected override void Awake()
        {
            base.Awake();

            notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps;

            materialController = this.GetComponentInBranch<CharacterActor, MaterialController>();
            if (materialController == null)
            {
                Debug.Log("Missing MaterialController component");
                this.enabled = false;
                return;
            }


        }

        protected override void Start()
        {
            base.Start();

            targetHeight = CharacterActor.DefaultBodySize.y;

            InitCrouch();
            //float minShrinkHeightRatio = CharacterActor.BodySize.x / CharacterActor.BodySize.y;
            //crouchParameters.heightRatio = Mathf.Max(minShrinkHeightRatio, crouchParameters.heightRatio);
        }

        void OnEnable()
        {
            CharacterActor.OnTeleport += OnTeleport;

        }

        void OnDisable()
        {
            CharacterActor.OnTeleport -= OnTeleport;
        }

        public override string GetInfo()
        {
            return "This state serves as a multi purpose movement based state. It is responsible for handling gravity and jump, walk and run, crouch, " +
            "react to the different material properties, etc. Basically it covers all the common movements involved " +
            "in a typical game, from a 3D platformer to a first person walking simulator.";
        }

        void OnTeleport(Vector3 position, Quaternion rotation)
        {
            targetLookingDirection = CharacterActor.Forward;
            isAllowedToCancelJump = false;
        }


        public bool FollowExternalReference
        {
            get
            {
                return lookingDirectionParameters.followExternalReference;
            }
            set
            {
                lookingDirectionParameters.followExternalReference = value;
            }
        }

        /// <summary>
        /// Gets/Sets the useGravity toggle. Use this property to enable/disable the effect of gravity on the character.
        /// </summary>
        /// <value></value>
        public bool UseGravity
        {
            get
            {
                return verticalMovementParameters.useGravity;
            }
            set
            {
                verticalMovementParameters.useGravity = value;
            }
        }



        public override void CheckExitTransition()
        {

            if (CharacterActions.jetPack.value)
            {
                CharacterStateController.EnqueueTransition<JetPack>();
            }
            else if (CharacterActions.dash.Started)
            {
                CharacterStateController.EnqueueTransition<Dash>();
            }
            else if (CharacterActor.Triggers.Count != 0)
            {
                CharacterStateController.EnqueueTransition<LadderClimbing>();
                CharacterStateController.EnqueueTransition<RopeClimbing>();
            }
            else if (!CharacterActor.IsGrounded)
            {
                if (!CharacterActions.crouch.value)
                    CharacterStateController.EnqueueTransition<WallSlide>();

                CharacterStateController.EnqueueTransition<LedgeHanging>();


            }

        }


        void SetMotionValues()
        {
            if (CharacterActor.IsGrounded)
            {
                if (CharacterActor.IsStable)
                    currentMotion = planarMovementParameters.stableMovement;
                else
                    currentMotion = planarMovementParameters.unstableGroundedMovement;
            }
            else
            {
                currentMotion = planarMovementParameters.notGroundedMovement;
            }


            // Material values
            if (CharacterActor.IsGrounded)
            {
                currentMotion.acceleration *= materialController.CurrentSurface.accelerationMultiplier * materialController.CurrentVolume.accelerationMultiplier;
                currentMotion.deceleration *= materialController.CurrentSurface.decelerationMultiplier * materialController.CurrentVolume.decelerationMultiplier;
            }
            else
            {
                currentMotion.acceleration *= materialController.CurrentVolume.accelerationMultiplier;
                currentMotion.deceleration *= materialController.CurrentVolume.decelerationMultiplier;
            }


        }

        /// <summary>
        /// Processes the lateral movement of the character (stable and unstable state), that is, walk, run, crouch, etc. 
        /// This movement is tied directly to the "movement" character action.
        /// </summary>
        protected virtual void ProcessPlanarMovement(float dt)
        {
            SetMotionValues();

            float speedMultiplier = materialController.CurrentSurface.speedMultiplier * materialController.CurrentVolume.speedMultiplier;

            bool needToAccelerate = false;

            switch (CharacterActor.CurrentState)
            {
                case CharacterActorState.NotGrounded:

                    if (CharacterActor.WasGrounded)
                    {
                        currentPlanarSpeedLimit = Mathf.Max(CharacterActor.PlanarVelocity.magnitude, planarMovementParameters.baseSpeedLimit);
                    }


                    needToAccelerate = (CharacterStateController.InputMovementReference * currentPlanarSpeedLimit).magnitude >= CharacterActor.PlanarVelocity.magnitude;


                    CharacterActor.PlanarVelocity = Vector3.MoveTowards(
                        CharacterActor.PlanarVelocity,
                        currentPlanarSpeedLimit * CharacterStateController.InputMovementReference * speedMultiplier,
                        (needToAccelerate ? currentMotion.acceleration : currentMotion.deceleration) * dt
                    );

                    break;
                case CharacterActorState.StableGrounded:



                    // Run ------------------------------------------------------------
                    if (planarMovementParameters.runInputMode == InputMode.Toggle)
                    {
                        if (CharacterActions.run.Started)
                            wantToRun = !wantToRun;
                    }
                    else
                    {
                        wantToRun = CharacterActions.run.value;
                    }

                    if (wantToCrouch)
                        wantToRun = false;

                    if (isCrouched)
                    {
                        currentPlanarSpeedLimit = planarMovementParameters.baseSpeedLimit * crouchParameters.speedMultiplier;
                    }
                    else
                    {
                        currentPlanarSpeedLimit = wantToRun ? planarMovementParameters.boostSpeedLimit : planarMovementParameters.baseSpeedLimit;
                    }

                    Vector3 targetPlanarVelocity = currentPlanarSpeedLimit * CharacterStateController.InputMovementReference * speedMultiplier;

                    needToAccelerate = CharacterStateController.InputMovementReference != Vector3.zero;


                    // Set the velocity
                    CharacterActor.PlanarVelocity = Vector3.MoveTowards(
                        CharacterActor.PlanarVelocity,
                        targetPlanarVelocity,
                        (needToAccelerate ? currentMotion.acceleration : currentMotion.deceleration) * dt
                    );


                    break;
                case CharacterActorState.UnstableGrounded:

                    currentPlanarSpeedLimit = planarMovementParameters.baseSpeedLimit;

                    needToAccelerate = (CharacterStateController.InputMovementReference * currentPlanarSpeedLimit).magnitude >= CharacterActor.PlanarVelocity.magnitude;

                    CharacterActor.PlanarVelocity = Vector3.MoveTowards(
                        CharacterActor.PlanarVelocity,
                        currentPlanarSpeedLimit * CharacterStateController.InputMovementReference * speedMultiplier,
                        (needToAccelerate ? currentMotion.acceleration : currentMotion.deceleration) * dt
                    );

                    break;
            }

        }



        protected virtual void ProcessGravity(float dt)
        {
            if (!verticalMovementParameters.useGravity)
                return;

            verticalMovementParameters.UpdateParameters(materialController.CurrentVolume.gravityAscendingMultiplier);

            float gravityMultiplier = CharacterActor.LocalVelocity.y >= 0 ?
            materialController.CurrentVolume.gravityAscendingMultiplier :
            materialController.CurrentVolume.gravityDescendingMultiplier;

            float gravity = gravityMultiplier * verticalMovementParameters.GravityMagnitude;


            if (!CharacterActor.IsStable)
                CharacterActor.VerticalVelocity += -CharacterActor.Up * (gravity * dt);



        }





        void ProcessVerticalMovement(float dt)
        {
            ProcessGravity(dt);
            ProcessJump(dt);

        }

        public override void EnterBehaviour(float dt, CharacterState fromState)
        {

            CharacterActor.alwaysNotGrounded = false;

            targetLookingDirection = CharacterActor.Forward;

            if (fromState == CharacterStateController.GetState<WallSlide>())
            {
                // "availableNotGroundedJumps + 1" because the update code will consume one jump!
                notGroundedJumpsLeft = verticalMovementParameters.availableNotGroundedJumps + 1;
            }

            currentPlanarSpeedLimit = Mathf.Max(CharacterActor.PlanarVelocity.magnitude, planarMovementParameters.baseSpeedLimit);

        }


        public override void UpdateBehaviour(float dt)
        {
            HandleSize(dt);
            HandleVelocity(dt);
            HandleRotation(dt);

        }


        public override void PostUpdateBehaviour(float dt)
        {
            if (CharacterStateController.Animator == null)
                return;

            if (CharacterStateController.Animator.runtimeAnimatorController == null)
                return;

            if (!CharacterStateController.Animator.gameObject.activeSelf)
                return;

            CharacterStateController.Animator.SetBool(groundedParameter, CharacterActor.IsGrounded);
            CharacterStateController.Animator.SetBool(stableParameter, CharacterActor.IsStable);
            CharacterStateController.Animator.SetFloat(verticalSpeedParameter, CharacterActor.LocalVelocity.y);
            CharacterStateController.Animator.SetFloat(planarSpeedParameter, CharacterActor.PlanarVelocity.magnitude);
            CharacterStateController.Animator.SetFloat(horizontalAxisParameter, CharacterActions.movement.value.x);
            CharacterStateController.Animator.SetFloat(verticalAxisParameter, CharacterActions.movement.value.y);
            CharacterStateController.Animator.SetFloat(heightParameter, CharacterActor.BodySize.y);

        }


        protected virtual void HandleSize(float dt)
        {
            // Want to crouch ---------------------------------------------------------------------    
            if (CharacterActor.IsGrounded && crouchParameters.enableCrouch)
            {
                if (crouchParameters.inputMode == InputMode.Toggle)
                {
                    if (CharacterActions.crouch.Started)
                        wantToCrouch = !wantToCrouch;
                }
                else
                {
                    wantToCrouch = CharacterActions.crouch.value;
                }

            }
            else
            {
                wantToCrouch = false;
            }

            if (wantToRun)
                wantToCrouch = false;

            // Process Size ----------------------------------------------------------------------------        
            targetHeight = wantToCrouch ? CharacterActor.DefaultBodySize.y * crouchParameters.heightRatio : CharacterActor.DefaultBodySize.y;

            Vector3 targetSize = new Vector2(CharacterActor.DefaultBodySize.x, targetHeight);

            bool validSize = CharacterActor.SetBodySize(targetSize);

            if (validSize)
                isCrouched = wantToCrouch;

        }



        protected virtual void HandleVelocity(float dt)
        {
            ProcessVerticalMovement(dt);
            ProcessPlanarMovement(dt);
        }
    }


}






