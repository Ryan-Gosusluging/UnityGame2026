using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _bounceForce = 15f;
    [SerializeField] private bool _resetVerticalVelocity = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            ApplyBounce(rb);
        }
    }

    private void ApplyBounce(Rigidbody2D rb)
    {
        Vector2 newVelocity = rb.linearVelocity;

        if (_resetVerticalVelocity)
        {
            newVelocity.y = 0f;
        }

        rb.linearVelocity = newVelocity;
        rb.AddForce(Vector2.up * _bounceForce, ForceMode2D.Impulse);
    }
}