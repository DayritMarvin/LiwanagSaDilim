using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("LevelMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }



    //Chapter Menu Button
    public void ChapterBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    //MainStory Button
    public void MainStorySkip()
    {
        SceneManager.LoadScene("Level 1");
    }

}
