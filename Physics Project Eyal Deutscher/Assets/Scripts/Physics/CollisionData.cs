using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class CollisionData
{
    public CollisionData(EyalCollider otherCollider,CollisionDirectionType directionType)
    {
        Collider = otherCollider;
        DirectionType = directionType;
    }
    public EyalCollider Collider;
    public CollisionDirectionType DirectionType;
}
