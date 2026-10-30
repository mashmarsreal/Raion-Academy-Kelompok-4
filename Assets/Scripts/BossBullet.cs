using Unity.VisualScripting;
using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // panggil fungsi darah Player 
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log("Player kena peluru mini boss!");
            }
            Destroy(gameObject);
        }
    }
}