using UnityEngine;

public class WindSource : MonoBehaviour
{
    [Header("Wind Settings")]
    [SerializeField] private float _windForce = 20f;
    [SerializeField] private bool _isIntermittent = false;

    [Header("Intermittent Mode Settings")]
    [SerializeField] private float _activeDuration = 2f;
    [SerializeField] private float _inactiveDuration = 2f;

    private float _timer;
    private bool _isActive = true;

    private void Update()
    {
        if (_isIntermittent)
        {
            HandleIntermittentLogic();
        }
    }

    private void HandleIntermittentLogic()
    {
        _timer += Time.deltaTime;

        if (_isActive && _timer >= _activeDuration)
        {
            _isActive = false;
            _timer = 0f;
        }
        else if (!_isActive && _timer >= _inactiveDuration)
        {
            _isActive = true;
            _timer = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!_isActive) return;

        if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            Vector2 direction = transform.up;

            rb.AddForce(direction * _windForce, ForceMode2D.Force);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _isActive ? Color.cyan : Color.gray;

        Vector3 start = transform.position;
        Vector3 direction = transform.up * (_windForce / 5f); 

        Gizmos.DrawRay(start, direction);

        Vector3 right = Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0, 0, 150) * Vector3.up * 0.5f;
        Vector3 left = Quaternion.LookRotation(Vector3.forward, direction) * Quaternion.Euler(0, 0, -150) * Vector3.up * 0.5f;

        Gizmos.DrawRay(start + direction, right);
        Gizmos.DrawRay(start + direction, left);

        if (TryGetComponent(out BoxCollider2D box))
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.offset, box.size);
        }
    }
}