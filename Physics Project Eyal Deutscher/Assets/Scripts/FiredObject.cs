using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiredObject : MonoBehaviour
{
    [SerializeField] EyalRigidbody2D _rigidbody;
    public void Update()
    {
        if (_rigidbody == null) return;
        if (RopeHandler.Instance.HasConnectedRope) ;
            //RopeHandler.Instance.CheckIfCutRope(_rigidbody.Collider);
    }
}
