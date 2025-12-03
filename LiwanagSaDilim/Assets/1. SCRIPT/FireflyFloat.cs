using UnityEngine;

public class FireflyRandomMove : MonoBehaviour
{
    public float movementRange = 1.0f;
    public float speed = 1.0f;
    private Vector3 startLocalPosition;
    private float randomOffset;

    void Start()
    {
        startLocalPosition = transform.localPosition;
        
        randomOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float noiseX = Mathf.PerlinNoise(Time.time * speed + randomOffset, 0); 
        float noiseY = Mathf.PerlinNoise(0, Time.time * speed + randomOffset);

        float xOffset = (noiseX - 0.5f) * movementRange;
        float yOffset = (noiseY - 0.5f) * movementRange;

        transform.localPosition = new Vector3(
            startLocalPosition.x + xOffset,
            startLocalPosition.y + yOffset,
            startLocalPosition.z
        );
    }
}