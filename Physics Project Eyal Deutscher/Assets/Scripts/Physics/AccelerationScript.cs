using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationScript : MonoBehaviour
{
    enum movementType
    {
        Velocity = 0,
        Acceleration = 1,
        ChangingAcceleration = 2,
    }
    [SerializeField] movementType _movementType;
    [SerializeField] float _speed;
    [SerializeField] float _currentAcceleration;
    [SerializeField] float _currentChangingAcceleration;
    private void Start()
    {
        _currentAcceleration = _speed;
        _currentChangingAcceleration = _speed;
    }
    private void FixedUpdate()
    {
        Vector3 moveDeltaAmount;
        switch (_movementType)
        {
            case movementType.Velocity:
                moveDeltaAmount = new Vector3(_speed*Time.fixedDeltaTime,0,0);
                
            this.transform.Translate(moveDeltaAmount);
                break;


            case movementType.Acceleration:
                moveDeltaAmount = new Vector3(_currentAcceleration* Time.fixedDeltaTime,0,0);
                transform.Translate(moveDeltaAmount);
                _currentAcceleration += _speed;

                break;
            case movementType.ChangingAcceleration:
                moveDeltaAmount = new Vector3(_currentChangingAcceleration * Time.fixedDeltaTime,0,0);
                transform.Translate(moveDeltaAmount);
                if (transform.position.x > 8)
                {
                    _currentChangingAcceleration -= (_speed);
                }
                else if (transform.position.x < 3)
                {
                    _currentChangingAcceleration += (_speed);
                }
                break;
            default:
                break;
        }
    }

}
