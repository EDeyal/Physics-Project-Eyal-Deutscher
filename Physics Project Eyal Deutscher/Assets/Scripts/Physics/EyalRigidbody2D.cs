using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EyalCollider))]
public class EyalRigidbody2D : MonoBehaviour
{
    #region Fields
    static float _dragOffset = 0.01f;
    [SerializeField] float _mass;
    [SerializeField,Range(0,0.1f)] float _drag;
    [SerializeField] float _bounciness;
    [SerializeField] bool _hasGravity;
    [SerializeField] bool _isMoveable;
    [SerializeField] bool _isGrounded;
    [SerializeField] Vector2 _gravityForce;
    [SerializeField] Vector2 _lastPosition;
    public event Action<Vector2> OnPositionChanged;

    [Space,Header("Rope")]
    [SerializeField] bool _isAttachedToRope = false;
    [SerializeField] Vector2 _ropeAnchorPos = Vector2.zero;
    [SerializeField] float _ropeLength;
    [SerializeField] float _ropeStiffness;
    [SerializeField,Tooltip("Clamps the magnitude from 0 to this number")] float _maxMagnitudePerFrame = 10;

    [Header("Do not change, read only fields")]
    [SerializeField] Vector2 _totalAccelerationForce;
    [SerializeField] Vector2 _velocity;
    [SerializeField] List<Vector2> _accelerationForces;
    [SerializeField] List<Vector2> _activeForces;
    [SerializeField] List<Vector2> _nextActiveForces;
    [SerializeField] List<CollisionData> _collisionData;
    EyalCollider _collider;
    #endregion

    #region Properties
    public EyalCollider Collider => _collider;
    public float Bounceiness => _bounciness;
    public float Mass => _mass;
    public Vector2 Velocity => _velocity;
    public bool IsMoveable => _isMoveable;
    public bool IsGrounded { get => _isGrounded; set => _isGrounded = value; }
    public List<CollisionData> CollisionsData => _collisionData;
    public bool IsAttachedToRope => _isAttachedToRope;
    public Vector3 RopeAnchorPoint => _ropeAnchorPos;
    #endregion

    private void Awake()
    {
        if (_mass == 0)
            throw new System.Exception($"Mass of Object{this.gameObject.name} is 0");

        _collider = gameObject.GetComponent<EyalCollider>();
        if(_collider == null)
            throw new System.Exception($"Rigidbody {gameObject.name} has no collider"); //rigidbody must have a collider
        _collider.SetRigidbody(this);
    }


    private void Start()
    {
        _lastPosition = transform.position;
    }

    public void AddAccelerationForce(Vector2 accelerationForce)
    { 
        _accelerationForces.Add(accelerationForce);
    }
    public void RemoveAccelerationForce(Vector2 accelerationForce)
    { 
        _accelerationForces.Remove(accelerationForce);
    }
    public void CalculateCurrentAccelerationForceAddition()
    {
        _totalAccelerationForce = Vector2.zero;

        if (_hasGravity)
        {
            if(!_accelerationForces.Contains(_gravityForce * Mass)) //checks if it exists can cause a bug where another force of the same magnitude will prevent this force from adding
                AddAccelerationForce(_gravityForce * Mass);

            if (_isGrounded)
            {
                //if does not have the opposite from gravity add counter for the gravity
                if (!_accelerationForces.Contains(-_gravityForce*Mass))
                    AddAccelerationForce(-_gravityForce*Mass);
            }
            else
            {
                //if is not colliding with the ground remove the counter force
                if(_accelerationForces.Contains(-_gravityForce * Mass))
                    RemoveAccelerationForce(-_gravityForce * Mass);
            }
        }

        foreach (var force in _accelerationForces)
        {
            _totalAccelerationForce += force / _mass * Time.fixedDeltaTime;
        }
    }
    public void AddForce(Vector2 force)
    {
        _activeForces.Add(force);
    }

