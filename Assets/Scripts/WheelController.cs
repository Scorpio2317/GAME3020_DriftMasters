﻿using UnityEngine;
using UnityEngine.InputSystem;

public class WheelController : MonoBehaviour {

    [Header("Steer & Drive Lists")]
    public WheelAlignment[] steerWheels;
    public WheelAlignment[] driveWheels;

    [Header("Steer")]
    public float wheelSteeringAngle = 25f;
    public float wheelRotateSpeed = 8f;
    public int steerSign = 1;

    [Header("Drive")]
    public float wheelAcceleration = 8f;
    public float wheelMaxTorque = 1500f;
    public int forwardTorqueSign = 1;

    [Header("Brake/Reverse")]
    public float BreakPower = 1.0f;
    public float brakeDamping = 0.3f;

    [Header("Handbrake")]
    public float handbrakeBrakeTorque = 800f;

    [Header("Input")]
    [Range(0f, 0.5f)] public float deadzone = 0.12f;
    private bool isSpacebarPressed;

    [Header("References")]
    public Rigidbody rb;

    private Vector2 moveInput;
    private bool handbrake;

    public void OnMove(InputValue input)
    {
        Vector2 raw = input.Get<Vector2>();
        
        if (Mathf.Abs(raw.x) < deadzone)
            raw.x = 0f;
        if (Mathf.Abs(raw.y) < deadzone)
            raw.y = 0f;

        moveInput = raw;
    }
    void Update()
    {
        if (Keyboard.current != null)
            isSpacebarPressed = Keyboard.current.spaceKey.isPressed;
    }

    void FixedUpdate()
    {
        WheelControl();
    }

    void WheelControl()
    {
        float steerX = moveInput.x;
        float gasY = moveInput.y;

        float targetSteerDeg = steerSign * wheelSteeringAngle * steerX;
        for (int i = 0; i < steerWheels.Length; i++)
        {
            var wa = steerWheels[i];
            var col = wa.wheelCol;
            if (!col) continue;

            float newAngle = Mathf.LerpAngle(col.steerAngle, targetSteerDeg, Time.fixedDeltaTime * wheelRotateSpeed);
            col.steerAngle = newAngle;
            wa.steeringAngle = newAngle;
        }

        float targetTorque = 0f;
        float brake = 0f;

        if (gasY > 0f)
        {
            targetTorque = forwardTorqueSign * wheelMaxTorque * gasY;
            brake = 0f;
            if (rb) rb.linearDamping = 0f;
        }
        else if (gasY < 0f)
        {
            targetTorque = -forwardTorqueSign * wheelMaxTorque * Mathf.Abs(gasY) * BreakPower;
            brake = 0f;
            if (rb) rb.linearDamping = brakeDamping;
        }
        else
        {
            brake = 0f;
            if (rb) rb.linearDamping = 0f;
        }

        float step = wheelMaxTorque * Time.fixedDeltaTime * wheelAcceleration;
        float hb = isSpacebarPressed ? handbrakeBrakeTorque : 0f;

        for (int i = 0; i < driveWheels.Length; i++)
        {
            var wa = driveWheels[i];
            var col = wa.wheelCol;
            if (!col) continue;

            col.brakeTorque = brake + hb;

            float current = col.motorTorque;
            float to = (Mathf.Approximately(gasY, 0f)) ? 0f : targetTorque;
            col.motorTorque = Mathf.MoveTowards(current, to, step);
        }
    }
}