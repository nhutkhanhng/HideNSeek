using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    public partial class CharacterActor
    {
        #region Collision Properties

        /// <summary>
        /// Returns true if the character is standing on an edge.
        /// </summary>
        public bool IsOnEdge { get { return characterCollisionInfo.isOnEdge; } }

        /// <summary>
        /// Gets the grounded state, true if the ground object is not null, false otherwise.
        /// </summary>
        public bool IsGrounded { get { return characterCollisionInfo.groundObject != null; } }


        /// <summary>
        /// Gets the angle between the up vector and the stable normal.
        /// </summary>
        public float GroundSlopeAngle { get { return characterCollisionInfo.groundSlopeAngle; } }

        /// <summary>
        /// Gets the contact point obtained directly from the ground test (sphere cast).
        /// </summary>
        public Vector3 GroundContactPoint { get { return characterCollisionInfo.groundContactPoint; } }

        /// <summary>
        /// Gets the normal vector obtained directly from the ground test (sphere cast).
        /// </summary>
        public Vector3 GroundContactNormal { get { return characterCollisionInfo.groundContactNormal; } }
        public Vector3 GroundStableNormal { get { return IsStable ? characterCollisionInfo.groundStableNormal : Up; } }
        public GameObject GroundObject { get { return characterCollisionInfo.groundObject; } }
        public Transform GroundTransform { get { return GroundObject != null ? GroundObject.transform : null; } }

        /// <summary>
        /// Gets the Collider3D component of the current ground.
        /// </summary>
        public Collider GroundCollider3D { get { return characterCollisionInfo.groundCollider3D; } }


        // Wall ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────	

        /// <summary>
        /// Gets the wall collision flag, true if the character hit a wall, false otherwise.
        /// </summary>
        public bool WallCollision { get { return characterCollisionInfo.wallCollision; } }

        /// <summary>
        /// Gets the angle between the contact normal (wall collision) and the Up direction.
        /// </summary>	
        public float WallAngle { get { return characterCollisionInfo.wallAngle; } }

        /// <summary>
        /// Gets the current contact (wall collision).
        /// </summary>
        public Contact WallContact { get { return characterCollisionInfo.wallContact; } }


        // Head ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────	
        public bool HeadCollision
        {
            get
            {
                return characterCollisionInfo.headCollision;
            }
        }
        public float HeadAngle
        {
            get
            {
                return characterCollisionInfo.headAngle;
            }
        }

        public Contact HeadContact
        {
            get
            {
                return characterCollisionInfo.headContact;
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
        public bool IsStable
        {
            get
            {
                return IsGrounded && characterCollisionInfo.groundSlopeAngle <= slopeLimit;
            }
        }
        public bool IsOnUnstableGround
        {
            get
            {
                return IsGrounded && characterCollisionInfo.groundSlopeAngle > slopeLimit;
            }
        }
        public bool WasGrounded { get; private set; }
        public bool WasStable { get; private set; }
        public RigidbodyComponent GroundRigidbodyComponent
        {
            get
            {
                if (!IsStable)
                    groundRigidbodyComponent = null;

                return groundRigidbodyComponent;
            }
        }

        RigidbodyComponent groundRigidbodyComponent = null;
        public bool IsGroundARigidbody
        {
            get
            {
                return characterCollisionInfo.groundRigidbody3D != null;
            }
        }
        public bool IsGroundAKinematicRigidbody
        {
            get
            {
                return characterCollisionInfo.groundRigidbody3D.isKinematic;
            }
        }

        public Vector3 DynamicGroundPointVelocity
        {
            get
            {
                if (GroundRigidbodyComponent == null)
                    return Vector3.zero;

                return GroundRigidbodyComponent.GetPointVelocity(Position);

            }
        }

        public override string ToString()
        {
            const string nullString = " ---- ";


            string triggerString = "";

            for (int i = 0; i < Triggers.Count; i++)
            {
                triggerString += " - " + Triggers[i].gameObject.name + "\n";
            }

            return string.Concat(
                "Ground : \n",
                "──────────────────\n",
                "Is Grounded : ", IsGrounded, '\n',
                "Is Stable : ", IsStable, '\n',
                "Slope Angle : ", characterCollisionInfo.groundSlopeAngle, '\n',
                "Is On Edge : ", characterCollisionInfo.isOnEdge, '\n',
                "Edge Angle : ", characterCollisionInfo.edgeAngle, '\n',
                "Object Name : ", characterCollisionInfo.groundObject != null ? characterCollisionInfo.groundObject.name : nullString, '\n',
                "Layer : ", LayerMask.LayerToName(characterCollisionInfo.groundLayer), '\n',
                "Rigidbody Type: ", GroundRigidbodyComponent != null ? GroundRigidbodyComponent.IsKinematic ? "Kinematic" : "Dynamic" : nullString, '\n',
                "Dynamic Ground : ", GroundRigidbodyComponent != null ? "Yes" : "No", "\n\n",
                "Wall : \n",
                "──────────────────\n",
                "Wall Collision : ", characterCollisionInfo.wallCollision, '\n',
                "Wall Angle : ", characterCollisionInfo.wallAngle, "\n\n",
                "Head : \n",
                "──────────────────\n",
                "Head Collision : ", characterCollisionInfo.headCollision, '\n',
                "Head Angle : ", characterCollisionInfo.headAngle, "\n\n",
                "Triggers : \n",
                "──────────────────\n",
                "Current : ", CurrentTrigger.gameObject != null ? CurrentTrigger.gameObject.name : nullString, '\n',
                triggerString
            );
        }

        #endregion

        protected CharacterCollisionInfo characterCollisionInfo = new CharacterCollisionInfo();
        CharacterCollisions characterCollisions = new CharacterCollisions();


        List<Contact> wallContacts = new List<Contact>();
        List<Contact> headContacts = new List<Contact>();
        // Contacts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets a list with all the current contacts.
        /// </summary>
        public List<Contact> Contacts
        {
            get
            {
                if (PhysicsComponent == null)
                    return null;

                return PhysicsComponent.Contacts;
            }
        }



        // Triggers ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Gets the most recent trigger.
        /// </summary>
        public Trigger CurrentTrigger
        {
            get
            {
                if (PhysicsComponent.Triggers.Count == 0)
                    return new Trigger();   // "Null trigger"

                return PhysicsComponent.Triggers[PhysicsComponent.Triggers.Count - 1];
            }
        }

        /// <summary>
        /// Gets a list with all the triggers.
        /// </summary>
        public List<Trigger> Triggers
        {
            get
            {
                return PhysicsComponent.Triggers;
            }
        }

        // =================================================================================
        void SetColliderSize()
        {

            float verticalOffset = IsStable ? Mathf.Max(StepOffset, CharacterConstants.ColliderMinBottomOffset) : CharacterConstants.ColliderMinBottomOffset;

            float radius = BodySize.x / 2f;
            float height = BodySize.y - verticalOffset;

            ColliderComponent.Size = new Vector2(2f * radius, height);
            ColliderComponent.Offset = Vector2.up * (verticalOffset + height / 2f);
        }
        void GetContactsInformation()
        {
            bool wasCollidingWithWall = characterCollisionInfo.wallCollision;
            bool wasCollidingWithHead = characterCollisionInfo.headCollision;

            wallContacts.Clear();
            headContacts.Clear();

            for (int i = 0; i < Contacts.Count; i++)
            {
                Contact contact = Contacts[i];

                float verticalAngle = Vector3.Angle(Up, contact.normal);

                // Get the wall information -------------------------------------------------------------
                if (CustomUtilities.isCloseTo(verticalAngle, 90f, CharacterConstants.WallContactAngleTolerance))
                    wallContacts.Add(contact);


                // Get the head information -----------------------------------------------------------------
                if (verticalAngle >= CharacterConstants.MinHeadContactAngle)
                    headContacts.Add(contact);


            }


            if (wallContacts.Count == 0)
            {
                characterCollisionInfo.ResetWallInfo();
            }
            else
            {
                Contact wallContact = wallContacts[0];

                characterCollisionInfo.wallCollision = true;
                characterCollisionInfo.wallAngle = Vector3.Angle(Up, wallContact.normal);
                characterCollisionInfo.wallContact = wallContact;

                if (!wasCollidingWithWall)
                {
                    if (OnWallHit != null)
                        OnWallHit(wallContact);
                }

            }


            if (headContacts.Count == 0)
            {
                characterCollisionInfo.ResetHeadInfo();
            }
            else
            {
                Contact headContact = headContacts[0];

                characterCollisionInfo.headCollision = true;
                characterCollisionInfo.headAngle = Vector3.Angle(Up, headContact.normal);
                characterCollisionInfo.headContact = headContact;

                if (!wasCollidingWithHead)
                {
                    if (OnHeadHit != null)
                        OnHeadHit(headContact);
                }
            }


        }
        // =================================================================================

        protected void InitCollisionInfo(CharacterActor Actor, PhysicsComponent PhysicsCompoenent)
        {
            characterCollisions.Initialize(Actor, PhysicsCompoenent);
        }

        protected virtual void ApplyWeight(Vector3 contactPoint)
        {
            if (!applyWeightToGround)
                return;

            {
                if (GroundCollider3D == null)
                    return;

                if (GroundCollider3D.attachedRigidbody == null)
                    return;

                GroundCollider3D.attachedRigidbody.AddForceAtPosition(-transform.up * CharacterBody.Mass * weightGravity, contactPoint);
            }
        }
    }
}