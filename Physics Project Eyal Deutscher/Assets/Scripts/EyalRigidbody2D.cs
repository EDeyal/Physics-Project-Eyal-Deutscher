using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyalRigidbody2D : MonoBehaviour
{
    [SerializeField] float _mass;
    [SerializeField] float _friction;
    [SerializeField] bool _hasGravity;
    [SerializeField] Vector2 _gravityForce;
    [Header("Do not Change, read only fields")]
    [SerializeField] float _acceleration;
    [SerializeField] Vector2 _currentMovementForce;
    [SerializeField]List<Vector2> _activeForces;
    [SerializeField] List<Vector2> _nextActiveForces;

    private void AddForce(Vector2 force)
    {
        _activeForces.Add(force);
    }
    private void ResetForces()
    {
        _activeForces.Clear();
    }
    private void CalculateCurreMovementForce()
    {
        _currentMovementForce = Vector2.zero;
        foreach (var vector in _activeForces)
        {
            _currentMovementForce += vector;
        }
        if (_hasGravity)
        {
            _currentMovementForce += _gravityForce;
        }
    }
    private void ReduceForcesEffects()
    {
        for (int i = 0; i < _activeForces.Count; i++)
        {
            float x = _activeForces[i].x;
            if (x > 0)
            {
                x -= _friction * Time.fixedDeltaTime;
                if (x < 0)
                    x = 0;
            }
            else if (x < 0)
            {
                x += _friction * Time.fixedDeltaTime;
                if (x > 0)
                    x = 0;
            }
            float y = _activeForces[i].y;
            if (y > 0)
            {
                y -= _friction * Time.fixedDeltaTime;
                if (y < 0)
                    y = 0;
            }
            else if (y < 0)
            {
                y += _friction * Time.fixedDeltaTime;
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
        _activeForces = _nextActiveForces;
        _nextActiveForces.Clear();
    }
    public void FixedUpdate()
    {
        CalculateCurreMovementForce();
        transform.Translate(_currentMovementForce.x * Time.fixedDeltaTime, _currentMovementForce.y * Time.fixedDeltaTime, 0);
        ReduceForcesEffects();
    }
}
