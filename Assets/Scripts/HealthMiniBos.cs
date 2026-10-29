using UnityEngine;

public class MiniBossHealth : MonoBehaviour
{
    public int maxHealth = 15;
    public GameObject Coin;

    int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " HP: " + currentHealth + " / " + maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (LevelManager.instance != null)
        {
        LevelManager.instance.AddKill();
        }

        if (Coin != null)
        {
        Instantiate(Coin, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}