using UnityEngine;

public class AutoWakeUp : MonoBehaviour
{
    [Header("Ilagay dito ang object na naka-hide")]
    public GameObject targetObject;

    // Ang Awake ang pinakaunang umaandar pag-Play sa Unity
    void Awake()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true); // Gigisingin niya ang FadeCanvas!
        }
    }
}