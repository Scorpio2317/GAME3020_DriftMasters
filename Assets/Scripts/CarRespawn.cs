using UnityEngine;
using UnityEngine.InputSystem;

public class CarRespawn : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Save Point Settings")]
    public float saveInterval = 0.5f;
    public float uprightDotThreshold = 0.3f;

    [Header("Auto Respawn (when flipped)")]
    public float autoFlipDelay = 2f;

    [Header("Keyboard")]
    public KeyCode keyboardRespawnKey = KeyCode.R;

    private Vector3 lastSafePosition;
    private Quaternion lastSafeRotation;

    private float saveTimer = 0f;
    private float flippedTimer = 0f;

    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();

        lastSafePosition = transform.position;
        lastSafeRotation = transform.rotation;
    }

    private void Update()
    {
        UpdateSafePoint();
        CheckManualRespawn();
        CheckAutoRespawn();
    }

    private void UpdateSafePoint()
    {
        saveTimer += Time.deltaTime;

        float dot = Vector3.Dot(transform.up, Vector3.up);

        if (saveTimer >= saveInterval && dot > uprightDotThreshold)
        {
            lastSafePosition = transform.position;
            lastSafeRotation = transform.rotation;
            saveTimer = 0f;
        }
    }

    private void CheckManualRespawn()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Respawn();
        }
    }

    private void CheckAutoRespawn()
    {
        float dot = Vector3.Dot(transform.up, Vector3.up);

        if (dot < -uprightDotThreshold)
        {
            flippedTimer += Time.deltaTime;
        }
        else
        {
            flippedTimer = 0f;
        }

        if (flippedTimer >= autoFlipDelay)
        {
            Respawn();
            flippedTimer = 0f;
        }
    }

    private void Respawn()
    {
        if (!rb) return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetPositionAndRotation(lastSafePosition, lastSafeRotation);
    }
}
