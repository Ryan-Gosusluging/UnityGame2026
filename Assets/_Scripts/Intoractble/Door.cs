using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Настройки двери")]
    [SerializeField] private GameObject _doorBody;
    [SerializeField] private ButtonInteractable _linkedButton;
    [SerializeField] private bool _closeOnPress = false;

    [Header("Анимация двери (смещение)")]
    [SerializeField] private float _offsetX = 0f;      // Смещение по X
    [SerializeField] private float _offsetY = -3f;     // Смещение по Y
    [SerializeField] private float _offsetZ = 0f;      // Смещение по Z
    [SerializeField] private float _animationSpeed = 5f;

    [Header("Дополнительно")]
    [SerializeField] private bool _disableColliderWhenOpen = true;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private bool _isOpen = false;
    private Vector3 _targetPosition;

    private void Start()
    {
        if (_doorBody == null)
            _doorBody = gameObject;

        _closedPosition = _doorBody.transform.localPosition;
        _openPosition = _closedPosition + new Vector3(_offsetX, _offsetY, _offsetZ);
        _targetPosition = _closedPosition;

        if (_linkedButton != null)
        {
            _linkedButton.OnButtonActivated.AddListener(OnButtonPressed);
            _linkedButton.OnButtonDeactivated.AddListener(OnButtonReleased);
            UpdateDoorState(_linkedButton.IsPressed);
        }
    }

    private void Update()
    {
        // Плавное движение двери
        _doorBody.transform.localPosition = Vector3.Lerp(
            _doorBody.transform.localPosition,
            _targetPosition,
            _animationSpeed * Time.deltaTime
        );
    }

    private void OnButtonPressed()
    {
        UpdateDoorState(true);
    }

    private void OnButtonReleased()
    {
        UpdateDoorState(false);
    }

    private void UpdateDoorState(bool isButtonPressed)
    {
        bool shouldBeOpen = _closeOnPress ? isButtonPressed : !isButtonPressed;

        if (shouldBeOpen != _isOpen)
        {
            _isOpen = shouldBeOpen;
            _targetPosition = _isOpen ? _openPosition : _closedPosition;

            // Отключаем коллайдер при открытии (опционально)
            if (_disableColliderWhenOpen && TryGetComponent<Collider2D>(out var doorCollider))
            {
                doorCollider.enabled = !_isOpen;
            }

            Debug.Log($"Door {gameObject.name}: {(_isOpen ? "Opened" : "Closed")}");
        }
    }

    // Публичные методы для ручного управления
    public void OpenDoor() => UpdateDoorState(!_closeOnPress);
    public void CloseDoor() => UpdateDoorState(_closeOnPress);

    public void SetDoorImmediate(bool open)
    {
        _isOpen = open;
        _targetPosition = open ? _openPosition : _closedPosition;
        _doorBody.transform.localPosition = _targetPosition;
    }

    private void OnDestroy()
    {
        if (_linkedButton != null)
        {
            _linkedButton.OnButtonActivated.RemoveListener(OnButtonPressed);
            _linkedButton.OnButtonDeactivated.RemoveListener(OnButtonReleased);
        }
    }

    // Визуализация в редакторе
    private void OnDrawGizmosSelected()
    {
        if (_doorBody != null)
        {
            Gizmos.color = Color.green;
            Vector3 openWorldPos = _doorBody.transform.parent.TransformPoint(_closedPosition + new Vector3(_offsetX, _offsetY, _offsetZ));
            Gizmos.DrawWireCube(openWorldPos, _doorBody.transform.lossyScale);

            Gizmos.color = Color.red;
            Vector3 closedWorldPos = _doorBody.transform.parent.TransformPoint(_closedPosition);
            Gizmos.DrawWireCube(closedWorldPos, _doorBody.transform.lossyScale);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(closedWorldPos, openWorldPos);
        }
    }
}