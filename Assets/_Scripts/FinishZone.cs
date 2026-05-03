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
            Debug.Log("Победа! Оба игрока на финише.");
            UnlockNextLevel();
            Invoke("LoadNextScene", 2f);
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

    private void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Игра пройдена!");
            SceneManager.LoadScene(1); 
        }
    }
}