using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    /// <summary>
    /// This component is responsible for smoothing out the graphics-related elements (under the root object) based on the character movement (CharacterActor).
    /// It allows you to modify the position and rotation accordingly, producing a great end result.
    /// </summary>
    [AddComponentMenu("Character Controller Pro/Core/Character Graphics/Root Controller")]
    [DefaultExecutionOrder(ExecutionOrder.CharacterGraphicsOrder)]
    public class CharacterGraphicsRootController : CharacterGraphics
    {
        [Header("Rotation interpolation")]

        [Tooltip("Whether or not to smooth out the rotation change.")]
        [SerializeField]
        bool lerpRotation = true;

        [SerializeField]
        float rotationLerpSpeed = 25f;

        [Header("Vertical displacement interpolation")]

        [Tooltip("Whether or not to smooth out the vertical position change (vertical displacement). A vertical displacement happens everytime the character " +
        "increase/decrease its vertical position (slopes, step up, step down, etc.).")]
        [SerializeField]
        bool lerpVerticalDisplacement = true;

        [Tooltip("How fast the step up action is going to be.")]
        [SerializeField]
        float positiveDisplacementSpeed = 20f;

        [Tooltip("How fast the step down action is going to be.")]
        [SerializeField]
        float negativeDisplacementSpeed = 40f;

        // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

        Vector3 previousPosition = default(Vector3);
        Quaternion previousRotation = default(Quaternion);

        void Start()
        {
            previousPosition = transform.position;
            previousRotation = transform.rotation;
        }


        void OnEnable()
        {
            CharacterActor.OnTeleport += OnTeleport;
        }

        void OnDisable()
        {
            CharacterActor.OnTeleport -= OnTeleport;
        }


        void Update()
        {
            if (CharacterActor == null)
            {
                this.enabled = false;
                return;
            }

            float dt = Time.deltaTime;

            HandleRotation(dt);
            HandleVerticalDisplacement(dt);

        }

        void HandleVerticalDisplacement(float dt)
        {
            if (!lerpVerticalDisplacement)
                return;

            Vector3 planarDisplacement = Vector3.ProjectOnPlane(CharacterActor.transform.position - previousPosition, CharacterActor.Up);
            Vector3 verticalDisplacement = Vector3.Project(CharacterActor.transform.position - previousPosition, CharacterActor.Up);

            float lerpT = (CharacterActor.transform.InverseTransformVectorUnscaled(verticalDisplacement).y > 0f ? positiveDisplacementSpeed : negativeDisplacementSpeed) * dt;

            // Do not interpolate the position if the actor is affected by a rigidbody 
            if (CharacterActor.IsStable && CharacterActor.IsGroundARigidbody)
                lerpT = 1f;

            // Do not interpolate the position if the actor is unstable
            if (!CharacterActor.IsStable)
                lerpT = 1f;

            transform.position = previousPosition + planarDisplacement + Vector3.Lerp(Vector3.zero, verticalDisplacement, lerpT);

            previousPosition = transform.position;            
        }

        void HandleRotation(float dt)
        {
            if (!lerpRotation)
                return;

            transform.rotation = Quaternion.Slerp(previousRotation, CharacterActor.Rotation, rotationLerpSpeed * dt);

            previousRotation = transform.rotation;
        }

        void OnTeleport(Vector3 position, Quaternion rotation)
        {
            previousPosition = position;
            previousRotation = rotation;
        }



    }

}

