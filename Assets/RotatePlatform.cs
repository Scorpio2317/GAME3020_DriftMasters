using UnityEngine;

public class RotatePlatform : MonoBehaviour
{
    public float rotationSpeed = 30f;

    void Update()
    {
        // Rotate around Y axis forever
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
