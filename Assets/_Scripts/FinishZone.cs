using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishZone : MonoBehaviour
{
    public int CountPlayer;
    private int _playersInZone = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playersInZone++;
            CheckWinCondition();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playersInZone--;
        }
    }

    private void CheckWinCondition()
    {
        if (_playersInZone >= 2)
        {
            Debug.Log("Победа! Все игроки добрались до финиша.");
            UnlockNextLevel();
            
            GameLevelManager levelManager = FindObjectOfType<GameLevelManager>();
            if (levelManager != null)
            {
                levelManager.WinLevel();
            }
            else
            {
                Debug.LogWarning("GameLevelManager не найден! Переход в обход экрана победы...");
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                SceneManager.LoadScene(nextSceneIndex);
            }
        }
    }

    private void UnlockNextLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int reachedLevel = PlayerPrefs.GetInt("ReachedLevel", 1);

        if (currentLevel >= reachedLevel)
        {
            PlayerPrefs.SetInt("ReachedLevel", currentLevel);
        }
    }
}