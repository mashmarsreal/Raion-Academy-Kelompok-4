using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float horizontal;
    private float vertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        /*
            CLUE:
            Ambil input pergerakan pemain menggunakan Input.GetAxisRaw:
            1. Isi variabel horizontal dengan Input.GetAxisRaw("Horizontal");
            2. Isi variabel vertical dengan Input.GetAxisRaw("Vertical");
        */
        // Tulis kode di sini
    }

    private void FixedUpdate()
    {
        Vector2 movement = new Vector2(horizontal, vertical).normalized;
        rb.linearVelocity = movement * moveSpeed;

        /*
            CLUE:
            Gerakkan Player dengan mengatur linearVelocity pada Rigidbody2D:
            rb.linearVelocity = movement * moveSpeed;
        */
        // Tulis kode di sini
    }
}
