
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int killCount = 0;
    public TextMeshProUGUI killText;

    public int coinCount = 0;
    public TextMeshProUGUI coinText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateKillText();
        UpdateCoinText();   
    }

    public void AddKill()
    {
        killCount++;
        UpdateKillText();
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinText();
    }

    private void UpdateKillText()
    {
        if (killText != null)
        {
            killText.text = "Kills: " + killCount;
        }
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coinCount;
        }
    }
}
