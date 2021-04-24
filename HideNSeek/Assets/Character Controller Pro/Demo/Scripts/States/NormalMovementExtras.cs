using UnityEngine;
using Lightbug.Utilities;


namespace Lightbug.CharacterControllerPro.Demo
{

[System.Serializable]
public class PlanarMovementParameters
{
    
    [Min( 0f )]
    public float baseSpeedLimit = 6f;    
    
    [Tooltip("\"Toggle\" will activate/deactivate the action when the input is \"pressed\". On the other hand, \"Hold\" will activate the action when the input is pressed, " + 
    "and deactivate it when the input is \"released\".")]
    public InputMode runInputMode = InputMode.Hold;

    
    [Min( 0f )]
    public float boostSpeedLimit = 10f;


    [CustomClassDrawer]
    public PlanarMovementProperties stableMovement = new PlanarMovementProperties( 50f , 40f );

    [CustomClassDrawer]
    public PlanarMovementProperties unstableGroundedMovement = new PlanarMovementProperties( 10f , 2f );

    [CustomClassDrawer]
    public PlanarMovementProperties notGroundedMovement = new PlanarMovementProperties( 10f , 5f );


    [System.Serializable]
    public struct PlanarMovementProperties
    {
        [Tooltip("How fast the character increses its current velocity.")]
        public float acceleration;

        [Tooltip("How fast the character reduces its current velocity.")]
        public float deceleration;

        public PlanarMovementProperties( float acceleration , float deceleration )
        {
            this.acceleration = acceleration;
            this.deceleration = deceleration;
        }
    }

}


[System.Serializable]
public class VerticalMovementParameters
{    

    public enum UnstableJumpMode
    {
        Vertical ,
        GroundNormal
    }
    
    
    [Header("Gravity")]

    [Tooltip("It enables/disables gravity. The gravity value is calculated based on the jump apex height and duration.")]
    public bool useGravity = true;

    [Header("Jump")]

    public bool canJump = true;

    [Space(10f)]

    [Tooltip("The height reached at the apex of the jump. The maximum height will depend on the \"jumpCancellationMode\".")]
    [Min(0f)]
    public float jumpApexHeight = 2.25f;

    [Tooltip("The amount of time to reach the \"base height\" (apex).")]
    [Min(0f)]
    public float jumpApexDuration = 0.5f;


    [Space(10f)]

    [Tooltip("Reduces the vertical velocity when the jump action is canceled.")]
    public bool cancelJumpOnRelease = true;

    [Tooltip("How much the vertical velocity is reduced when canceling the jump (0 = no effect , 1 = zero velocity).")]
    [Range( 0f , 1f )]
    public float cancelJumpMultiplier = 0.5f;

    [Tooltip("When canceling the jump (releasing the action), if the jump time is less than this value nothing is going to happen. Only when the timer is greater than this \"min time\" the jump will be affected.")]
    public float cancelJumpMinTime = 0.1f;

    [Tooltip("When canceling the jump (releasing the action), if the jump time is less than this value (and greater than the \"min time\") the velocity will be affected.")]
    public float cancelJumpMaxTime = 0.3f;  
    
    [Space(10f)]

    [Tooltip("This will help to perform the jump action after the actual input has been started. This value determines the maximum time between input and ground detection.")]
	[Min( 0f )] 
    public float preGroundedJumpTime = 0.2f;

    [Tooltip("If the character is not grounded, and the \"not grounded time\" is less or equal than this value, the jump action will be performed as a grounded jump. This is also known as \"coyote time\".")]
    [Min(0f)]
    public float postGroundedJumpTime = 0.1f;

    [Space(10f)]

    [Min(0)]
    [Tooltip("Number of jumps available for the character in the air.")]
	public int availableNotGroundedJumps = 1;

    [Space(10f)]

    public bool canJumpOnUnstableGround = false;
    

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    float gravityMagnitude = 10f;
    
    public float GravityMagnitude
    {
        get
        {
            return gravityMagnitude;
        }
    }

    float jumpSpeed = 10f;

    public void UpdateParameters( float positiveGravityMultiplier )
    { 
        gravityMagnitude = positiveGravityMultiplier * ( ( 2 * jumpApexHeight ) / Mathf.Pow( jumpApexDuration , 2 ) );
        jumpSpeed = gravityMagnitude * jumpApexDuration;
    }

    public float JumpSpeed 
    {
        get
        {
            return jumpSpeed;
        }
    }

}

[System.Serializable]
public class CrouchParameters
{   

    public bool enableCrouch = true;

    [Tooltip("This multiplier represents the crouch ratio relative to the default height.")]
    [Min( 0f )]
    public float heightRatio = 0.75f;

    [Tooltip("How much the crouch action affects the movement speed?.")]
    [Min( 0f )]
    public float speedMultiplier = 0.3f;


    [Tooltip("\"Toggle\" will activate/deactivate the action when the input is \"pressed\". On the other hand, \"Hold\" will activate the action when the input is pressed, " + 
    "and deactivate it when the input is \"released\".")]
    public InputMode inputMode = InputMode.Hold;

}

    

[System.Serializable]
public class LookingDirectionParameters
{    
    public bool followExternalReference = true; 

}


public enum InputMode
{
    Toggle ,
    Hold
}


}
