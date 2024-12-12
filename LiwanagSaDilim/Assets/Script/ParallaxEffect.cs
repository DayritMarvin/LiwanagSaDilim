using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float ParallaxEff;
    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = cam.transform.position.x * ParallaxEff; // 0 = move with camera , 1 = won`t move , 0.5 half
        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
