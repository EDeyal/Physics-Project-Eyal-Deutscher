using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] EyalRigidbody2D _rigidbody2D;
    [SerializeField,Range(1,1000)] float _mouseDragStrengthMultiplication = 1;
    [SerializeField] Camera _camera;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] int _zLocation;
    [Header("Do not Change, read only fields")]
    [SerializeField] Vector3 _startingPoint;
    [SerializeField] Vector3 _endPoint;
    
    bool _isDrawing;
    private void OnValidate()
    {
        _camera = Camera.main;
        _lineRenderer = GetComponent<LineRenderer>();
    }
    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (_rigidbody2D == null)
                return;
            _startingPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Mouse Clicked Down");
            _isDrawing = true;
            _startingPoint.z = _zLocation;
        }
        if (_isDrawing)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, _startingPoint);
            _endPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            _endPoint.z = _zLocation;
            _lineRenderer.SetPosition(1, _endPoint);
        }
        else
        {
            _lineRenderer.enabled = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (_rigidbody2D == null)
                return;
            _endPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Mouse Clicked Up");
            _endPoint.z = 1;
            CalculateDirection();
            _isDrawing=false;
        }
    }
    private void CalculateDirection()
    { 
        Vector3 direction = _startingPoint - _endPoint;
        //Debug.Log("Direction is: " +direction);

        _rigidbody2D.AddForce(direction * _mouseDragStrengthMultiplication);

        _rigidbody2D = null;
    }
}
