using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountingFragments : MonoBehaviour
{

    [SerializeField] LevelHandler levelHandler;

    [Range(1, 3)]
    [SerializeField] int fragmentsCountAssigned = 1;

    private void OnEnable()
    {
        if(levelHandler.fragmentsCollected >= fragmentsCountAssigned)
        {
            StartCoroutine(fragmentsPop());
        }
        
    }

    IEnumerator fragmentsPop()
    {
        yield return new WaitForSecondsRealtime(0.2f * fragmentsCountAssigned);
        GetComponent<Animator>().Play("FragmentsPop");
    }
}
