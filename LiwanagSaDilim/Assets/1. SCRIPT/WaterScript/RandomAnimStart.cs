using UnityEngine;

public class RandomAnimStart : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        
        if (anim != null)
        {
            // 1. Ibahin nang kaunti ang bilis (halimbawa: 80% hanggang 120% ng original speed)
            // Para kahit tumagal ang laro, hindi sila magsasabay ulit.
            anim.speed = Random.Range(0.7f, 1.2f);

            // 2. Utusan ang animation na magsimula sa random na part (0.0 hanggang 1.0)
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
            anim.Play(state.fullPathHash, -1, Random.Range(0f, 5f));
        }
    }
}