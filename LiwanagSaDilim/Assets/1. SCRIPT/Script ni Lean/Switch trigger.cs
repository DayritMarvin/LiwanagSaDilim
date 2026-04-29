using UnityEngine;

public class ToggleSwitch : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] targets;

    private bool isOn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        isOn = !isOn;

        foreach (MonoBehaviour t in targets)
        {
            if (t == null) continue;

            if (isOn)
                t.SendMessage("Activate", SendMessageOptions.DontRequireReceiver);
            else
                t.SendMessage("Deactivate", SendMessageOptions.DontRequireReceiver);
        }
    }
}