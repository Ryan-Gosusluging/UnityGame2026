using UnityEngine;
using System.Collections.Generic;

public class PlayerCamera : MonoBehaviour
{
    [Header("Игроки")]
    [SerializeField] private List<Transform> _targets = new List<Transform>();

    [Header("Настройки камеры")]
    [SerializeField] private float _minOrthographicSize = 5f;
    [SerializeField] private float _maxOrthographicSize = 15f;
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private Vector2 _offset = Vector2.zero;

    [Header("Отступы от краев экрана")]
    [SerializeField] private float _horizontalPadding = 2f;
    [SerializeField] private float _verticalPadding = 2f;

    [Header("Ограничения позиции (опционально)")]
    [SerializeField] private bool _useBounds = false;
    [SerializeField] private Vector2 _minBounds;
    [SerializeField] private Vector2 _maxBounds;

    private Camera _camera;
    private Vector3 _currentVelocity;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        if (_camera == null)
            _camera = Camera.main;
    }

    private void Start()
    {
        if (_targets.Count == 0)
        {
            FindPlayers();
        }
    }

    private void LateUpdate()
    {
        if (_targets.Count == 0) return;

        _targets.RemoveAll(t => t == null);

        if (_targets.Count == 0) return;

        Vector2 centerPoint = GetCenterPoint();
        float requiredSize = GetRequiredOrthographicSize();

        requiredSize = Mathf.Clamp(requiredSize, _minOrthographicSize, _maxOrthographicSize);

        Vector3 targetPosition = new Vector3(
            centerPoint.x + _offset.x,
            centerPoint.y + _offset.y,
            transform.position.z
        );

        if (_useBounds)
        {
            float halfHeight = requiredSize;
            float halfWidth = requiredSize * _camera.aspect;

            targetPosition.x = Mathf.Clamp(targetPosition.x, _minBounds.x + halfWidth, _maxBounds.x - halfWidth);
            targetPosition.y = Mathf.Clamp(targetPosition.y, _minBounds.y + halfHeight, _maxBounds.y - halfHeight);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _currentVelocity,
            1f / _smoothSpeed
        );

        _camera.orthographicSize = Mathf.Lerp(
            _camera.orthographicSize,
            requiredSize,
            _smoothSpeed * Time.deltaTime
        );
    }

    private Vector2 GetCenterPoint()
    {
        if (_targets.Count == 1)
        {
            return _targets[0].position;
        }

        Vector2 sum = Vector2.zero;
        foreach (Transform target in _targets)
        {
            sum += (Vector2)target.position;
        }

        return sum / _targets.Count;
    }

    private float GetRequiredOrthographicSize()
    {
        if (_targets.Count == 1)
        {
            return _minOrthographicSize;
        }

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Transform target in _targets)
        {
            Vector2 pos = target.position;

            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        float horizontalDistance = (maxX - minX) + _horizontalPadding * 2f;
        float verticalDistance = (maxY - minY) + _verticalPadding * 2f;

        float requiredSizeByWidth = horizontalDistance / (2f * _camera.aspect);
        float requiredSizeByHeight = verticalDistance / 2f;

        return Mathf.Max(requiredSizeByWidth, requiredSizeByHeight);
    }

    private void FindPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            _targets.Add(player.transform);
        }

        Debug.Log($"Найдено игроков: {_targets.Count}");
    }


    public void AddTarget(Transform Target)
    {
        if (!_targets.Contains(Target))
        {
            _targets.Add(Target);
        }
    }

    public void RemoveTarget(Transform Target)
    {
        _targets.Remove(Target);
    }

    public void ClearTargets()
    {
        _targets.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        if (_useBounds)
        {
            Gizmos.color = Color.yellow;
            Vector2 center = (_minBounds + _maxBounds) / 2f;
            Vector2 size = _maxBounds - _minBounds;
            Gizmos.DrawWireCube(center, size);
        }
    }
}