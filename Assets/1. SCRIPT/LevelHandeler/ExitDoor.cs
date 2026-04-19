using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    private LevelHandler levelHandler;

    void Start()
    {
        levelHandler = FindObjectOfType<LevelHandler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (levelHandler != null)
            {
                levelHandler.levelFinished();
            }
        }
    }
}