using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float speed = 7f;
    public float lifeTime = 4.5f;
    public int damage = 1;

    private Vector2 moveDirection = Vector2.zero;
    private bool isDirectionSet = false;

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        isDirectionSet = true;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (isDirectionSet)
        {
            transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Hanya berikan damage jika mengenai Player
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
        // Jangan hancur jika menabrak sesama Enemy / Boss
    }
}
