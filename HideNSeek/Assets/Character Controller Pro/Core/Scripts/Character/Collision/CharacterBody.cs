using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

    public enum CharacterBodyType
    {
        Sphere,
        Capsule
    }

    [AddComponentMenu("Character Controller Pro/Core/Character Body")]
    public class CharacterBody : MonoBehaviour
    {
        [SerializeField]
        Vector2 bodySize = new Vector2(1f, 2f);

        [SerializeField]
        float mass = 50f;
        public RigidbodyComponent RigidbodyComponent { get; private set; }

        public ColliderComponent ColliderComponent { get; private set; }

        public float Mass => mass;

        public Vector2 BodySize => bodySize;

        void Awake()
        {

            ColliderComponent = gameObject.AddComponent<CapsuleColliderComponent3D>();
            RigidbodyComponent = gameObject.AddComponent<RigidbodyComponent3D>();
        }

    }

}
