using UnityEngine;
using UnityEngine.Events;

public class CooperativeDoor : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private CooperativeButton _buttonA;
    [SerializeField] private CooperativeButton _buttonB;

    [Header("Door Settings")]
    [SerializeField] private GameObject _doorObject;
    [SerializeField] private Vector3 _openOffset = new Vector3(0, 2, 0);
    [SerializeField] private float _moveSpeed = 2f;

    [Header("Visual Feedback")]
    [SerializeField] private Color _lockedColor = Color.red;
    [SerializeField] private Color _readyColor = Color.green;
    [SerializeField] private SpriteRenderer _doorGlow;

    [Header("Events")]
    public UnityEvent onDoorOpened;
    public UnityEvent onDoorClosed;

    private bool _isDoorOpen = false;
    private bool _hasBeenOpened = false; // Флаг, что дверь уже была открыта
    private Vector3 _closedPosition;
    private Vector3 _openPosition;

    private void Start()
    {
        if (_doorObject == null)
        {
            Debug.LogError("Door Object is not assigned!");
            return;
        }

        _closedPosition = _doorObject.transform.position;
        _openPosition = _closedPosition + _openOffset;

        Debug.Log($"Door initialized - Closed: {_closedPosition}, Open: {_openPosition}");

        if (_buttonA != null)
        {
            _buttonA.onButtonPressed.AddListener(() => CheckButtonsState());
            _buttonA.onButtonReleased.AddListener(() => CheckButtonsState());
        }

        if (_buttonB != null)
        {
            _buttonB.onButtonPressed.AddListener(() => CheckButtonsState());
            _buttonB.onButtonReleased.AddListener(() => CheckButtonsState());
        }
    }

    private void CheckButtonsState()
    {
        // Если дверь уже была открыта, больше не реагируем на кнопки
        if (_hasBeenOpened)
        {
            Debug.Log("Door already opened permanently, ignoring button changes");
            return;
        }

        bool buttonAPressed = _buttonA != null && _buttonA.IsPressed;
        bool buttonBPressed = _buttonB != null && _buttonB.IsPressed;

        bool bothPressedNow = buttonAPressed && buttonBPressed;

        // Если обе кнопки нажаты И дверь ещё не открывалась
        if (bothPressedNow && !_hasBeenOpened)
        {
            Debug.Log("Both buttons pressed for the first time! Door will open permanently.");
            _hasBeenOpened = true;
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        _isDoorOpen = true;
        onDoorOpened?.Invoke();

        if (_doorGlow != null)
            _doorGlow.color = _readyColor;

        Debug.Log($"Door opening to: {_openPosition}");
    }

    private void CloseDoor()
    {
        // Этот метод больше не используется, но оставим для совместимости
        Debug.Log("CloseDoor called but door will stay open permanently");
    }

    private void Update()
    {
        if (_doorObject == null) return;

        // Двигаем дверь только если она должна быть открыта
        if (_isDoorOpen)
        {
            Vector3 targetPosition = _openPosition;
            _doorObject.transform.position = Vector3.MoveTowards(
                _doorObject.transform.position,
                targetPosition,
                _moveSpeed * Time.deltaTime
            );
        }
    }

    // Опционально: метод для сброса (если нужно перезапустить уровень)
    public void ResetDoor()
    {
        _hasBeenOpened = false;
        _isDoorOpen = false;
        _doorObject.transform.position = _closedPosition;
        if (_doorGlow != null)
            _doorGlow.color = _lockedColor;
        Debug.Log("Door has been reset");
    }
}