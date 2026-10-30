
using System;
using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 20;
    private int currentHealth;
    public GameObject Coin;
    public static event Action onDie;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(gameObject.name + " HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (CompareTag("Enemy"))
        {
        LevelManager.instance.AddKill();
        Instantiate(Coin, transform.position, Quaternion.identity);
        }
        if (CompareTag("Player"))
        {
            onDie?.Invoke();
            Debug.Log($"Player Get hit");
        }

        Destroy(gameObject);
    }
}
