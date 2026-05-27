using UnityEngine;
using UnityEngine.Events;

public class PlatformSwitcherButton : MonoBehaviour
{
    [Header("Platforms")]
    [SerializeField] private GameObject _platformA; // Первая платформа
    [SerializeField] private GameObject _platformB; // Вторая платформа

    [Header("Initial State")]
    [SerializeField] private bool _platformAIsVisible = true; // Какая платформа видна изначально

    [Header("Visual Feedback")]
    [SerializeField] private SpriteRenderer _buttonVisual;
    [SerializeField] private Color _inactiveColor = Color.gray;
    [SerializeField] private Color _activeColor = Color.green;

    [Header("Events")]
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;

    private bool _isPressed = false;
    private int _playersOnButton = 0;
    private bool _isSwitched = false; // Состояние переключения

    public bool IsPressed => _isPressed;

    private void Start()
    {
        // Устанавливаем начальное состояние
        SetPlatformsVisibility(_platformAIsVisible);
        UpdateButtonVisual();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playersOnButton++;
            UpdateButtonState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playersOnButton--;
            UpdateButtonState();
        }
    }

    private void UpdateButtonState()
    {
        bool wasPressed = _isPressed;
        _isPressed = _playersOnButton > 0;

        if (_isPressed != wasPressed)
        {
            if (_isPressed)
            {
                Debug.Log("Button PRESSED - Switching platforms!");
                onButtonPressed?.Invoke();
                SwitchPlatforms(true); // Переключаем
            }
            else
            {
                Debug.Log("Button RELEASED - Restoring platforms!");
                onButtonReleased?.Invoke();
                SwitchPlatforms(false); // Возвращаем обратно
            }

            UpdateButtonVisual();
        }
    }

    private void SwitchPlatforms(bool switchToAlternate)
    {
        _isSwitched = switchToAlternate;

        if (switchToAlternate)
        {
            // Нажато - показываем противоположную платформу
            SetPlatformsVisibility(!_platformAIsVisible);
        }
        else
        {
            // Отпущено - возвращаем исходное состояние
            SetPlatformsVisibility(_platformAIsVisible);
        }
    }

    private void SetPlatformsVisibility(bool platformAVisible)
    {
        if (_platformA != null)
            SetPlatformVisibility(_platformA, platformAVisible);

        if (_platformB != null)
            SetPlatformVisibility(_platformB, !platformAVisible);

        Debug.Log($"Platform A visible: {platformAVisible}, Platform B visible: {!platformAVisible}");
    }

    private void SetPlatformVisibility(GameObject platform, bool visible)
    {
        if (platform == null) return;

        SpriteRenderer renderer = platform.GetComponent<SpriteRenderer>();
        Collider2D collider = platform.GetComponent<Collider2D>();

        // Управляем видимостью спрайта
        if (renderer != null)
            renderer.enabled = visible;

        // Управляем коллайдером (чтобы можно было стоять)
        if (collider != null)
            collider.enabled = visible;

        // Дополнительно: можно управлять дочерними объектами
        foreach (Transform child in platform.transform)
        {
            child.gameObject.SetActive(visible);
        }
    }

    private void UpdateButtonVisual()
    {
        if (_buttonVisual != null)
            _buttonVisual.color = _isPressed ? _activeColor : _inactiveColor;
    }
}