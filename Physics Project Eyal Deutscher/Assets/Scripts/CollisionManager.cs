using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoSingleton<CollisionManager>
{
    private List<EyalCollider> _colliders;

    public override void Awake()
    {
        base.Awake();
        _colliders = new List<EyalCollider>();
    }
    public void RigisterCollider(EyalCollider collider)
    {
        if (_colliders.Contains(collider))
        {
            throw new System.Exception($"Rigidbody {collider.gameObject.name} is already in rigidbody list");
        }
        _colliders.Add(collider);
    }
    public void UnrigisterCollider(EyalCollider collider)
    {
        _colliders.Remove(collider);
    }

    private void FixedUpdate()
    {
        DetectCollisions();
    }
    private void DetectCollisions()
    {
        EyalCollider collider1;
        EyalCollider collider2;
        bool triggerOverlap = false;
        bool hasCollided = true;

        //AABB collision detection
        for (int i = 0; i < _colliders.Count; i++)
        {
            collider1 = _colliders[i];

            for (int j = 0; j < _colliders.Count; j++)
            {
                collider2 = _colliders[j];
                if (collider1 == collider2)
                {
                    continue;
                }
                if (AABBCollision(collider1, collider2,out hasCollided,out triggerOverlap))
                {
                    if (hasCollided)
                    { 
                        ResolveCollisions(collider1,collider2,triggerOverlap);
                    }
                }
            }
        }
    }
    private bool AABBCollision(EyalCollider collider1, EyalCollider collider2,out bool hasCollided, out bool triggerOverlap)
    {
        triggerOverlap = false;

        if (collider1.Rigidbody == null && collider2.Rigidbody == null)
        {
            //no rigidbodies are involved
            hasCollided = false;
            return false;
        }

        //get if the objects are triggers
        bool isTrigger1 = collider1.IsTrigger;
        bool isTrigger2 = collider2.IsTrigger;

        if (isTrigger1 && isTrigger2)
        {
            //should not collide
            hasCollided = false;
            return false;
        }
        else
        {
            hasCollided = true;
        }

        //get the sizeOfBounds for the objects
        Bounds rb1Bounds = collider1.Bounds;
        Bounds rb2Bounds = collider2.Bounds;

        //check for overlap
        bool overlapX = rb1Bounds.min.x <= rb2Bounds.max.x && rb1Bounds.max.x >= rb2Bounds.min.x;
        bool overlapY = rb1Bounds.min.y <= rb2Bounds.max.y && rb1Bounds.max.y >= rb2Bounds.min.y;

        bool _isOverlaping = overlapX && overlapY;
        //if both are triggers check overlap

        if (_isOverlaping)
        {
            if (isTrigger1 || isTrigger2)
            {
                    triggerOverlap = true;
            }
        }
        return _isOverlaping;
    }
    private void ResolveCollisions(EyalCollider collider1, EyalCollider collider2,bool isTriggerCollision)
    {
        if (isTriggerCollision)
        {
            Debug.Log("Overlap Collision");
        }
        else
        { 
            Debug.Log("Collision");
        }
        //resolve collision logic
    }
}
