using UnityEngine;

public class MiniBossShoot : MonoBehaviour
{
    [Header("long range attack")]
    public GameObject bulletPrefab;
    public float fireRate = 1.5f;
    public float bulletSpeed = 6f;

    [Header("Melee attack")]
    public int meleedamage = 3;
    public float meleerange = 1.5f;
    public float meleecooldown = 0.5f;
    

    Transform player;
    float nextFireTime;
    float nextMeleeTime;
    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
            return;
        }
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= meleerange)
        {
            if (Time.time >= nextMeleeTime){
                melee();
                nextMeleeTime = Time.time + meleecooldown;
                } 
        } else if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;

                
            }

    }

    void melee()
    {
        Health playerhealth = player.GetComponent<Health>();
        if (playerhealth != null)
        {
            Debug.Log("Kena damage bang segini " +meleedamage);
            playerhealth.TakeDamage(meleedamage);
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