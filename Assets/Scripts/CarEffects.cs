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

    private struct WheelFx
    {
        public WheelCollider collider;
        public TrailRenderer trail;
        public ParticleSystem smoke;
        public float emitUntilTime;
    }

    private readonly List<WheelFx> wheels = new List<WheelFx>();

    private void Awake()
    {
        foreach (var col in GetComponentsInChildren<WheelCollider>(true))
        {
            var trail = col.GetComponentInChildren<TrailRenderer>(true);
            var smoke = col.GetComponentInChildren<ParticleSystem>(true);

            if (trail == null && smoke == null)
                continue;

            if (trail != null)
                trail.emitting = false;

            if (smoke != null)
                smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            wheels.Add(new WheelFx
            {
                collider = col,
                trail = trail,
                smoke = smoke,
                emitUntilTime = 0f
            });
        }
    }

    private void Update()
    {
        bool isHandbrakeDown = Input.GetKey(handbrakeKey);

        for (int i = 0; i < wheels.Count; i++)
        {
            var w = wheels[i];
            var col = w.collider;

            bool isGrounded = col.GetGroundHit(out WheelHit hit);
            float slipMagnitude = isGrounded ? Mathf.Max(Mathf.Abs(hit.sidewaysSlip), Mathf.Abs(hit.forwardSlip)) : 0f;

            bool shouldEmitNow = isGrounded && (slipMagnitude > skidSlipThreshold || isHandbrakeDown);

            if (shouldEmitNow)
            {
                w.emitUntilTime = Time.time + emitHoldSeconds;

                // Trails
                if (w.trail != null && !w.trail.emitting)
                    w.trail.emitting = true;

                // Smoke
                if (w.smoke != null && !w.smoke.isPlaying)
                    w.smoke.Play();
            }
            else
            {
                if (Time.time >= w.emitUntilTime)
                {
                    if (w.trail != null && w.trail.emitting)
                        w.trail.emitting = false;

                    if (w.smoke != null && w.smoke.isPlaying)
                        w.smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
            if (isHandbrakeDown && col.transform.localPosition.z < 0f)
            {
                col.brakeTorque = handbrakeBrakeTorque;
            }
            else if (!isHandbrakeDown)
            {
                col.brakeTorque = 0f;
            }

            wheels[i] = w;
        }
    }
}
