using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] EyalRigidbody2D _rigidbody2D;
    [SerializeField] float _mouseDragStrength = 1;
    [SerializeField] Camera _camera;
    [SerializeField] LineRenderer _lineRenderer;
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
            _startingPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Mouse Clicked Down");
            _isDrawing = true;
            _startingPoint.z = 1;
        }
        if (_isDrawing)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.SetPosition(0, _startingPoint);
            _lineRenderer.SetPosition(1, _camera.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
            _lineRenderer.enabled = false;
        }
        if (Input.GetMouseButtonUp(0))
        {
            _endPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Mouse Clicked Up");
            CalculateDirection();
            _isDrawing=false;
        }
    }
    private void CalculateDirection()
    { 
        Vector3 direction = _startingPoint - _endPoint;
        Debug.Log("Direction is: " +direction);
        _rigidbody2D.AddForce(direction / _mouseDragStrength);
    }
}
