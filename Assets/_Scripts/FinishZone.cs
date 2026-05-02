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
        if (_playersInZone >= CountPlayer)
        {
            Debug.Log("Уровень пройден вместе!");
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}