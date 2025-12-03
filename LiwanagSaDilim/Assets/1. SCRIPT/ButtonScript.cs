using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("MainStory");
    }

    public void Chapter()
    {
        SceneManager.LoadScene("LevelMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }

}
