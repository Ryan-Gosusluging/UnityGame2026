using UnityEngine;
using UnityEngine.Events;

public class CooperativeButton : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _buttonID;
    [SerializeField] private GameObject _pressIndicator;

    [Header("Events")]
    public UnityEvent onButtonPressed;
    public UnityEvent onButtonReleased;

    private bool _isPressed = false;
    private int _playersOnButton = 0;

    public int ButtonID => _buttonID;
    public bool IsPressed => _isPressed;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Button {_buttonID}: OnTriggerEnter2D with {other.gameObject.name}, Tag: {other.tag}");

        if (other.CompareTag("Player"))
        {
            _playersOnButton++;
            Debug.Log($"Button {_buttonID}: Players on button = {_playersOnButton}");
            UpdateButtonState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Button {_buttonID}: OnTriggerExit2D with {other.gameObject.name}");

        if (other.CompareTag("Player"))
        {
            _playersOnButton--;
            Debug.Log($"Button {_buttonID}: Players on button = {_playersOnButton}");
            UpdateButtonState();
        }
    }

    private void UpdateButtonState()
    {
        bool wasPressed = _isPressed;
        _isPressed = _playersOnButton > 0;

        Debug.Log($"Button {_buttonID}: UpdateButtonState - wasPressed={wasPressed}, isPressed={_isPressed}");

        if (_isPressed != wasPressed)
        {
            if (_isPressed)
            {
                Debug.Log($"!!! Button {_buttonID}: PRESSED !!!");
                onButtonPressed?.Invoke();
                if (_pressIndicator != null)
                    _pressIndicator.SetActive(true);
            }
            else
            {
                Debug.Log($"!!! Button {_buttonID}: RELEASED !!!");
                onButtonReleased?.Invoke();
                if (_pressIndicator != null)
                    _pressIndicator.SetActive(false);
            }
        }
    }
}