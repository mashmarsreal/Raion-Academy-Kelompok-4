
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (LevelManager.instance != null)
            {
                LevelManager.instance.AddCoin(coinValue);
                Destroy(gameObject);
            }
        }
    }
}
