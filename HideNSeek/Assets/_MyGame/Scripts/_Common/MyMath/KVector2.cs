using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct KVector2
{
    Vector2 _Position;

    public static implicit operator KVector2(Vector2 flat) => new KVector2() { _Position = flat };
    public static implicit operator Vector2(KVector2 flat) => flat._Position;

    public static implicit operator KVector2(Vector3 flat) => new KVector2() { _Position = new Vector2(flat.x, flat.z) };
    public static implicit operator Vector3(KVector2 flat) => new Vector3(flat._Position.x, 0, flat._Position.y);

    public float x { get { return _Position.x; } set { _Position.x = value; } }
    public float y { get { return _Position.y; } set { _Position.y = value; } }
}

