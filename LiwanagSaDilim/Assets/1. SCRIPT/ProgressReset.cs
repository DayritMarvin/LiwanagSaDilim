using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressReset : MonoBehaviour
{
    // Ito ang tatawagin mo sa On Click () ng "Yes" button
    public void ResetAllProgress()
    {
        // 1. Tawagin ang sarili mong Reset function sa GameManager
        // (Ito yung bubura ng PlayerPrefs AT mag-re-reset ng Memory ng laro)
        GameManager.ResetAllProgress();

        // 2. I-reload ang current scene (MainMenu) para mag-refresh ang mga buttons
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}