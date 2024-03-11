using System.Collections.Generic;
using UnityEngine;

public enum CollisionDirectionType
{
    None = 0,
    RightCollision = 1,
    LeftCollision = 2,
    TopCollision = 3,
    BottomCollision = 4
}
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
                if (AABBCollision(collider1, collider2, out hasCollided, out triggerOverlap))
                {
                    if (hasCollided)
                    {
                        ResolveCollisions(collider1, collider2, triggerOverlap);
                    }
                }
            }
        }
    }

    public bool CheckObjectCollision(EyalCollider colliderToCheck)
    {
        bool triggerOverlap = false;
        bool hasCollided = true;
        foreach (var otherCollider in _colliders)
        {
            if (colliderToCheck != otherCollider)
            {
                if (AABBCollision(colliderToCheck, otherCollider, out hasCollided, out triggerOverlap))
                {
                    if (hasCollided && !triggerOverlap)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private bool AABBCollision(EyalCollider collider1, EyalCollider collider2, out bool hasCollided, out bool triggerOverlap)
    {
        triggerOverlap = false;
        hasCollided = false;

        if (collider1.Rigidbody == null && collider2.Rigidbody == null)
        {
            //no rigidbodies are involved
            return false;
        }
        else if (collider1.Rigidbody == null)
        {
            //main object of collision has no rigidbody
            return false;
        }


        //get the bounds of both objects
        Bounds collider1Bounds = collider1.Bounds;
        Bounds collider2Bounds = collider2.Bounds;

        //check for overlap
        bool overlapX = collider1Bounds.min.x <= collider2Bounds.max.x && collider1Bounds.max.x >= collider2Bounds.min.x;
        bool overlapY = collider1Bounds.min.y <= collider2Bounds.max.y && collider1Bounds.max.y >= collider2Bounds.min.y;

        //check if there is overlap in both axes
        bool isOverlapping = overlapX && overlapY;

        //if both colliders are triggers they can not colliider, check only for overlap.
        if (collider1.IsTrigger && collider2.IsTrigger)
        {
            //should not collide with 2 triggers
            return false;
        }
        else if (!isOverlapping)
        {
            return false;
        }
        //We have collision
        hasCollided = true;
        //Debug.Log("Collision Accuring");

        collider1.CollisionResolveDirection = GetResolveDirectionType(collider1Bounds, collider2Bounds);

        if (collider1.IsTrigger || collider2.IsTrigger)
        {
            triggerOverlap = true;
        }

        return isOverlapping;
    }
    private CollisionDirectionType GetResolveDirectionType(Bounds collider1Bounds, Bounds collider2Bounds)
    {
        CollisionDirectionType directionResolveType = CollisionDirectionType.None;

        //get top and bottom bounds and check distance between them
        float colliderTopDistance = Mathf.Abs(collider1Bounds.max.y - collider2Bounds.min.y);

        //get bottom and top bounds and check distance between them
        float colliderBottomDistance = Mathf.Abs(collider1Bounds.min.y - collider2Bounds.max.y);

        //get right and left bounds and check distance between them
        float colliderRightDistance = Mathf.Abs(collider1Bounds.max.x - collider2Bounds.min.x);

        //get left and right bounds and check distance between them
        float colliderLeftDistance = Mathf.Abs(collider1Bounds.min.x - collider2Bounds.max.x);



        //shortest distance is the collision directionType
        float shortestDistance = colliderTopDistance;

        if (colliderBottomDistance < shortestDistance)
            shortestDistance = colliderBottomDistance;

        if (colliderRightDistance < shortestDistance)
            shortestDistance = colliderRightDistance;

        if (colliderLeftDistance < shortestDistance)
            shortestDistance = colliderLeftDistance;

        //shortest distance should not be equal to other distance if calculation is right on Boxes objects
        if (shortestDistance == colliderTopDistance)
        {
            //top bound hit
            directionResolveType = CollisionDirectionType.TopCollision;
        }
        else if (shortestDistance == colliderBottomDistance)
        {
            //bottom bound hit
            directionResolveType = CollisionDirectionType.BottomCollision;
        }
        else if (shortestDistance == colliderRightDistance)
        {
            //right bound hit
            directionResolveType = CollisionDirectionType.RightCollision;
        }
        else if (shortestDistance == colliderLeftDistance)
        {
            //left bound hit
            directionResolveType = CollisionDirectionType.LeftCollision;
        }
        else
        {
            Debug.LogError("Direction Type is wierd check it");
        }
        return directionResolveType;
    }
    private void ResolveCollisions(EyalCollider collider1, EyalCollider collider2, bool isTriggerCollision)
    {
        //Vector2 collisionNormal = CalculateCollisionNormal(collider1.transform.position, collider2.transform.position);//will work only for the middle area of the object
        if (isTriggerCollision)
        {
            Debug.Log("Overlap Collision");
            if (collider1.IsTrigger)
            {
                collider1.OnEyalTriggerEnter(collider2);
            }
            if (collider2.IsTrigger)
            {
                collider2.OnEyalTriggerEnter(collider1);
            }

        }
        else
        {
            collider2.OnEyalCollisionEnter(collider1);
            Vector2 resolveVector = collider1.Rigidbody.Velocity;

            switch (collider1.CollisionResolveDirection)
            {
                case CollisionDirectionType.None:
                    //probably an error
                    //Debug.LogError("Error?");
                    break;
                case CollisionDirectionType.RightCollision:
                    //Debug.Log("right collision");
                    //revert x axis
                    resolveVector.x *= -1;
                    break;
                case CollisionDirectionType.LeftCollision:
                    //Debug.Log("left collision");
                    //revert x axis
                    resolveVector.x *= -1;
                    break;
                case CollisionDirectionType.TopCollision:
                    //Debug.Log("top collision");
                    //revert y axis
                    resolveVector.y *= -1;
                    break;
                case CollisionDirectionType.BottomCollision:
                    //Debug.Log("bottom collision");
                    //revert y axis
                    resolveVector.y *= -1;
                    break;
                default:
                    break;
            }
            TryResolveCollision(collider1, resolveVector, collider2);
            //Debug.Log("Collision");
        }
    }
    private void TryResolveCollision(EyalCollider collider, Vector2 resolveVector, EyalCollider otherCollider)
    {
        if (collider.Rigidbody)
        {
            if (collider.Rigidbody.IsMoveable)
            {
                if (!collider.Rigidbody.IsResolvingCollision)
                {
                    collider.OnEyalCollisionEnter(otherCollider);
                    Bounce(collider, resolveVector);
                    //Debug.Log("Collision Normal is: " + collisionNormal);
                }
            }
        }
    }
    private void Bounce(EyalCollider collider, Vector2 resolveVector)
    {
        float BounceStrength = Mathf.Abs(collider.Rigidbody.Bounceiness / collider.Rigidbody.Mass);//bounciness equasion, basically redirects the velocity to the new normalized vector
        Debug.Log("Velocity " + collider.Rigidbody.Velocity);
        collider.Rigidbody.StopRigidbody();
        Vector2 forceToAdd = resolveVector * BounceStrength /Time.fixedDeltaTime;//multiply force by fixed delta time
        Debug.Log("Added Force " + forceToAdd);
        collider.Rigidbody.AddForce(forceToAdd);
    }

}
