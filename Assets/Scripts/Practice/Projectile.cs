
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    public int damage = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        /*
            CLUE:
            Gerakkan peluru maju ke depan/atas lokal menggunakan transform.Translate:
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        */
        // Tulis kode di sini

        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
            CLUE:
            Ganti "false" dengan kondisi pengecekan tag musuh:
            collision.CompareTag("Enemy")
        */
        if (collision.CompareTag("Enemy"))
        {
            Health health = collision.GetComponent<Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }

}
