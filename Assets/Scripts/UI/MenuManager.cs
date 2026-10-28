using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject PauseUI;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape is being held down.");
            Pause();
        }
    }

    public void Pause()
    {
        if (!PauseUI.activeInHierarchy) Time.timeScale = 0;
        else Time.timeScale = 1;
        PauseUI.SetActive(!PauseUI.activeInHierarchy);
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
