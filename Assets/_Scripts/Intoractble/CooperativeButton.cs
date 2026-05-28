using UnityEngine;
using UnityEngine.Events;

public class CooperativeButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _buttonID;
    [SerializeField] private GameObject _pressIndicator;

    [Header("Form Requirements")]
    [SerializeField] private bool _requireMassThreshold = true;
    [SerializeField] private float _requiredMinMass = 3f; // Минимальная масса для нажатия (3 = большая форма)

    [Header("Animation")]
    [SerializeField] private Transform _buttonVisual;
    [SerializeField] private Vector3 _pressedOffset = new Vector3(0, -0.1f, 0);
    [SerializeField] private float _animationSpeed = 10f;

    [Header("Events")]
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;

    private bool _isPressed = false;
    private int _playersOnButton = 0;
    private GameObject _currentPlayerOnButton;
    private Vector3 _originalVisualPosition;
    private Vector3 _targetVisualPosition;

    public int ButtonID => _buttonID;
    public bool IsPressed => _isPressed;

    private void Start()
    {
        if (_buttonVisual != null)
        {
            _originalVisualPosition = _buttonVisual.localPosition;
            _targetVisualPosition = _originalVisualPosition;
        }
    }

    private void Update()
    {
        // Анимация кнопки
        if (_buttonVisual != null)
        {
            _buttonVisual.localPosition = Vector3.Lerp(
                _buttonVisual.localPosition,
                _targetVisualPosition,
                Time.deltaTime * _animationSpeed
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Button {_buttonID}: OnTriggerEnter2D with {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            _playersOnButton++;
            if (_currentPlayerOnButton == null)
                _currentPlayerOnButton = other.gameObject;
            UpdateButtonState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Button {_buttonID}: OnTriggerExit2D with {other.gameObject.name}");

        if (other.CompareTag("Player"))
        {
            _playersOnButton--;
            if (_currentPlayerOnButton == other.gameObject)
                _currentPlayerOnButton = null;
            UpdateButtonState();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _playersOnButton > 0)
        {
            UpdateButtonState();
        }
    }

    private void UpdateButtonState()
    {
        bool canPress = CanAnyPlayerPressButton();
        bool wasPressed = _isPressed;
        _isPressed = canPress;

        Debug.Log($"Button {_buttonID}: UpdateButtonState - wasPressed={wasPressed}, isPressed={_isPressed}, canPress={canPress}");

        if (_isPressed != wasPressed)
        {
            if (_isPressed)
            {
                Debug.Log($"!!! Button {_buttonID}: PRESSED !!!");
                onButtonPressed?.Invoke();
                if (_pressIndicator != null)
                    _pressIndicator.SetActive(true);

                // Анимация нажатия
                if (_buttonVisual != null)
                {
                    _targetVisualPosition = _originalVisualPosition + _pressedOffset;
                }
            }
            else
            {
                Debug.Log($"!!! Button {_buttonID}: RELEASED !!!");
                onButtonReleased?.Invoke();
                if (_pressIndicator != null)
                    _pressIndicator.SetActive(false);

                // Анимация возврата
                if (_buttonVisual != null)
                {
                    _targetVisualPosition = _originalVisualPosition;
                }
            }
        }
    }

    private bool CanAnyPlayerPressButton()
    {
        if (_playersOnButton <= 0) return false;
        if (!_requireMassThreshold) return true;

        if (_currentPlayerOnButton != null)
        {
            // Получаем массу игрока через PlayerFormManager
            float playerMass = GetPlayerMass(_currentPlayerOnButton);
            bool canPress = playerMass >= _requiredMinMass;

            Debug.Log($"Button {_buttonID}: Player mass = {playerMass}, Required min mass = {_requiredMinMass}, Can press = {canPress}");

            return canPress;
        }

        return false;
    }

    private float GetPlayerMass(GameObject player)
    {
        var formManager = player.GetComponent<PlayerFormManager>();
        if (formManager != null)
        {
            return formManager.Mass;
        }

        // Fallback: пытаемся получить массу из Rigidbody2D
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            return rb.mass;
        }

        return 0f;
    }

    // Публичный метод для принудительного обновления состояния кнопки
    public void RefreshState()
    {
        UpdateButtonState();
    }

    // Визуализация в редакторе
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}