using UnityEngine;

public class MiniBossShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float fireRate = 1.5f;
    public float bulletSpeed = 6f;

    Transform player;
    float nextFireTime;

    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            return;
        }

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject b = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        b.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
        Destroy(b, 5f);
    }
}