using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

public struct HitInfoFilter
{
    public LayerMask collisionLayerMask;
    public bool ignoreRigidbodies;
    public bool ignoreTriggers;
    public float minimumDistance;
    public float maximumDistance;

    public HitInfoFilter( LayerMask collisionLayerMask, bool ignoreRigidbodies, bool ignoreTriggers , float minimumDistance = 0f , float maximumDistance = Mathf.Infinity )
    {
        this.collisionLayerMask = collisionLayerMask;
        this.ignoreRigidbodies = ignoreRigidbodies;
        this.ignoreTriggers = ignoreTriggers;
        this.minimumDistance = minimumDistance;
        this.maximumDistance = maximumDistance;
    }

        

}

}
