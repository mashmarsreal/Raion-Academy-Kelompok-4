using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Judul;
    int i = 0;

    void Update()
    {
        Animate();
    }

    void Animate()
    {
        if (i >= 0 && i <= 500) 
        {
            if (i % 25 == 0)
            {
                Judul.transform.localScale = new Vector3(Judul.transform.localScale.x + 0.01f, Judul.transform.localScale.x + 0.01f, Judul.transform.localScale.x + 0.01f);
            }
            i++;
        }
        else if (i > 500 && i <= 1100)
        {
            if (i % 25 == 0)
            {
                Judul.transform.localScale = new Vector3(Judul.transform.localScale.x - 0.01f, Judul.transform.localScale.x - 0.01f, Judul.transform.localScale.x - 0.01f);
            }
            i++;
        } 
        else i = 0;
        Debug.Log(i);
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("UI-Arya-Testing");
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
