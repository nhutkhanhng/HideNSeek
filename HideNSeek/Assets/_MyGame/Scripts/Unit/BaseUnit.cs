using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Assertions;
using System;
public class BaseUnit
{
    private static int m_iNextValidID;
    private void SetID(int val)
    {
        UnityEngine.Assertions.Assert.IsTrue(val >= m_iNextValidID);

        m_ID = val;

        m_iNextValidID = m_ID + 1;
    }



    private int m_ID;
    // every entity has a type associated with it (health, troll, ammo, etc..)
    private int m_iType;
    bool m_bTag;

    //its location in the environment
    protected Vector3 m_vPosition;

    protected Vector3 m_vScale;

    //the magnitude of this object's bounding radius
    protected double m_dBoundingRadius;


    public BaseUnit(int ID) { SetID(ID); }


    public virtual void Update() { }

    // public virtual bool HandleMessage(const Telegram& msg){return false;}

    //entities should be able to read/write their data to a stream
    // public virtual void Write(std::ostream&  os)const{}
    // public virtual void Read(std::ifstream& is) { }

    //use this to grab the next valid ID
    public static int GetNextValidID() { return m_iNextValidID; }

    //this can be used to reset the next ID
    public static void ResetNextValidID() { m_iNextValidID = 0; }

    public Vector3 Pos() { return m_vPosition; }
    public void SetPos(Vector3 new_pos) { m_vPosition = new_pos; }

    public double BRadius() { return m_dBoundingRadius; }
    public void SetBRadius(double r) { m_dBoundingRadius = r; }
    public int ID() { return m_ID; }

    public bool IsTagged() { return m_bTag; }
    void Tag() { m_bTag = true; }
    void UnTag() { m_bTag = false; }

    Vector3 Scale() { return m_vScale; }
    void SetScale(Vector3 val) { m_dBoundingRadius *= Mathf.Max(val.x, val.z) / Mathf.Max(m_vScale.x, m_vScale.z); m_vScale = val; }
    void SetScale(float val) { m_dBoundingRadius *= (val / Mathf.Max(m_vScale.x, m_vScale.z)); m_vScale = new Vector3(val, val, val); }

    public int EntityType() { return m_iType; }
    void SetEntityType(int new_type) { m_iType = new_type; }
}
