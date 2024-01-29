using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceBox : MonoBehaviour
{
    [SerializeField] float _mass;
    [SerializeField] float _acceleration;
    [SerializeField] float _friction;
    [SerializeField] Vector3 _currentMovementForce;

    void Update()
    {
        _currentMovementForce /= _mass;
        transform.Translate(_currentMovementForce.x, _currentMovementForce.y, _currentMovementForce.z);
    }
}
