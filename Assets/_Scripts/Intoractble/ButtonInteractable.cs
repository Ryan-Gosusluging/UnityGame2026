using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

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

    [Header("Требования к весу")]
    [SerializeField] private bool _requireHeavyObject = true;
    [SerializeField] private float _requiredMinMass = 3f;

    [Header("События")]
    public UnityEvent OnButtonActivated;
    public UnityEvent OnButtonDeactivated;

    private Vector3 _topDefaultPosition;
    private bool _isPressed = false;
    private float _releaseTimer = 0f;

    // Храним объекты на кнопке и их массу
    private Dictionary<GameObject, float> _objectsOnButton = new Dictionary<GameObject, float>();

    public bool IsPressed => _isPressed;

    private void Start()
    {
        if (_buttonTop == null)
            _buttonTop = transform;
        _topDefaultPosition = _buttonTop.localPosition;

        // Убедимся, что коллайдер НЕ триггер
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.isTrigger = false;
        }
    }

    private void Update()
    {
        HandleButtonAnimation();
        HandleReleaseTimer();
        CheckAllObjectsValidity();
    }

    private void HandleButtonAnimation()
    {
        Vector3 targetPosition = _isPressed
            ? _topDefaultPosition + Vector3.down * _pressDepth
            : _topDefaultPosition;

        float speed = _isPressed ? _pressSpeed : _releaseSpeed;
        _buttonTop.localPosition = Vector3.Lerp(
            _buttonTop.localPosition,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    private void HandleReleaseTimer()
    {
        if (!_stayPressed && _isPressed && !HasValidObject())
        {
            _releaseTimer -= Time.deltaTime;
            if (_releaseTimer <= 0f)
            {
                DeactivateButton();
            }
        }
        else if (_isPressed && HasValidObject())
        {
            // Если есть валидный объект, сбрасываем таймер
            _releaseTimer = _releaseDelay;
        }
    }

    private bool HasValidObject()
    {
        foreach (var obj in _objectsOnButton)
        {
            if (obj.Key != null && obj.Value >= _requiredMinMass)
            {
                return true;
            }
        }
        return false;
    }

    private void CheckAllObjectsValidity()
    {
        bool hadValid = HasValidObject();

        // Очищаем уничтоженные объекты
        List<GameObject> toRemove = new List<GameObject>();
        foreach (var obj in _objectsOnButton)
        {
            if (obj.Key == null)
            {
                toRemove.Add(obj.Key);
            }
        }

        foreach (var obj in toRemove)
        {
            _objectsOnButton.Remove(obj);
        }

        bool hasValid = HasValidObject();

        if (hadValid != hasValid)
        {
            if (hasValid && !_isPressed)
            {
                ActivateButton();
            }
            else if (!hasValid && _isPressed)
            {
                _releaseTimer = _releaseDelay;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsCollisionFromTop(collision))
        {
            float mass = GetObjectMass(collision.gameObject);
            _objectsOnButton[collision.gameObject] = mass;

            Debug.Log($"Button: {collision.gameObject.name} entered. Mass: {mass}");

            if (mass >= _requiredMinMass && !_isPressed)
            {
                ActivateButton();
            }
            _releaseTimer = _releaseDelay;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_objectsOnButton.ContainsKey(collision.gameObject))
        {
            _objectsOnButton.Remove(collision.gameObject);
            Debug.Log($"Button: {collision.gameObject.name} exited");

            if (!HasValidObject() && _isPressed)
            {
                _releaseTimer = _releaseDelay;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Проверяем, не изменилась ли масса объекта (смена формы)
        if (_objectsOnButton.ContainsKey(collision.gameObject))
        {
            float currentMass = GetObjectMass(collision.gameObject);
            float storedMass = _objectsOnButton[collision.gameObject];

            if (Mathf.Abs(currentMass - storedMass) > 0.01f)
            {
                _objectsOnButton[collision.gameObject] = currentMass;
                Debug.Log($"Button: {collision.gameObject.name} mass changed to {currentMass}");

                if (currentMass >= _requiredMinMass && !_isPressed)
                {
                    ActivateButton();
                }
                else if (currentMass < _requiredMinMass && _isPressed && !HasValidObject())
                {
                    _releaseTimer = _releaseDelay;
                }
            }
        }
    }

    private bool IsCollisionFromTop(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Проверяем, что объект надавил сверху (нормаль направлена вниз)
            if (contact.normal.y < -0.5f)
            {
                return true;
            }
        }
        return false;
    }

    private float GetObjectMass(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            return rb.mass;
        }
        return 0f;
    }

    private void ActivateButton()
    {
        if (_isPressed) return;
        _isPressed = true;
        OnButtonActivated?.Invoke();
        Debug.Log("!!! Кнопка НАЖАТА !!!");
    }

    private void DeactivateButton()
    {
        if (!_isPressed) return;
        _isPressed = false;
        OnButtonDeactivated?.Invoke();
        Debug.Log("!!! Кнопка ОТЖАТА !!!");
    }

    public void ResetButton()
    {
        _objectsOnButton.Clear();
        DeactivateButton();
    }

    public void ForceUpdateState()
    {
        CheckAllObjectsValidity();
    }
}