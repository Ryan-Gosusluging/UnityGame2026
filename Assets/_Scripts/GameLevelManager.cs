using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLevelManager : MonoBehaviour
{
    [Header("Параметры игроков")]
    [SerializeField] private PlayerStatus[] _players;

    [Header("Точка финиша")]
    [SerializeField] private GameObject _finishPoint;

    [Header("Интерфейс (UI)")]
    [SerializeField] private GameObject _gameOverPanel;

    [Header("Параметры задержки")]
    [SerializeField] private float _delayBeforeScreenShow = 1.5f;

    private int _alivePlayersCount;
    private bool _isGameOver = false;

    private void Start()
    {
        _alivePlayersCount = _players.Length;
        _isGameOver = false;

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }
        Time.timeScale = 1f; 
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
        if (_isGameOver) return; 

        _isGameOver = true;
        _alivePlayersCount--;

        if (_finishPoint != null && _finishPoint.activeSelf)
        {
            Debug.Log("Игрок погиб, финиш скрыт!");
            _finishPoint.SetActive(false);
        }


        Debug.Log("Один из игроков погиб! Запуск показа экрана конца игры...");
        StartCoroutine(ShowGameOverRoutine());
    }

    private IEnumerator ShowGameOverRoutine()
    {
        Debug.Log("[КОРУТИНА] ShowGameOverRoutine успешно запустилась и начинает ждать...");

        yield return new WaitForSecondsRealtime(_delayBeforeScreenShow);

        Debug.Log("[КОРУТИНА] Время ожидания прошло!");

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            Time.timeScale = 0f; 
            Debug.Log("[КОРУТИНА] Экран конца игры успешно включен.");
        }
        else
        {
            Debug.LogError("[КОРУТИНА] Ошибка: Панель конца игры НЕ назначена в инспекторе GameLevelManager!");
            RestartLevel();
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; 
        
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.LoadScene(0); 
        }
        else
        {
            SceneManager.LoadScene(0); 
        }
    }
}