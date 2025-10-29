using System.Collections.Generic;
using UnityEngine;

public class CarEffects : MonoBehaviour
{
    [Header("Skid Logic")]
    [Range(0f, 1f)]
    [SerializeField] private float skidSlipThreshold = 0.35f;

    [SerializeField] private float emitHoldSeconds = 0.25f;

    [Header("Handbrake")]
    [SerializeField] private KeyCode handbrakeKey = KeyCode.Space;

    [SerializeField] private float handbrakeBrakeTorque = 2000f;

    private struct WheelTrailState
    {
        public WheelCollider Collider;
        public TrailRenderer Trail;
        public float EmitUntilTime;
    }

    private readonly List<WheelTrailState> wheelTrails = new();

    private void Awake()
    {
        foreach (var wheel in GetComponentsInChildren<WheelCollider>(true))
        {
            var trail = wheel.GetComponentInChildren<TrailRenderer>(true);
            if (!trail) 
                continue;

            trail.emitting = false;
            wheelTrails.Add(new WheelTrailState
            {
                Collider = wheel,
                Trail = trail,
                EmitUntilTime = 0f
            });
        }
    }

    private void Update()
    {
        bool isHandbrakeDown = Input.GetKey(handbrakeKey);

        for (int i = 0; i < wheelTrails.Count; i++)
        {
            var state = wheelTrails[i];
            var wheel = state.Collider;
            var trail = state.Trail;

            // Read contact + slip
            bool isGrounded = wheel.GetGroundHit(out WheelHit hit);
            float slipMagnitude = isGrounded ? Mathf.Max(Mathf.Abs(hit.sidewaysSlip), Mathf.Abs(hit.forwardSlip)) : 0f;

            // Decide whether this wheel should emit a trail this frame
            bool shouldEmitNow = isGrounded && (slipMagnitude > skidSlipThreshold || isHandbrakeDown);

            if (shouldEmitNow)
            {
                if (!trail.emitting) trail.emitting = true;
                state.EmitUntilTime = Time.time + emitHoldSeconds; // extend hold
            }
            else if (trail.emitting && Time.time >= state.EmitUntilTime)
            {
                trail.emitting = false; // stop only after the hold expires
            }

            // Applied strong brake only to rear wheels
            if (isHandbrakeDown && wheel.transform.localPosition.z < 0f)
            {
                wheel.brakeTorque = handbrakeBrakeTorque;
            }
            else if (!isHandbrakeDown)
            {
                wheel.brakeTorque = 0f;
            }

            wheelTrails[i] = state; // write back
        }
    }
}
