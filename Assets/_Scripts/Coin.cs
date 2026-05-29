using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameLevelManager.Instance != null)
            {
                GameLevelManager.Instance.AddCoin();
            }

            Destroy(gameObject);
        }
    }
}