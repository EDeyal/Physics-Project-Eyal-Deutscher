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
struct ResolveCollisionData
{
    public ResolveCollisionData(EyalCollider mainCollider,EyalCollider otherCollider,Vector2 resolveVector)
    {
        MainCollider = mainCollider;
        OtherCollider = otherCollider;
        ResolveVector = resolveVector;
    }
    public EyalCollider MainCollider;
    public EyalCollider OtherCollider;
    public Vector2 ResolveVector;
}
public class CollisionManager : MonoSingleton<CollisionManager>
{
    private List<EyalCollider> _colliders;
    private List<ResolveCollisionData> _collisionsData;

    public override void Awake()
    {
        base.Awake();
        _colliders = new List<EyalCollider>();
        _collisionsData = new List<ResolveCollisionData>();
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
        ResolveCollisionData();
    }
    private void DetectCollisions()
    {
        EyalCollider collider1;
        EyalCollider collider2;
        bool triggerOverlap = false;
        bool hasCollided = true;

        //AABB collision detection on all colliders
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
                        //add to collision data in order to resolve later
                        AssignCollisionsData(collider1, collider2, triggerOverlap);
                    }
                }
            }
        }
    }

    public bool CheckObjectCollision(EyalCollider collider, EyalCollider otherCollider)
    {
        //objects can call for a check if they collided with another object
        bool triggerOverlap = false;
        bool hasCollided = true;

        if (AABBCollision(collider, otherCollider, out hasCollided, out triggerOverlap))
        {
            if (hasCollided && !triggerOverlap)
            {
                return true;
            }
        }
        return false;
    }
    private bool AABBCollision(EyalCollider collider1, EyalCollider collider2, out bool hasCollided, out bool triggerOverlap)
    {
        //The focus is on the collider 1 because every collider will have it's check as collider 1
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
        collider1.OnEyalCollisionEnter(collider2);
        //Debug.Log("Collision Accuring");

        //create collision data on collider to know the side that was hit
        collider1.CollisionResolveDirection = GetResolveDirectionType(collider1Bounds, collider2Bounds);

        if (collider1.IsTrigger || collider2.IsTrigger)
        {
            //Debug.Log("overlapping");
            collider1.OnEyalTriggerEnter(collider2);
            collider2.OnEyalTriggerEnter(collider1);
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
    private void AssignCollisionsData(EyalCollider collider1, EyalCollider collider2, bool isTriggerCollision)
    {
        //Vector2 collisionNormal = CalculateCollisionNormal(collider1.transform.position, collider2.transform.position);//will work only for the middle area of the object
        if (isTriggerCollision)
        {
            if (collider1.IsTrigger ||collider2.IsTrigger)
            {
                //Debug.Log("Overlap Collision");
                //collider1.OnEyalTriggerEnter(collider2);
                return;
            }
        }
        else
        {
            Vector2 resolveVector = Vector2.zero;
            if (collider2.Rigidbody) //2 bodies collide
            {
                float mass1 = collider1.Rigidbody.Mass;
                float mass2 = collider2.Rigidbody.Mass;

                //v1' = (2m2/m1+m2)*v2 + ((m1-m2)/(m1+m2)*v1)
                resolveVector = (2 * mass2 / (mass1 + mass2)) * collider2.Rigidbody.Velocity;
                resolveVector += (mass1 - mass2) / (mass1 + mass2) * collider1.Rigidbody.Velocity;

                //resolveVector += collider2.Rigidbody.Velocity;
            }
            else //hit a wall
            {
                resolveVector = collider1.Rigidbody.Velocity;
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
            }



            _collisionsData.Add(new ResolveCollisionData(collider1, collider2, resolveVector));
            //Debug.Log("Collision");
        }
    }
    private void ResolveCollisionData()
    {
        foreach (var data in _collisionsData)
        {
            TryResolveCollision(data.MainCollider, data.OtherCollider, data.ResolveVector);
        }
        _collisionsData.Clear();
    }
    private void TryResolveCollision(EyalCollider collider, EyalCollider otherCollider, Vector2 resolveVector)
    {
        if (collider.Rigidbody)
        {
            if (collider.Rigidbody.IsMoveable)
            {
                if (!collider.Rigidbody.IsResolvingCollision(otherCollider))
                {
                    if (collider.Rigidbody != null)
                    {
                        collider.Rigidbody.AddCollisionData(new CollisionData(otherCollider, collider.CollisionResolveDirection));
                    }
                    Debug.Log(collider.Rigidbody.gameObject.name + ": is colliding");
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
        //Debug.Log("Velocity " + collider.Rigidbody.Velocity);
        collider.Rigidbody.BounceRigidbody(resolveVector * BounceStrength);
    }

}
