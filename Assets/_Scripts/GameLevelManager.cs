using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance { get; private set; }

    [Header("Параметры игроков")]
    [SerializeField] private PlayerStatus[] _players;

    [Header("Точка финиша")]
    [SerializeField] private GameObject _finishPoint;

    [Header("Интерфейс (UI)")]
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private GameObject _victoryPanel;
    [SerializeField] private TMP_Text _coinCountText;

    [Header("Параметры задержки")]
    [SerializeField] private float _delayBeforeScreenShow = 1.5f;

    private int _alivePlayersCount;
    private bool _isGameOver = false;
    private int _collectedCoins = 0;

    private void Awake()
        {
            //синглтон при запуске сцены
            Instance = this;
        }


    private void Start()
    {
        _alivePlayersCount = _players.Length;
        _isGameOver = false;
         _collectedCoins = 0;

        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }
        if (_victoryPanel != null) _victoryPanel.SetActive(false);
        UpdateCoinUI();
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

    public void AddCoin()
    {
        _collectedCoins++;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (_coinCountText != null)
        {
            _coinCountText.text = "x" + _collectedCoins.ToString();
        }
    }

    public void WinLevel()
    {
        if (_isGameOver) return;
        _isGameOver = true;

        if (_victoryPanel != null)
        {
            _victoryPanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("Экран победы успешно показан!");
        }
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;

        string currentSceneName = SceneManager.GetActiveScene().name;
        
        string numberOnly = currentSceneName.Replace("Lvl", "").Replace("lvl", "").Trim();

        if (int.TryParse(numberOnly, out int currentLevelNumber))
        {
            int nextLevelNumber = currentLevelNumber + 1;
            
            string prefix = currentSceneName.StartsWith("lvl") ? "lvl" : "Lvl";
            string nextSceneName = prefix + nextLevelNumber; // Например, "Lvl2"

            if (DoesSceneExist(nextSceneName))
            {
                Debug.Log($"Загружаем следующий уровень по имени: {nextSceneName}");
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log($"Следующий уровень {nextSceneName} не добавлен в Build Settings. Игра пройдена!");
                
                SceneManager.LoadScene("LevelMenu"); 
            }
        }
        else
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    private bool DoesSceneExist(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            
            string nameInBuild = System.IO.Path.GetFileNameWithoutExtension(path);
            
            if (nameInBuild.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
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