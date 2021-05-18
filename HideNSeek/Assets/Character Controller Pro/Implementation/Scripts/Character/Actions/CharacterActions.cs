using UnityEngine;

namespace Lightbug.CharacterControllerPro.Implementation
{

    /// <summary>
    /// This struct contains all the inputs actions available for the character to interact with.
    /// </summary>
    [System.Serializable]
    public struct CharacterActions
    {
        public const string _JUMP = "Jump";
        public const string _RUN = "Run";
        public const string _INTERACT = "Interact";
        public const string _JETPACK = "Jet Pack";
        public const string _DASH = "Dash";
        public const string _CROUCH = "Crouch";

        public const string _MOVEMENT = "Movement";

        // Bool actions
        public BoolAction @jump;
        public BoolAction @run;
        public BoolAction @interact;
        public BoolAction @jetPack;
        public BoolAction @dash;
        public BoolAction @crouch;


        // Float actions


        // Vector2 actions
        public Vector2Action @movement;

        /// <summary>
        /// Reset all the actions.
        /// </summary>
        public void Reset()
        {
            @jump.Reset();
            @run.Reset();
            @interact.Reset();
            @jetPack.Reset();
            @dash.Reset();
            @crouch.Reset();


            @movement.Reset();

        }

        /// <summary>
        /// Initializes all the actions by instantiate them. Each action will be instantiated with its specific type (Bool, Float or Vector2).
        /// </summary>
        public void InitializeActions()
        {
            @jump = new BoolAction();
            @jump.Initialize();

            @run = new BoolAction();
            @run.Initialize();

            @interact = new BoolAction();
            @interact.Initialize();

            @jetPack = new BoolAction();
            @jetPack.Initialize();

            @dash = new BoolAction();
            @dash.Initialize();

            @crouch = new BoolAction();
            @crouch.Initialize();



            @movement = new Vector2Action();

        }

        /// <summary>
        /// Updates the values of all the actions based on the current input handler (human).
        /// </summary>
        public void SetValues(InputHandler inputHandler)
        {
            if (inputHandler == null)
                return;

            @jump.value = inputHandler.GetBool(_JUMP);
            @run.value = inputHandler.GetBool(_RUN);
            @interact.value = inputHandler.GetBool(_INTERACT);
            @jetPack.value = inputHandler.GetBool(_JETPACK);
            @dash.value = inputHandler.GetBool(_DASH);
            @crouch.value = inputHandler.GetBool(_CROUCH);


            @movement.value = inputHandler.GetVector2(_MOVEMENT);

        }

        /// <summary>
        /// Copies the values of all the actions from an existing set of actions.
        /// </summary>
        public void SetValues(CharacterActions characterActions)
        {
            @jump.value = characterActions.jump.value;
            @run.value = characterActions.run.value;
            @interact.value = characterActions.interact.value;
            @jetPack.value = characterActions.jetPack.value;
            @dash.value = characterActions.dash.value;
            @crouch.value = characterActions.crouch.value;


            @movement.value = characterActions.movement.value;

        }

        /// <summary>
        /// Update all the actions internal states.
        /// </summary>
        public void Update(float dt)
        {
            @jump.Update(dt);
            @run.Update(dt);
            @interact.Update(dt);
            @jetPack.Update(dt);
            @dash.Update(dt);
            @crouch.Update(dt);

        }


    }


}