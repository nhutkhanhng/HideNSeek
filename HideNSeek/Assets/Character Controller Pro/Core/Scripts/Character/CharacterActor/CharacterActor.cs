using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    public enum CharacterActorState
    {
        NotGrounded,
        StableGrounded,
        UnstableGrounded
    }


    /// <summary>
    /// This class represents a character actor. It contains all the character information, collision flags, collision events, and so on. It also responsible for the execution order 
    /// of everything related to the character, such as movement, rotation, teleportation, rigidbodies interactions, body size, etc. Since the character can be 2D or 3D, this abstract class must be implemented in the 
    /// two formats, one for 2D and one for 3D.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Actor")]
    [RequireComponent(typeof(CharacterBody))]
    [DefaultExecutionOrder(ExecutionOrder.CharacterActorOrder)]
    public partial class CharacterActor : MonoBehaviour
    {

        [Header("Stability")]
        [Tooltip("The slope limit set the maximum angle considered as stable.")]
        [Range(1f, 85f)]
        public float slopeLimit = 60f;

        [Header("Grounding")]
        [Tooltip("Prevents the character from enter grounded state (IsGrounded will be false)")]
        public bool alwaysNotGrounded = false;

        [Condition("alwaysNotGrounded", ConditionAttribute.ConditionType.IsFalse)]
        [Tooltip("If enabled the character will do an initial ground check (at \"Start\"). If the test fails the starting state will be \"Not grounded\".")]

        public bool forceGroundedAtStart = true;

        [Space(10f)]

        [Tooltip("The offset distance applied to the bottom of the character. A higher offset means more walkable surfaces")]
        [Min(0f)]
        public float stepUpDistance = 0.5f;

        [Tooltip("The distance the character is capable of detecting ground. Use this value to clamp (or not) the character to the ground.")]
        [Min(0f)]
        public float stepDownDistance = 0.5f;


        [Space(10f)]

        [Tooltip("With this enabled the character bottom sphere (capsule) will be simulated as a cylinder. This works only when the character is standing on an edge.")]
        public bool edgeCompensation = false;

        [Space(10f)]

        [Tooltip("This will prevent the character from stepping over an unstable surface (a \"bad\" step). This requires a bit more processing, so if your character does not need this level of precision " +
        "you can disable it.")]
        public bool preventBadSteps = true;



        [Space(10f)]


        [Tooltip("Should the character be affected by the movement of the ground.")]
        public bool supportDynamicGround = true;


        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue)]
        [Tooltip("The character will react to RigidbodyComponent components associated with the ground. This component is just a \"smart wrapper\" around Unity's Rigidbody (2D and 3D). " +
        "From the character perspective it has a higher priority, meaning that the character will always choose to use it instead of vanilla rigidbodies (see the \"supportVanillaRigidbodies\" property for more info).\n\n" +
        "TIP: Use RigidbodyComponent components if possible (especially for kinematic platforms). The logic needs to use this component as well.")]
        public bool supportRigidbodyComponent = true;

        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue)]
        [Tooltip("The character will react to vanilla rigidbodies (a.k.a Rigidbody/Rigidbody2D), whether they are kinematic or dynamic. This is very useful for the character to be influenced by rigidbodies without a " +
        "pre-defined movement/rotation logic (e.g. some random box in the scene).\n\n" +
        "NOTE: In this case a RigidbodyComponent component is not required for the ground to affect the character movement, however, the result is not perfect. Use it as a fallback setting " +
        "(in case the ground is not a RigidbodyComponent).")]
        public bool supportVanillaRigidbodies = true;

        [Condition("supportDynamicGround", ConditionAttribute.ConditionType.IsTrue)]
        [Tooltip("If this option is enabled the forward direction of the character will be affected by the rotation of the ground (only yaw motion allowed).")]
        public bool rotateForwardDirection = true;



        [Header("Velocity")]

        [Tooltip("Whether or not to project the initial velocity (stable) onto walls.")]
        [SerializeField]
        public bool slideOnWalls = true;

        [Tooltip("Should the actor re-assign the rigidbody velocity after the simulation is done?\n\n" +
        "PreSimulationVelocity: the character uses the velocity prior to the simulation (modified by this component).\nPostSimulationVelocity: the character uses the velocity received from the simulation (no re-assignment).\nInputVelocity: the character \"gets back\" its initial velocity (before being modified by this component).")]
        public CharacterVelocityMode stablePostSimulationVelocity = CharacterVelocityMode.UsePostSimulationVelocity;

        [Tooltip("Should the actor re-assign the rigidbody velocity after the simulation is done?\n\n" +
        "PreSimulationVelocity: the character uses the velocity prior to the simulation (modified by this component).\nPostSimulationVelocity: the character uses the velocity received from the simulation (no re-assignment).\nInputVelocity: the character \"gets back\" its initial velocity (before being modified by this component).")]
        public CharacterVelocityMode unstablePostSimulationVelocity = CharacterVelocityMode.UsePostSimulationVelocity;


        [Header("Size")]

        [Min(0f)]
        public float sizeLerpSpeed = 8f;

        [Header("Rotation")]

        [Tooltip("Should this component define the character \"Up\" direction?")]
        public bool constraintRotation = true;

        [Condition("constraintRotation", ConditionAttribute.ConditionType.IsTrue)]
        [Tooltip("The desired up direction.")]
        public Vector3 constraintUpDirection = Vector3.up;

        [Condition("constraintRotation", ConditionAttribute.ConditionType.IsTrue)]
        public Transform upDirectionReference = null;

        [Condition("upDirectionReference", ConditionAttribute.ConditionType.HasReference)]
        public VerticalAlignmentSettings.VerticalReferenceMode upDirectionReferenceMode = VerticalAlignmentSettings.VerticalReferenceMode.Away;


        [Header("Physics")]

        public bool canPushDynamicRigidbodies = true;

        [Condition("canPushDynamicRigidbodies", ConditionAttribute.ConditionType.IsTrue)]
        public LayerMask pushableRigidbodyLayerMask = -1;

        public bool applyWeightToGround = true;

        [Condition("applyWeightToGround", ConditionAttribute.ConditionType.IsTrue)]
        public float weightGravity = CharacterConstants.DefaultGravity;


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


        public float StepOffset
        {
            get
            {
                return stepUpDistance - BodySize.x / 2f;
            }
        }

        public RigidbodyComponent RigidbodyComponent => CharacterBody.RigidbodyComponent;

        public ColliderComponent ColliderComponent => CharacterBody.ColliderComponent;

        public PhysicsComponent PhysicsComponent { get; private set; }

        public CharacterBody CharacterBody { get; private set; }

        public CharacterActorState CurrentState
        {
            get
            {
                if (IsGrounded)
                    return IsStable ? CharacterActorState.StableGrounded : CharacterActorState.UnstableGrounded;
                else
                    return CharacterActorState.NotGrounded;
            }
        }

        public CharacterActorState PreviousState
        {
            get
            {
                if (WasGrounded)
                    return WasStable ? CharacterActorState.StableGrounded : CharacterActorState.UnstableGrounded;
                else
                    return CharacterActorState.NotGrounded;
            }
        }


        Dictionary<Transform, Terrain> terrains = new Dictionary<Transform, Terrain>();
        Dictionary<Transform, RigidbodyComponent> groundRigidbodyComponents = new Dictionary<Transform, RigidbodyComponent>();

        public float GroundedTime { get; private set; }
        public float NotGroundedTime { get; private set; }

        public float StableGroundedTime { get; private set; }
        public float UnstableGroundedTime { get; private set; }


        /// <summary>
        /// Gets the current body size (width and height).
        /// </summary>
        public Vector2 BodySize { get; private set; }

        /// <summary>
        /// Gets the current body size (width and height).
        /// </summary>
        public Vector2 DefaultBodySize => CharacterBody.BodySize;




        /// <summary>
        /// Gets the rigidbody velocity.
        /// </summary>
        public Vector3 Velocity
        {
            get
            {
                return RigidbodyComponent.Velocity;
            }
            set
            {
                RigidbodyComponent.Velocity = value;
            }
        }

        /// <summary>
        /// Gets the rigidbody velocity projected onto a plane formed by its up direction.
        /// </summary>
        public Vector3 PlanarVelocity
        {
            get
            {
                return Vector3.ProjectOnPlane(Velocity, Up);
            }
            set
            {
                Velocity = Vector3.ProjectOnPlane(value, Up) + VerticalVelocity;
            }
        }

        /// <summary>
        /// Gets the rigidbody velocity projected onto its up direction.
        /// </summary>
        public Vector3 VerticalVelocity
        {
            get
            {
                return Vector3.Project(Velocity, Up);
            }
            set
            {
                Velocity = PlanarVelocity + Vector3.Project(value, Up);
            }
        }

        /// <summary>
        /// Gets the rigidbody velocity projected onto a plane formed by its up direction.
        /// </summary>
        public Vector3 StableVelocity
        {
            get
            {
                return CustomUtilities.ProjectVectorOnPlane(Velocity, GroundStableNormal, Up);
            }
            set
            {
                Velocity = CustomUtilities.ProjectVectorOnPlane(value, GroundStableNormal, Up);
            }
        }


        public Vector3 LastGroundedVelocity { get; private set; }



        /// <summary>
        /// Gets the rigidbody local velocity.
        /// </summary>
        public Vector3 LocalVelocity
        {
            get
            {
                return transform.InverseTransformVectorUnscaled(Velocity);
            }
            set
            {
                Velocity = transform.TransformVectorUnscaled(value);
            }
        }

        /// <summary>
        /// Gets the rigidbody local planar velocity.
        /// </summary>
        public Vector3 LocalPlanarVelocity
        {
            get
            {
                return transform.InverseTransformVectorUnscaled(PlanarVelocity);
            }
            set
            {
                PlanarVelocity = transform.TransformVectorUnscaled(value);
            }
        }




        /// <summary>
        /// Returns true if the character local vertical velocity is less than zero. 
        /// </summary>
        public bool IsFalling
        {
            get
            {
                return LocalVelocity.y < 0f;
            }
        }

        /// <summary>
        /// Returns true if the character local vertical velocity is greater than zero.
        /// </summary>
        public bool IsAscending
        {
            get
            {
                return LocalVelocity.y > 0f;
            }
        }



        #region public Body properties
        public Vector3 Center { get { return GetCenter(Position); } }
        public Vector3 Top
        {
            get
            {
                return GetTop(Position);
            }
        }
        public Vector3 Bottom
        {
            get
            {
                return GetBottom(Position);
            }
        }

        public Vector3 TopCenter
        {
            get
            {
                return GetTopCenter(Position);
            }
        }
        public Vector3 BottomCenter
        {
            get
            {
                return GetBottomCenter(Position, 0f);
            }
        }
        public Vector3 OffsettedBottomCenter
        {
            get
            {
                return GetBottomCenter(Position, StepOffset);
            }
        }

        #endregion

        #region Body functions        
        public Vector3 GetCenter(Vector3 position)
        {
            return position + Up * BodySize.y / 2f;
        }
        public Vector3 GetTop(Vector3 position)
        {
            return position + Up * (BodySize.y - CharacterConstants.SkinWidth);
        }
        public Vector3 GetBottom(Vector3 position)
        {
            return position + Up * CharacterConstants.SkinWidth;
        }        
        public Vector3 GetTopCenter(Vector3 position)
        {
            return position + Up * (BodySize.y - BodySize.x / 2f);
        }

        /// <summary>
        /// Gets the center of the top sphere of the collision shape (considering an arbitrary body size).
        /// </summary>
        public Vector3 GetTopCenter(Vector3 position, Vector2 bodySize)
        {
            return position + Up * (bodySize.y - bodySize.x / 2f);
        }

        /// <summary>
        /// Gets the center of the bottom sphere of the collision shape.
        /// </summary>
        public Vector3 GetBottomCenter(Vector3 position, float bottomOffset = 0f)
        {
            return position + Up * (BodySize.x / 2f + bottomOffset);
        }


        /// <summary>
        /// Gets the center of the bottom sphere of the collision shape (considering an arbitrary body size).
        /// </summary>
        public Vector3 GetBottomCenter(Vector3 position, Vector2 bodySize)
        {
            return position + Up * bodySize.x / 2f;
        }

        /// <summary>
        /// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
        /// </summary>
        public Vector3 GetBottomCenterToTopCenter()
        {
            return Up * (BodySize.y - BodySize.x);
        }

        /// <summary>
        /// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
        /// </summary>
        public Vector3 GetBottomCenterToTopCenter(Vector2 bodySize)
        {
            return Up * (bodySize.y - bodySize.x);
        }

        // /// <summary>
        // /// Gets the center of the bottom sphere of the collision shape, considering the collider bottom offset.
        // /// </summary>
        // public Vector3 GetOffsettedBottomCenter( Vector3 position )
        // {
        // 	return position + Up * ( bodySize.x / 2f + StepOffset );
        // }

        #endregion

        public float SlopeLimit => slopeLimit;

        /// <summary>
        /// Gets the current rigidbody position.
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return RigidbodyComponent.Position;
            }
            set
            {
                RigidbodyComponent.Position = value;
            }
        }

        /// <summary>
        /// Gets the current rigidbody rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return RigidbodyComponent.Rotation;
            }
            set
            {
                RigidbodyComponent.Rotation = value;
            }
        }

        /// <summary>
        /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.up).
        /// </summary>
        public Vector3 Up
        {
            get
            {
                return RigidbodyComponent.Up;
            }
            set
            {
                if (value == Vector3.zero)
                    return;
                Quaternion.LookRotation(Forward, value);
                Quaternion deltaRotation = Quaternion.FromToRotation(Up, value.normalized);
                Rotation = deltaRotation * Rotation;
            }
        }
        /// <summary>
        /// Gets the current forward direction based on the rigidbody rotation (not necessarily transform.forward).
        /// </summary>
        public Vector3 Forward
        {
            get
            {
                return Rotation * Vector3.forward;
            }
            set
            {

                if (value == Vector3.zero)
                    return;

                Quaternion deltaRotation = Quaternion.FromToRotation(Forward, value.normalized);
                Rotation = deltaRotation * Rotation;
            }
        }

        /// <summary>
        /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.right).
        /// </summary>
        public Vector3 Right
        {
            get
            {
                return Rotation * Vector3.right;
            }
        }

        public bool IsKinematic
        {
            get
            {
                return RigidbodyComponent.IsKinematic;
            }
            set
            {
                RigidbodyComponent.IsKinematic = value;
            }
        }

        public enum CharacterVelocityMode
        {
            UseInputVelocity,
            UsePreSimulationVelocity,
            UsePostSimulationVelocity
        }


        public Vector3 InputVelocity { get; private set; }
        public Vector3 PreSimulationVelocity { get; private set; }
        public Vector3 PostSimulationVelocity { get; private set; }

        /// <summary>
        /// Gets the difference between the post-simulation velocity (after the physics simulation) and the pre-simulation velocity (just before the physics simulation). 
        /// This value is useful to detect any external response due to the physics simulation, such as hits coming from other rigidbodies.
        /// </summary>
        public Vector3 ExternalVelocity { get; private set; }





        Vector2 targetBodySize;

        void HandleSize(float dt)
        {
            BodySize = Vector2.Lerp(BodySize, targetBodySize, sizeLerpSpeed * dt);
            SetColliderSize();
        }

        #region Events



        public event System.Action<Contact> OnHeadHit;
        public event System.Action<Contact> OnWallHit;
        public event System.Action<Vector3, Quaternion> OnTeleport;
        public event System.Action<Vector3> OnGroundedStateEnter;
        public event System.Action OnGroundedStateExit;


        #endregion


        public Terrain CurrentTerrain { get; private set; }

        public bool IsOnTerrain => CurrentTerrain != null;

    


        /// <summary>
        /// Checks if the new character size fits in place. If this check is valid then the real size of the character is changed.
        /// </summary>
        public bool SetBodySize(Vector2 size)
        {
            HitInfoFilter filter = new HitInfoFilter(PhysicsComponent.CollisionLayerMask, true, true);
            if (!characterCollisions.CheckBodySize(size, Position, filter))
                return false;

            targetBodySize = size;

            return true;
        }



        bool IsAStableEdge(CollisionInfo collisionInfo)
        {
            return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle <= slopeLimit;
        }

        bool IsAnUnstableEdge(CollisionInfo collisionInfo)
        {
            return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle > slopeLimit;
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (CharacterBody == null)
                CharacterBody = GetComponent<CharacterBody>();

            Gizmos.color = new Color(1f, 1f, 1f, 0.2f);

            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 origin = Vector3.up * stepUpDistance;
            Gizmos.DrawWireCube(
                origin,
                new Vector3(1.1f * CharacterBody.BodySize.x, 0.02f, 1.1f * CharacterBody.BodySize.x)
            );

            Gizmos.matrix = Matrix4x4.identity;

        }
#endif


        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────


    }


}