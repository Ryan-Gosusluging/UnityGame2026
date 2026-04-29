using UnityEngine;
using UnityEngine.Events;

public class ButtonInteractable : MonoBehaviour
{
    [Header("Настройки продавливания")]
    [SerializeField] private Transform _buttonTop;
    [SerializeField] private float _pressDepth = 0.2f;
    [SerializeField] private float _pressSpeed = 8f;
    [SerializeField] private float _releaseSpeed = 4f;

    [Header("Режимы")]
    [SerializeField] private bool _stayPressed = false;
    [SerializeField] private float _releaseDelay = 0.3f;

    [Header("События")]
    public UnityEvent OnButtonActivated;
    public UnityEvent OnButtonDeactivated;

    private Vector3 _topDefaultPosition;
    private bool _isPressed = false;
    private float _releaseTimer = 0f;
    private int _objectsOnButton = 0;

    public bool IsPressed => _isPressed;

    private void Start()
    {
        if (_buttonTop == null)
            _buttonTop = transform;

        _topDefaultPosition = _buttonTop.localPosition;
    }

    private void Update()
    {
        HandleButtonAnimation();
        HandleReleaseTimer();
    }

    private void HandleButtonAnimation()
    {
        Vector3 targetPosition;

        if (_isPressed)
        {
            targetPosition = _topDefaultPosition + Vector3.down * _pressDepth;
        }
        else
        {
            targetPosition = _topDefaultPosition;
        }

        float speed = _isPressed ? _pressSpeed : _releaseSpeed;
        _buttonTop.localPosition = Vector3.Lerp(
            _buttonTop.localPosition,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    private void HandleReleaseTimer()
    {
        if (!_stayPressed && _isPressed && _objectsOnButton <= 0)
        {
            _releaseTimer -= Time.deltaTime;

            if (_releaseTimer <= 0f)
            {
                DeactivateButton();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D Collision)
    {
        if (IsCollisionFromTop(Collision))
        {
            _objectsOnButton++;

            if (!_isPressed)
            {
                ActivateButton();
            }

            _releaseTimer = _releaseDelay;
        }
    }

    private void OnCollisionExit2D(Collision2D Collision)
    {
        _objectsOnButton--;

        if (_objectsOnButton <= 0)
        {
            _objectsOnButton = 0;

            if (!_stayPressed)
            {
                _releaseTimer = _releaseDelay;
            }
        }
    }

    private bool IsCollisionFromTop(Collision2D Collision)
    {
        foreach (ContactPoint2D contact in Collision.contacts)
        {
            if (contact.normal.y < -0.5f) 
            {
                return true;
            }
        }
        return false;
    }

    private void ActivateButton()
    {
        if (_isPressed) return;

        _isPressed = true;
        OnButtonActivated?.Invoke();
        Debug.Log("Кнопка нажата!");
    }

    private void DeactivateButton()
    {
        if (!_isPressed) return;

        _isPressed = false;
        OnButtonDeactivated?.Invoke();
        Debug.Log("Кнопка отжата!");
    }
    public void ResetButton()
    {
        _objectsOnButton = 0;
        DeactivateButton();
    }

    private void OnDrawGizmos()
    {
        if (_buttonTop != null)
        {
            Gizmos.color = _isPressed ? Color.green : Color.yellow;
            Vector3 pressedPos = _buttonTop.position + Vector3.down * _pressDepth;
            Gizmos.DrawWireCube(pressedPos, transform.localScale);
        }
    }
}