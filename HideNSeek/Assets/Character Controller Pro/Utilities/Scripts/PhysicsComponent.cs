using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component is an encapsulation of the Physics and Physics2D classes, containing the most commonly used 
/// methods from these components. Also it holds the information from the collision and trigger messages received, such as 
/// contacts, trigger, etc.
/// </summary>
public abstract class PhysicsComponent : MonoBehaviour
{

	protected int hits = 0;

	public List<Contact> Contacts { get; protected set; } = new List<Contact>();
	public List<Trigger> Triggers { get; protected set; } = new List<Trigger>();	
	
	protected abstract LayerMask GetCollisionLayerMask();
	public abstract void IgnoreLayerCollision( int layerA , int layerB , bool ignore );
	public abstract void IgnoreLayerMaskCollision( LayerMask layerMask , bool ignore );
    

	// Contacts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	public void ClearContacts()
	{	
		Contacts.Clear();
	}

	protected abstract void GetClosestHit( out HitInfo hitInfo , Vector3 castDisplacement , HitInfoFilter filter );

	// Casts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	public abstract int Raycast( out HitInfo hitInfo , Vector3 origin , Vector3 castDisplacement , HitInfoFilter filter );

	public abstract int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , HitInfoFilter filter );

	public abstract int CapsuleCast( out HitInfo hitInfo , Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , HitInfoFilter filter );
    

    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public abstract bool OverlapSphere( Vector3 center , float radius , HitInfoFilter filter );

    public abstract bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , HitInfoFilter filter );

	public LayerMask CollisionLayerMask { get; private set; } = 0;

	protected virtual void Awake()
	{
		this.hideFlags = HideFlags.None;
		
		CollisionLayerMask = GetCollisionLayerMask();
	}

	void FixedUpdate()
	{
		// Update the collision layer mask (collision matrix) of this object.
		CollisionLayerMask = GetCollisionLayerMask();

		// If there are null triggers then delete them from the list
		for( int i = Triggers.Count - 1 ; i >= 0 ; i-- )
		{					
			if( Triggers[i].gameObject == null )
				Triggers.RemoveAt( i );
		}
	}

}

}

