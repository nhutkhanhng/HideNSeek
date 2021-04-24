using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.CharacterControllerPro.Implementation;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Demo
{
[AddComponentMenu("Character Controller Pro/Demo/Character/States/Wall Slide")]
public class WallSlide : CharacterState
{

    [Header("Filter")]

    [SerializeField]
    protected bool filterByTag = true;

    [Condition("filterByTag" , ConditionAttribute.ConditionType.IsTrue ) ]
    [SerializeField]
    protected string wallTag = "WallSlide";

    
    [Header("Slide")]

    [SerializeField]
    protected float slideAcceleration = 10f;

    [Range( 0f , 1f )]
    [SerializeField]
    protected float initialIntertia = 0.4f;

    [Header("Grab")]

    public bool enableGrab = true;

    [Condition("enableGrab" , ConditionAttribute.ConditionType.IsTrue ) ]
    [SerializeField]
    protected float grabHorizontalSpeed = 1f;

    [Condition("enableGrab" , ConditionAttribute.ConditionType.IsTrue ) ]
    [SerializeField]
    protected float grabVerticalSpeed = 3f;

    

    [Header("Size")]

    [SerializeField]
    protected bool modifySize = true;

    [Condition("modifySize" , ConditionAttribute.ConditionType.IsTrue ) ]
    [SerializeField]
    protected float width = 0.5f;

    [Condition("modifySize" , ConditionAttribute.ConditionType.IsTrue ) ]
    [SerializeField]
    protected float height = 1.5f;

    [Header("Jump")]

    [SerializeField]
    protected float jumpNormalVelocity = 5f;

    [SerializeField]
    protected float jumpVerticalVelocity = 10f;

    [Header("Animation")]

    [SerializeField]
    protected string horizontalVelocityParameter = "HorizontalVelocity";

    [SerializeField]
    protected string verticalVelocityParameter = "VerticalVelocity";

    [SerializeField]
    protected string grabParameter = "Grab";

    [SerializeField]
    protected string movementDetectedParameter = "MovementDetected";

    

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    protected bool wallJump = false;
    protected Vector2 initialSize = Vector2.zero;
    
	public override void CheckExitTransition()
    {
        if( CharacterActions.crouch.value || CharacterActor.IsGrounded || !CharacterActor.WallCollision || !CheckCenterRay() )
        {
            CharacterStateController.EnqueueTransition<NormalMovement>();
        }
        else if( CharacterActions.jump.Started )
        {          
            wallJump = true;

            CharacterStateController.EnqueueTransition<NormalMovement>();
        }
        else
        {
            CharacterStateController.EnqueueTransition<LedgeHanging>();            
        }
	}

    
	public override bool CheckEnterTransition( CharacterState fromState )
    {
        if( CharacterActor.IsAscending )
            return false;

        if( !CharacterActor.WallCollision )
            return false;
        
        if( filterByTag )
            if( !CharacterActor.WallContact.gameObject.CompareTag( wallTag ) )
                return false;
        
        HitInfo centerRayHitInfo;
        CharacterActor.PhysicsComponent.Raycast( 
            out centerRayHitInfo , 
            CharacterActor.Center , 
            - CharacterActor.WallContact.normal * 1.2f * CharacterActor.BodySize.x , 
            new HitInfoFilter( CharacterActor.PhysicsComponent.CollisionLayerMask , true , true )
        );

        if( !CheckCenterRay() )
            return false;
        
        // Vector3 centerToWallContact = CharacterActor.WallContact.point - CharacterActor.Center;
        // Vector3 localCenterToWallContact = CharacterActor.transform.InverseTransformVectorUnscaled( Vector3.Project( centerToWallContact , CharacterActor.Up ) );
        // if( localCenterToWallContact.y > 0.1f )
        //     return false;
        
        return true;
	}

    bool CheckCenterRay()
    {
        HitInfo centerRayHitInfo;
        CharacterActor.PhysicsComponent.Raycast( 
            out centerRayHitInfo , 
            CharacterActor.Center , 
            - CharacterActor.WallContact.normal * 1.2f * CharacterActor.BodySize.x , 
            new HitInfoFilter( CharacterActor.PhysicsComponent.CollisionLayerMask , true , true )
        );

        if( centerRayHitInfo.hit && centerRayHitInfo.transform.gameObject == CharacterActor.WallContact.gameObject)
            return true;

        return false;
    }

    

    public override void EnterBehaviour( float dt , CharacterState fromState )
    {
        
        CharacterActor.Velocity *= initialIntertia; 
        CharacterActor.Forward = - CharacterActor.WallContact.normal;

        if( modifySize )
        {
            initialSize = CharacterActor.BodySize;
            CharacterActor.SetBodySize( new Vector2( width , height ) );
        }
    }

    public override void ExitBehaviour( float dt, CharacterState toState )
    {
        if( wallJump )
        {
            wallJump = false;
                      
            CharacterActor.Velocity = jumpVerticalVelocity * CharacterActor.Up + jumpNormalVelocity * CharacterActor.WallContact.normal;
        }

        if( modifySize )
        {
            CharacterActor.SetBodySize( initialSize );
        }
    }
    
    protected bool IsGrabbing => CharacterActions.run.value && enableGrab;

	public override void UpdateBehaviour( float dt )
    {
        if( IsGrabbing )
        {
            Vector3 rightDirection = Vector3.ProjectOnPlane( CharacterStateController.MovementReferenceRight , CharacterActor.WallContact.normal ).normalized;
            Vector3 upDirection = CharacterActor.Up;
            Vector3 targetVelocity =  CharacterActions.movement.value.x * rightDirection * grabHorizontalSpeed + 
            CharacterActions.movement.value.y * upDirection * grabVerticalSpeed;
            CharacterActor.Velocity = Vector3.MoveTowards( CharacterActor.Velocity , targetVelocity , 10f * dt );


        }
        else
        {
            CharacterActor.VerticalVelocity += - CharacterActor.Up * slideAcceleration * dt;
        }
	}

    public override void PostUpdateBehaviour(float dt)
    {
        CharacterStateController.Animator.SetFloat( horizontalVelocityParameter , CharacterActor.LocalVelocity.x );
        CharacterStateController.Animator.SetFloat( verticalVelocityParameter , CharacterActor.LocalVelocity.y );
        CharacterStateController.Animator.SetBool( grabParameter , IsGrabbing );
        CharacterStateController.Animator.SetBool( movementDetectedParameter , CharacterActions.movement.Detected );
        
    }

    public override void UpdateIK(int layerIndex)
    {        
        if( IsGrabbing && CharacterActions.movement.Detected )
        {   
            CharacterStateController.Animator.SetLookAtWeight( Mathf.Clamp01( CharacterActor.Velocity.magnitude ) , 0f , 0.2f );
            CharacterStateController.Animator.SetLookAtPosition( CharacterActor.Position + CharacterActor.Velocity );
        }
        else
        {
            CharacterStateController.Animator.SetLookAtWeight( 0f );
        }
        
    }



    

    
    
}

}
