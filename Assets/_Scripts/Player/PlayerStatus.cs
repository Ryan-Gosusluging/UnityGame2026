using UnityEngine;
using System;

public class PlayerStatus : MonoBehaviour
{
    public event Action OnPlayerDied;
    private bool _isDead = false;
    public void Die()
    {
        if (_isDead) return;

        _isDead = true;
        Debug.Log($"{gameObject.name} погиб!");

        OnPlayerDied?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trap"))
        {
            Die();
        }
    }
}