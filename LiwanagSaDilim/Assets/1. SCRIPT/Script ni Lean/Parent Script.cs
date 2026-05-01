using UnityEngine;

public class PlatformGroup : MonoBehaviour
{
    [SerializeField] private GameObject[] platforms;

    public void Activate()
    {
        foreach (GameObject platform in platforms)
        {
            if (platform != null)
            {
                platform.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}