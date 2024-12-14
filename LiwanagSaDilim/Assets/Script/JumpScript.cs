using UnityEngine;

public class JumpScript : MonoBehaviour
{
    public AudioSource audioSource; // Reference to the AudioSource
    public AudioClip jumpClip;      // Reference to the jump sound

    // This method is called by the Animation Event
    public void PlayJumpSound()
    {
        if (audioSource != null && jumpClip != null)
        {
            audioSource.PlayOneShot(jumpClip);
        }
        else
        {
            Debug.LogWarning("AudioSource or JumpClip is missing!");
        }
    }
}
