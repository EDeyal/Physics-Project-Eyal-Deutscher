using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EyalRigidbody2D : MonoBehaviour
{
    [SerializeField] float _mass;
    [SerializeField] float _drag;
    [SerializeField] bool _hasGravity;
    [SerializeField] Vector2 _gravityForce;

    [Header("Borders")]
    [SerializeField]float _yTop;
    [SerializeField]float _yBottom;
    [SerializeField]float _xRight;
    [SerializeField]float _xLeft;
    [Header("Do not Change, read only fields")]
    [SerializeField] Vector2 _accelerationForce;
    [SerializeField] Vector2 _velocity;
    [SerializeField]List<Vector2> _activeForces;
    [SerializeField] List<Vector2> _nextActiveForces;

    private void Awake()
    {
        if (_mass == 0)
            throw new System.Exception($"Mass of Object{this.gameObject.name} is 0");
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
            _accelerationForce += _gravityForce * _mass *Time.fixedDeltaTime;
        }
        _velocity += _accelerationForce*Time.fixedDeltaTime;
    }
    private void CalculateAccelerationForce()
    {
        foreach (var force in _activeForces)
        {
            _accelerationForce += force;
        }
    }
    private void ReduceForcesEffects()
    {
        for (int i = 0; i < _activeForces.Count; i++)
        {
            float x = _activeForces[i].x;
            if (x > 0)
            {
                x -= _drag * Time.fixedDeltaTime;
                if (x < 0)
                    x = 0;
            }
            else if (x < 0)
            {
                x += _drag * Time.fixedDeltaTime;
                if (x > 0)
                    x = 0;
            }
            float y = _activeForces[i].y;
            if (y > 0)
            {
                y -= _drag * Time.fixedDeltaTime;
                if (y < 0)
                    y = 0;
            }
            else if (y < 0)
            {
                y += _drag * Time.fixedDeltaTime;
                if (y > 0)
                    y = 0;
            }
            Vector2 newVector = new Vector2(x, y);
            _activeForces[i] = newVector;
            if (newVector == Vector2.zero)
            {
                continue;
            }
            _nextActiveForces.Add(_activeForces[i]);
        }
        TransferNextForcesToActiveForces();
    }
    private void TransferNextForcesToActiveForces()
    {
        _activeForces.Clear();
        foreach (var force in _nextActiveForces)
        {
            _activeForces.Add(force);
        }
        _nextActiveForces.Clear();
    }
    private void CheckBorders()
    {

        if (this.transform.position.y >= _yTop)
        {
            _velocity *= new Vector2(1, -1);
            transform.position = new Vector2(transform.position.x, _yTop);
        }
        if (this.transform.position.y <= _yBottom)
        {
            _velocity *= new Vector2(1, -1);
            transform.position = new Vector2(transform.position.x, _yBottom);
        }
        if (this.transform.position.x >= _xRight)
        {
            _velocity *= new Vector2(-1, 1);
            transform.position = new Vector2(_xRight, transform.position.y);

        }
        if (this.transform.position.x <= _xLeft)
        {
            _velocity *= new Vector2(-1, 1);
            transform.position = new Vector2(_xLeft, transform.position.y);
        }
    }
    public void FixedUpdate()
    {
        CalculateAccelerationForce();
        CalculateVelocity();
        transform.Translate(_velocity.x * Time.fixedDeltaTime / _mass, _velocity.y * Time.fixedDeltaTime / _mass, 0);
        ReduceForcesEffects();
        CheckBorders();
    }
}
