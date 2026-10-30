using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject PauseUI;
    public GameObject GameOverUI;
    void Start()
    {
        closeAll();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape is being held down.");
            Pause();
        }
    }

    void closeAll()
    {
        PauseUI.SetActive(false);
        GameOverUI.SetActive(false);
    }

    public void Pause()
    {
        if (!PauseUI.activeInHierarchy) Time.timeScale = 0;
        else Time.timeScale = 1;
        PauseUI.SetActive(!PauseUI.activeInHierarchy);
    }

    public void RestartScene()
    {
        Debug.Log($"Test");
        Time.timeScale = 1;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void GameOver()
    {
        // Time.timeScale = 0;
        GameOverUI.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    void OnEnable()
    {
        Health.onDie += GameOver;
    }

    void OnDisable()
    {
        Health.onDie -= GameOver;
    }
}
