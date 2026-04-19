using UnityEngine;

public class PlatformRise : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void Activate()
    {
        anim.SetBool("Active", true);
    }

    public void Deactivate()
    {
        anim.SetBool("Active", false);
    }
}