using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxConnectedToRope : MonoBehaviour
{
    [SerializeField] EyalRigidbody2D _rigidbody;
    private void Update()
    {
        if (_rigidbody == null) return;

        if (_rigidbody.IsAttachedToRope)
        {
            RopeHandler.Instance.DrawRope(_rigidbody, _rigidbody.RopeAnchorPoint);
        }
    }
}
