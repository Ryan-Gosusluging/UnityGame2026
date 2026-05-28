using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float _pushForce = 15f;
    [SerializeField] private float _mass = 2f;
    [SerializeField] private float _gravityScale = 1f;

    private Rigidbody2D _rb;
    private Collider2D _pushableCollider;
    private bool _isBeingPushed = false;
    private float _originalDrag;
    private bool _originalIsKinematic;

    public float Mass => _mass;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody2D>();
        }

        _pushableCollider = GetComponent<Collider2D>();

        _rb.mass = _mass;
        _rb.gravityScale = _gravityScale;
        _originalDrag = 5f;
        _rb.linearDamping = _originalDrag;
        _rb.angularDamping = 2f;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _originalIsKinematic = _rb.isKinematic;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var formManager = collision.gameObject.GetComponent<PlayerFormManager>();

            if (formManager != null && formManager.CanPushObjects)
            {
                // Тяжёлая форма - нормальная физика
                _isBeingPushed = true;
                _rb.isKinematic = false;
                _rb.linearDamping = 0.5f;
                Debug.Log($"Pushable: Heavy player - normal physics");
            }
            else
            {
                // Лёгкая форма - ящик становится кинематическим (неподвижным)
                _isBeingPushed = false;
                _rb.isKinematic = true; // Кинематическое тело не реагирует на силы
                _rb.linearVelocity = Vector2.zero;
                Debug.Log($"Pushable: Light player - kinematic (immovable)");
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var formManager = collision.gameObject.GetComponent<PlayerFormManager>();

            if (formManager != null && formManager.CanPushObjects && _isBeingPushed)
            {
                Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
                float force = _pushForce * formManager.Mass;
                _rb.AddForce(pushDirection * force, ForceMode2D.Force);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isBeingPushed = false;
            _rb.isKinematic = _originalIsKinematic;
            _rb.linearDamping = _originalDrag;
            Debug.Log($"Pushable: Player exited");
        }
    }

    public void ResetBox()
    {
        _isBeingPushed = false;
        _rb.isKinematic = _originalIsKinematic;
        _rb.linearDamping = _originalDrag;
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
    }
}