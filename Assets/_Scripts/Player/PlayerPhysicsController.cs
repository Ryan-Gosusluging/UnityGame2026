// Приватные поля начинаем с _
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerPhysicsController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Collider2D _collider;

    private IPlayerStatsProvider _statsProvider;

    private bool _isGrounded;
    private float _currentHorizontalInput;
    private bool _jumpRequested;

    public bool IsGrounded => _isGrounded;

    private void Awake()
    {
        _statsProvider = GetComponent<IPlayerStatsProvider>();
        if (_statsProvider == null)
        {
            Debug.LogError("IPlayerStatsProvider не найден");
        }
    }

    private void Update()
    {
        _isGrounded = CheckGround();
        if (_jumpRequested && _isGrounded)
        {
            ExecuteJump();
        }
        _jumpRequested = false;
    }

    public void SetMoveInput(float HorizontalAxis)
    {
        _currentHorizontalInput = HorizontalAxis;
    }

    public void RequestJump()
    {
        _jumpRequested = true;
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
    }

    private void ApplyHorizontalMovement()
    {
        if (_statsProvider == null) return;

        float targetSpeed = _currentHorizontalInput * _statsProvider.MoveSpeed;
        float smoothedSpeed = Mathf.Lerp(_rigidbody.linearVelocity.x, targetSpeed, _statsProvider.GroundAcceleration * Time.fixedDeltaTime);

        _rigidbody.linearVelocity = new Vector2(smoothedSpeed, _rigidbody.linearVelocity.y);
    }

    private void ExecuteJump()
    {
        _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, 0f);
        _rigidbody.AddForce(Vector2.up * _statsProvider.JumpForce, ForceMode2D.Impulse);
    }

    private bool CheckGround()
    {
        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(_collider.bounds.center, Vector2.down, _collider.bounds.extents.y + extraHeight, _statsProvider.GroundLayer);
        return hit.collider != null;
    }
}