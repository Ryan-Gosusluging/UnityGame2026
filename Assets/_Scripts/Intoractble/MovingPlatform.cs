using UnityEngine;
using System.Collections.Generic;

public enum PlatformMode
{
    Auto,       
    OnButton,  
    Triggered  
}

public class MovingPlatform : MonoBehaviour
{
    [Header("Режим")]
    [SerializeField] private PlatformMode _mode = PlatformMode.Auto;

    [Header("Точки маршрута")]
    [SerializeField] private List<Transform> _waypoints = new List<Transform>();

    [Header("Настройки движения")]
    [SerializeField] private float _speed = 3f;
    [SerializeField] private float _waitTime = 1f;
    [SerializeField] private bool _loop = true;

    [Header("Для режима OnButton")]
    [SerializeField] private ButtonInteractable _linkedButton;

    private int _currentWaypointIndex = 0;
    private int _direction = 1;
    private float _waitTimer = 0f;
    private bool _isMoving = true;

    private void Start()
    {
        if (_mode == PlatformMode.Auto)
        {
            _isMoving = true;
        }
        else if (_mode == PlatformMode.OnButton)
        {
            _isMoving = false;
            if (_linkedButton != null)
            {
                _linkedButton.OnButtonActivated.AddListener(StartMoving);
                _linkedButton.OnButtonDeactivated.AddListener(StopMoving);
            }
        }
        else if (_mode == PlatformMode.Triggered)
        {
            _isMoving = false;
        }
    }

    private void FixedUpdate()
    {
        if (!_isMoving || _waypoints.Count < 2) return;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.fixedDeltaTime;
            return;
        }

        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Transform target = _waypoints[_currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance < 0.05f)
        {
            _waitTimer = _waitTime;

            if (_loop)
            {
                _currentWaypointIndex += _direction;

                if (_currentWaypointIndex >= _waypoints.Count || _currentWaypointIndex < 0)
                {
                    _direction *= -1;
                    _currentWaypointIndex = Mathf.Clamp(_currentWaypointIndex, 0, _waypoints.Count - 1);
                }
            }
            else
            {
                if (_currentWaypointIndex == _waypoints.Count - 1)
                {
                    _isMoving = false;
                    return;
                }
                _currentWaypointIndex++;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                _speed * Time.fixedDeltaTime
            );
        }
    }


    public void StartMoving()
    {
        _isMoving = true;
    }

    public void StopMoving()
    {
        _isMoving = false;
    }

    public void ToggleMoving()
    {
        _isMoving = !_isMoving;
    }

    private void OnButtonPressed()
    {
        if (_mode == PlatformMode.OnButton)
            _isMoving = true;
    }

    private void OnButtonReleased()
    {
        if (_mode == PlatformMode.OnButton)
            _isMoving = false;
    }

    private void OnDestroy()
    {
        if (_linkedButton != null)
        {
            _linkedButton.OnButtonActivated.AddListener(StartMoving);
            _linkedButton.OnButtonDeactivated.AddListener(StopMoving);
        }
    }

    private void OnDrawGizmos()
    {
        if (_waypoints.Count < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < _waypoints.Count; i++)
        {
            if (_waypoints[i] != null)
                Gizmos.DrawWireSphere(_waypoints[i].position, 0.2f);
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < _waypoints.Count - 1; i++)
        {
            if (_waypoints[i] != null && _waypoints[i + 1] != null)
                Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
        }
    }
}