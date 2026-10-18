using UnityEngine;
using UnityEngine.SceneManagement;


public class StartMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Game");
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
