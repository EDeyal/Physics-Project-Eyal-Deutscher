using UnityEngine;

public class EyalCollider : MonoBehaviour
{
    [SerializeField] bool _isTrigger;
    Renderer _renderer;
    //event for trigger interactions
    public delegate void TriggerEventHandler(EyalCollider other);
    public event TriggerEventHandler TriggerEntered;

    //events for collisionInteractions
    public delegate void CollisionEventHandler(EyalCollider colliother);
    public event CollisionEventHandler CollisionEntered;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void OnTriggerEnter(EyalCollider other)
    {
        if (_isTrigger)
        {
            TriggerEntered?.Invoke(other);
        }
    }

    private void OnCollisionEnter(EyalCollider other)
    {
        if (!_isTrigger)
        { 
            CollisionEntered?.Invoke(other);
        }
    }
}
