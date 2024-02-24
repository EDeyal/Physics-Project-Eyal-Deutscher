using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoSingleton<CollisionManager>
{
    private List<EyalRigidbody2D> _rigidbodies;
    //private List<>

    public override void Awake()
    {
        base.Awake();
        _rigidbodies = new List<EyalRigidbody2D>();
    }
    public void RigisterRigidbody(EyalRigidbody2D rigidbody)
    {
        if (_rigidbodies.Contains(rigidbody))
        {
            throw new System.Exception($"Rigidbody {rigidbody.gameObject.name} is already in rigidbody list");
        }
        _rigidbodies.Add(rigidbody);
    }
    public void UnrigisterRigidbody(EyalRigidbody2D rigidbody)
    {
        _rigidbodies.Remove(rigidbody);
    }

    private void FixedUpdate()
    {
        DetectCollisions();
    }
    private void DetectCollisions()
    {

        EyalRigidbody2D rb1;
        EyalRigidbody2D rb2;
        bool triggerOverlap = false;

        //AABB collision detection
        for (int i = 0; i < _rigidbodies.Count; i++)
        {
            rb1 = _rigidbodies[i];

            for (int j = 0; j +1 < _rigidbodies.Count; j++)
            {
                rb2 = _rigidbodies[j];
                if (AABBCollision(rb1, rb2,out triggerOverlap))
                {
                    ResolveCollisions(rb1,rb2,triggerOverlap);
                }
            }
        }
    }
    private bool AABBCollision(EyalRigidbody2D rb1, EyalRigidbody2D rb2, out bool triggerOverlap)
    {
        triggerOverlap = false;

        //get if the objects are triggers
        bool isTrigger1 = rb1.IsTrigger;
        bool isTrigger2 = rb2.IsTrigger;

        if (isTrigger1 && isTrigger2)
        {
            //should not collide
            return false;
        }

        //get the sizeOfBounds for the objects
        Bounds rb1Bounds = rb1.Bounds;
        Bounds rb2Bounds = rb2.Bounds;

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
    private void ResolveCollisions(EyalRigidbody2D rb1, EyalRigidbody2D Rb2,bool isTriggerCollision)
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
