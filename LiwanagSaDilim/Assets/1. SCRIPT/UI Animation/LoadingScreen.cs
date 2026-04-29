using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public GameObject loadingPanel; 

    public float minLoadTime = 1f;

    // Ito ang tatawagin mo sa button
    public void LoadSceneWithAnimation(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
        Time.timeScale = 1f;
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        // 1. I-activate ang Loading Screen
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        float timer = 0f;

        // 2. Simulan ang pag-load sa background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        
        // Pigilan muna ang Unity na lumipat agad para makita ang animation
        operation.allowSceneActivation = false; 

        // 3. Loop habang naghihintay
        while (!operation.isDone)
        {
            // Bilangin ang oras na lumipas
            timer += Time.unscaledDeltaTime;

            // Ang Unity ay humihinto sa 0.9 progress kapag pinigilan natin lumipat
            if (operation.progress >= 0.9f)
            {
                // Kung tapos na mag-load AT lumampas na sa minimum time (1.5 seconds)
                if (timer >= minLoadTime)
                {
                    // Lipat na sa next scene!
                    operation.allowSceneActivation = true;
                }
            }

            yield return null; // Maghintay per frame
        }
    }
}