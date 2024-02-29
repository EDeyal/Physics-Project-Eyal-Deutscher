using UnityEngine;

public class EyalCollider : MonoBehaviour
{
    [SerializeField] bool _isTrigger;
    EyalRigidbody2D _rigidbody;

    public EyalRigidbody2D Rigidbody => _rigidbody;
    public bool IsTrigger => _isTrigger;
    Renderer _renderer;

    public Bounds Bounds
    {
        get
        {
            return _renderer.bounds;
        }
    }
    private void OnValidate()
    {
        if (_renderer == null)
        {
            gameObject.GetComponent<Renderer>();
        }
    }
    public void Start()
    {
        CollisionManager.Instance.RigisterCollider(this);
    }
    private void OnDestroy()
    {
        CollisionManager.Instance.UnrigisterCollider(this);
    }
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
    public void SetRigidbody(EyalRigidbody2D rigidbody)
    {
        _rigidbody = rigidbody;
    }
    public void OnEyalTriggerEnter(EyalCollider other)
    {
        if (_isTrigger)
        {
            TriggerEntered?.Invoke(other);
        }
    }

    public void OnEyalCollisionEnter(EyalCollider other)
    {
        if (!_isTrigger)
        { 
            CollisionEntered?.Invoke(other);
        }
    }
}
