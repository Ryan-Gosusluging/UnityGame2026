using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLevelManager : MonoBehaviour
{
    [Header("Настройки игроков")]
    [SerializeField] private PlayerStatus[] _players;

    [Header("Объект финиша")]
    [SerializeField] private GameObject _finishPoint;

    [Header("Настройки перезагрузки")]
    [SerializeField] private float _delayBeforeRestart = 2f; 

    private int _alivePlayersCount;

    private void Start()
    {
        _alivePlayersCount = _players.Length;
    }

    private void OnEnable()
    {
        foreach (var player in _players)
        {
            if (player != null)
                player.OnPlayerDied += HandlePlayerDeath;
        }
    }

    private void OnDisable()
    {
        foreach (var player in _players)
        {
            if (player != null)
                player.OnPlayerDied -= HandlePlayerDeath;
        }
    }

    private void HandlePlayerDeath()
    {
        _alivePlayersCount--;

        if (_finishPoint != null && _finishPoint.activeSelf)
        {
            Debug.Log("Финиш заблокирован, так как команда не в полном составе!");
            _finishPoint.SetActive(false);
        }

        if (_alivePlayersCount <= 0)
        {
            Debug.Log("Все игроки погибли. Перезапуск...");
            StartCoroutine(RestartLevelRoutine());
        }
    }

    private IEnumerator RestartLevelRoutine()
    {
        yield return new WaitForSeconds(_delayBeforeRestart);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}