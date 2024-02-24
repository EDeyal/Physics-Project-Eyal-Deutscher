using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EyalCollider))]
public class EyalRigidbody2D : MonoBehaviour
{
    [SerializeField] float _mass;
    [SerializeField] float _drag;
    [SerializeField] bool _hasGravity;
    [SerializeField] Vector2 _gravityForce;
    [SerializeField] Vector2 _lastPosition;
    public event Action<Vector2> OnPositionChanged;

    [Header("Do not Change, read only fields")]
    [SerializeField] Vector2 _accelerationForce;
    [SerializeField] Vector2 _velocity;
    [SerializeField] List<Vector2> _activeForces;
    [SerializeField] List<Vector2> _nextActiveForces;
    EyalCollider _collider;

    public EyalCollider Collider => _collider;

    private void Awake()
    {
        if (_mass == 0)
            throw new System.Exception($"Mass of Object{this.gameObject.name} is 0");

        _collider = gameObject.GetComponent<EyalCollider>();
        if(_collider == null)
            throw new System.Exception($"Rigidbody {gameObject.name} has no collider");
        _collider.SetRigidbody(this);
    }


    private void Start()
    {
        _lastPosition = transform.position;
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
        if (_hasGravity)
        {
            _accelerationForce += _gravityForce * _mass * Time.fixedDeltaTime;
        }
        _velocity += _accelerationForce * Time.fixedDeltaTime;
        _velocity = UseDragEffectOnVector(_velocity);
    }
    private void CalculateAccelerationForce()
    {
        foreach (var force in _activeForces)
        {
            _velocity += force * Time.fixedDeltaTime;
        }
    }
    private Vector2 UseDragEffectOnVector(Vector2 vector)
    {
        //drag for the current frame
        float dragDeltaTime = _drag * Time.fixedDeltaTime;

        //if the x vector is larger then the drag, reduce the vector length, if it is smaller, make it 0.
        float newX = Mathf.Abs(vector.x) > dragDeltaTime ? vector.x - Mathf.Sign(vector.x) * dragDeltaTime : 0f;

        //if the y vector is larger then the drag, reduce the vector length, if it is smaller, make it 0.
        float newY = Mathf.Abs(vector.y) > dragDeltaTime ? vector.y - Mathf.Sign(vector.y) * dragDeltaTime : 0f;

        //return a new vector
        return new Vector2(newX, newY);
    }
    private void ReduceForcesEffects()
    {
        foreach (var force in _activeForces)
        {
            Vector2 newVector = UseDragEffectOnVector(force);
            //if a vector is 0 it does not effect the equasion and need to be removed.s
            if (newVector != Vector2.zero)
            {
                _nextActiveForces.Add(newVector);
            }
        }

        //make a new list in order to swap
        var newList = _activeForces;
        //update the active forces after calculations
        _activeForces = _nextActiveForces;
        //make the next active list point to the new list
        _nextActiveForces = newList;
        //clear the new list
        _nextActiveForces.Clear();
    }
    private void CheckMovement()
    {
        var currentPos = (Vector2)transform.position;
        if (_lastPosition != currentPos)
        {
            //let The Collision Manager to know about this movement
            OnPositionChanged?.Invoke(currentPos);
            _lastPosition = currentPos;
        }
    }
    public void FixedUpdate()
    {
        CalculateAccelerationForce();
        CalculateVelocity();
        transform.Translate(_velocity.x * Time.fixedDeltaTime / _mass, _velocity.y * Time.fixedDeltaTime / _mass, 0);
        ReduceForcesEffects();
        CheckMovement();
    }
}