    private void ResetForces()
    {
        _activeForces.Clear();
    }
    private void CalculateVelocity()
    {
        if (_isAttachedToRope)
        {
            Vector2 ropeForce = CalculatioRopeForce();

            AddForce(ropeForce);
        }

        _velocity += CalculateVelocityByForces();
        CalculateCurrentAccelerationForceAddition();
        _velocity += _totalAccelerationForce;
        _velocity = UseDragEffectOnVector(_velocity);
    }
    private Vector2 CalculateVelocityByForces()
    {
        Vector2 velocityAddition = Vector2.zero;
        foreach (var force in _activeForces)
        {
            velocityAddition += force / _mass * Time.fixedDeltaTime;
        }
        _activeForces.Clear();
        return velocityAddition;
    }
    private Vector2 UseDragEffectOnVector(Vector2 vector)
    {
        //reduce vector length by drag;
        vector *= (1-_drag);
        //if the x vector is smaller then the drag delta, make it 0.
        float newX = 0f;
        if (Mathf.Abs(vector.x) > _dragOffset)
        {
            newX = vector.x;
        }
        //if the y vector is smaller then the drag delta, make it 0.
        float newY = 0f;
        if (Mathf.Abs(vector.y) > _dragOffset)
        {
            newY = vector.y;
        }


        vector = new Vector2(newX, newY);//the reduced vector by drag force

        return vector;
    }
    private void CheckMovement()
    {
        var currentPos = (Vector2)transform.position;
        if (_lastPosition != currentPos)
        {
            //let anyone that needs to move a notification
            OnPositionChanged?.Invoke(currentPos);
            _lastPosition = currentPos;
        }
    }
    #region Collisions
    private void TryResolveCollision()
    {
        List<CollisionData> existingCollisions = new List<CollisionData>();
        foreach (var collisionData in _collisionData)
        {
            if (CollisionManager.Instance.CheckObjectCollision(_collider, collisionData.Collider))//check if they still collide
            {
                existingCollisions.Add(collisionData);//check every collision in order to know if you are colliding with another object
            }
        }
        _collisionData = existingCollisions;//updating current collisions

        //check is grounded
        _isGrounded = false;
        foreach (var collisionData in _collisionData)
        { 
            if (collisionData.DirectionType == CollisionDirectionType.BottomCollision)
            {
                _isGrounded = true;
            }
        }
    }
    public void BounceRigidbody(Vector2 resolveVector)
    {
        _velocity = resolveVector;
        ResetForces();
    }
    public bool IsResolvingCollision(EyalCollider otherCollider)
    {
        foreach (var collisionData in _collisionData)
        {
            if (collisionData.Collider == otherCollider)
            {
                return true;
            }
        }
        return false;
    }
    public void AddCollisionData(CollisionData collisionData)
    {
        if (_collisionData.Contains(collisionData))
        {
            return;
        }
        _collisionData.Add(collisionData);
    }
    public void RemoveCollidingCollider(CollisionData collisionData)
    {
        if (!_collisionData.Contains(collisionData))
        {
            return;
        }
        _collisionData.Remove(collisionData);
    }

    #endregion

    #region RopeCalculations
    public void AttachToRope(Vector2 attachmentPoint, float ropeLength, float stiffness)
    { 
        _isAttachedToRope = true;
        _ropeAnchorPos = attachmentPoint;
        _ropeLength = ropeLength;
        _ropeStiffness = stiffness;
    }
    public void DetachFromRope()
    { 
        _isAttachedToRope = false;
        _ropeAnchorPos = Vector2.zero;
        _ropeLength = 0;
        _ropeStiffness = 0;
    }

    private float CalculateTensionMagnitude(float CurrentDistance, float ropeLength, float stiffness)
    {
        float extention = CurrentDistance - ropeLength;

        //ensure that the extention is absolute
        extention = Mathf.Abs(extention);
        //Debug.Log("Extention is: " + extention);
        extention = Mathf.Clamp(extention, 0, _maxMagnitudePerFrame);
        //
        float tensionMagnutude = stiffness * extention;

        return tensionMagnutude;
    }

    private Vector2 CalculatioRopeForce()
    {
        //calculate direction to anchor point
        Vector2 attachmentVector = _ropeAnchorPos - (Vector2)transform.position;

        //calculate distance
        float distanceFromAnchor = attachmentVector.magnitude;

        if (distanceFromAnchor < _ropeLength)
        {
            attachmentVector *= -1;

            //rope is loose and should not effect anything.
            return Vector2.zero;
        }
        float tensionMagnitude = CalculateTensionMagnitude(distanceFromAnchor, _ropeLength, _ropeStiffness);

        attachmentVector.Normalize();


        Vector2 tensionForce = attachmentVector * tensionMagnitude;

        return tensionForce;
    }
    #endregion

    public void FixedUpdate()
    {
        //calculate forces
        CalculateVelocity();

        //move this object
        if (_isGrounded && _velocity.y < 0)
        {
            transform.Translate(_velocity.x * Time.fixedDeltaTime, 0, 0);
        }
        else
        { 
            transform.Translate(_velocity.x * Time.fixedDeltaTime, _velocity.y * Time.fixedDeltaTime, 0);
        }
        //check if this object moved this frame
        CheckMovement();
        //solve collisions if any exists
        if (_collisionData.Count>0)
        {
            TryResolveCollision();
        }
    }
}
