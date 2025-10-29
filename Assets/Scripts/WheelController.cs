using UnityEngine;
using UnityEngine.InputSystem;

public class WheelController : MonoBehaviour {

    [Header("Steer & Drive")]
    public WheelAlignment[] steerWheels;
    public WheelAlignment[] driveWheels;

    [Header("Steer")]
    public float wheelRotateSpeed = 8f;
    public float wheelSteeringAngle = 25f;
    public int steerSign = 1;

    [Header("Drive")]
    public float wheelAcceleration = 8f;
    public float wheelMaxTorque = 1500f;
    public int forwardTorqueSign = 1;

    [Header("Brake/Reverse")]
    public float BreakPower = 1.0f;
    public float brakeDamping = 0.3f;

    [Header("Input")]
    [Range(0f, 0.5f)] public float deadzone = 0.12f;

    public Rigidbody RB;

    private Vector2 moveInput;

    public void OnMove(UnityEngine.InputSystem.InputValue input)
    {
        Vector2 raw = input.Get<Vector2>();
        if (Mathf.Abs(raw.x) < deadzone) raw.x = 0f;
        if (Mathf.Abs(raw.y) < deadzone) raw.y = 0f;
        moveInput = raw;
    }

    void Update() => WheelControl();

    void WheelControl()
    {
        float steerX = moveInput.x;
        float gasY = moveInput.y;

        float targetSteerDeg = steerSign * wheelSteeringAngle * steerX;
        foreach (var wa in steerWheels)
        {
            wa.steeringAngle = Mathf.LerpAngle(wa.steeringAngle, targetSteerDeg, Time.deltaTime * wheelRotateSpeed);
        }

        float targetTorque = 0f;
        float brake = 0f;

        if (gasY > 0f)
        {
            targetTorque = forwardTorqueSign * wheelMaxTorque * gasY;
            brake = 0f;
            if (RB) RB.linearDamping = 0f;
        }
        else if (gasY < 0f)
        {
            targetTorque = 0f;
            brake = Mathf.Abs(gasY) * wheelMaxTorque * BreakPower;
            if (RB) RB.linearDamping = brakeDamping;
        }
        else
        {
            brake = 0f;
            if (RB) RB.linearDamping = 0f;
        }

        foreach (var wa in driveWheels)
        {
            var col = wa.wheelCol;

            col.brakeTorque = brake;

            float curr = col.motorTorque;
            float to = (Mathf.Approximately(gasY, 0f)) ? 0f : targetTorque;
            float step = wheelMaxTorque * Time.deltaTime * wheelAcceleration;

            col.motorTorque = Mathf.MoveTowards(curr, to, step);
        }
    }

}
