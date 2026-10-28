
using Unity.Mathematics;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public GameObject Coin;
    public GameObject[] health; 

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(gameObject.name + " HP: " + currentHealth);
        Destroy(health[currentHealth]);

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
            Debug.Log($"Test");
        }

        Destroy(gameObject);
    }
}
