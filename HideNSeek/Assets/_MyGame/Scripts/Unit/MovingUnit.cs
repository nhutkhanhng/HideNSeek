using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingUnit : BaseUnit
{
    public MovingUnit(int ID) : base(ID)
    {
    }

    protected Vector3 m_vVelocity;

    protected Vector3 m_vSide;
    // a vector3 perpendicular to the heading vector3
    public Vector3 vSide
    {
        get { return Vector3.Cross(m_vHeading, Vector3.up); }
        protected set
        {
            m_vSide = value;
        }
    }

    // a normalized vector pointing in the direction the entity is heading
    protected Vector3 m_vHeading;


    // ====
    float m_fMass;
    float m_fMaxSpeed;
    // the maximun force this entity can produce to power itself ( like rockets and thrust);
    float m_fMaxForce;
    // the maximun rate (radians per second) this vehicle can rotate;
    float m_fMaxTurnRate;


    MovingUnit(Vector3 position,
                 float radius,
                 Vector3 velocity,
                 float max_speed,
                 Vector3 heading,
                 float mass,
                 Vector3 scale,
                 float turn_rate,
                 float max_force) : base(GetNextValidID())
    {
        m_vHeading = heading;
        m_vVelocity = velocity;
        m_fMass = mass;
        m_fMaxSpeed = max_speed;
        m_fMaxTurnRate = turn_rate;
        m_fMaxForce = max_force;
        m_vPosition = position;
        m_dBoundingRadius = radius;
        m_vScale = scale;
    }

    //accessors
    public Vector3 Velocity() { return m_vVelocity; }
    public void SetVelocity(in Vector3 NewVel) { m_vVelocity = NewVel; }

    public float Mass() { return m_fMass; }

    public Vector3 Side() { return m_vSide; }

    public float MaxSpeed() { return m_fMaxSpeed; }
    public void SetMaxSpeed(float new_speed) { m_fMaxSpeed = new_speed; }

    public float MaxForce() { return m_fMaxForce; }
    public void SetMaxForce(float mf) { m_fMaxForce = mf; }

    public bool IsSpeedMaxedOut() { return m_fMaxSpeed * m_fMaxSpeed >= m_vVelocity.sqrMagnitude; }
    public float Speed() { return m_vVelocity.magnitude; }
    public float SpeedSq() { return m_vVelocity.sqrMagnitude; }

    public Vector3 Heading() { return m_vHeading; }
    public void SetHeading(Vector3 new_heading)
    {
        UnityEngine.Assertions.Assert.IsTrue(new_heading.sqrMagnitude - 1 < Mathf.Epsilon);

        m_vHeading = new_heading;
    }
    public bool RotateHeadingToFacePosition(Vector3 target)
    {
        Vector3 toTarget = (target - m_vPosition);

        float dot = Vector3.Dot(m_vHeading, toTarget);

        //some compilers lose acurracy so the value is clamped to ensure it
        //remains valid for the acos
        dot = Mathf.Clamp(dot, -1, 1);

        //first determine the angle between the heading vector and the target
        double angle = Mathf.Acos(dot);

        //return true if the player is facing the target
        if (angle < 0.00001) return true;

        //clamp the amount to turn to the max turn rate
        if (angle > m_fMaxTurnRate) angle = m_fMaxTurnRate;

        //The next few lines use a rotation matrix to rotate the player's heading
        //vector accordingly
        // Quaternion RotationMatrix;

        //notice how the direction of rotation has to be determined when creating
        //the rotation matrix
        //RotationMatrix.Rotate(angle * Vector3.SignedAngle(m_vHeading, toTarget));
        //RotationMatrix.TransformVector2Ds(m_vHeading);
        //RotationMatrix.TransformVector2Ds(m_vVelocity);

        //finally recreate m_vSide
        m_vSide = Vector3.Cross(m_vHeading, Vector3.up);

        return false;
    }

    public float MaxTurnRate() { return m_fMaxTurnRate; }
    void SetMaxTurnRate(float val) { m_fMaxTurnRate = val; }
}
