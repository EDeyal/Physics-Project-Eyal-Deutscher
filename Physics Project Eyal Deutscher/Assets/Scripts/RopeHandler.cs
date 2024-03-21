using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class RopeHandler : MonoSingleton<RopeHandler> 
{
    [SerializeField] LineRenderer _lineRenderer;
    EyalRigidbody2D _rigidbodyConnectedToRope;
    public bool HasConnectedRope;
    [SerializeField] List<EyalCollider> _ropeColliders;
    [SerializeField] Vector3 _spareCollidersLocation;
    private void Start()
    {
        foreach (var item in _ropeColliders)
        {
            item.TriggerEntered += CheckIfCutRope;
        }
    }
    public void EnableRope()
    {
        _lineRenderer.enabled = true;
    }
    public void DrawRope(EyalRigidbody2D rb, Vector3 point2)
    {
        HasConnectedRope = true;
        _rigidbodyConnectedToRope = rb;
        _lineRenderer.SetPosition(0, point2);
        _lineRenderer.SetPosition(1, rb.transform.position);

        SetRopeCollider(rb.transform.position, point2);
    }
    private void SetRopeCollider(Vector3 point1,Vector3 point2)
    {
        //get y difference between both loctions, that will be the amount of cubes between them
        //then place the cubes on a line between the 2 points
        int amountOfCollidersNeeded = (int)(point1.y - point2.y);
        amountOfCollidersNeeded = Mathf.Abs(amountOfCollidersNeeded);
        //Debug.Log("Amount of colliders needed is: " + amountOfCollidersNeeded);

        Vector3 direction = point1 - point2;
        direction.Normalize();
        Vector3 nextColliderPos = point2;
        for (int i = 0; i < amountOfCollidersNeeded; i++)
        {
            nextColliderPos = point2 + (direction * i);
            if (_ropeColliders[i] == null)
            { 
                Debug.LogError("Fill More Colliders to the rope");
                return;
            }
            _ropeColliders[i].transform.position = nextColliderPos;
            //Debug.Log("PositionToSpawn:" + nextColliderPos);
        }
        SetOtherCollidersAside(amountOfCollidersNeeded);
        //Vector3 location = (point2 + point2);//middle pos
        //location /= 2;
        //float magnitude = Vector3.Distance(point2, point1);
        //_ropeCollider.transform.position = location;
        //_ropeCollider.transform.LookAt(point2);
        //_ropeCollider.transform.localScale = new Vector3(1, 0.3f, magnitude);
    }
    private void SetOtherCollidersAside(float amountOfCollidersNeeded)
    {
        for (int i = 0; i < _ropeColliders.Count; i++)
        {
            if (i >= amountOfCollidersNeeded)
            {
                _ropeColliders[i].transform.position = _spareCollidersLocation;
            }
        }
    }
    public void CheckIfCutRope(EyalCollider otherCollider)
    {
        //Debug.Log("Checking if cut");
        if(otherCollider.gameObject.tag == "FiredObject" && HasConnectedRope)
        {
            _rigidbodyConnectedToRope.DetachFromRope();
            _lineRenderer.enabled = false;
            foreach (var collider in _ropeColliders)
            {
                collider.gameObject.SetActive(false);
                collider.TriggerEntered -= CheckIfCutRope;
            }
            //Debug.Log("Getting Cut");
        }
    }
}
