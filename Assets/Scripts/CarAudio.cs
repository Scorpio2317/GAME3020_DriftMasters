using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Engine")]
    public AudioSource engineSource;
    public float minEnginePitch = 0.8f;
    public float maxEnginePitch = 2.0f;
    public float pitchMaxSpeed = 60f;

    private void Awake()
    {
        if (!rb)
            rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateEngineSound();
    }

    private void UpdateEngineSound()
    {
        if (engineSource == null || rb == null)
            return;

        float speed = rb.linearVelocity.magnitude;
        float t = Mathf.Clamp01(speed / pitchMaxSpeed);

        engineSource.pitch = Mathf.Lerp(minEnginePitch, maxEnginePitch, t);
        engineSource.volume = Mathf.Lerp(0.25f, 1f, t);
    }
}
