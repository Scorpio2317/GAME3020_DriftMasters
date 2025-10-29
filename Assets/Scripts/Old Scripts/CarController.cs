using UnityEngine;
using UnityEngine.InputSystem;


public class CarController : MonoBehaviour
{
    private CarStateMachine stateMachine;

    public Rigidbody rb;
    public CarStats stats;
    public float maxSteer = 30, wheelBase = 2.5f, trackWidth = 1.5f;
    public float steeringModifier = 1;

    #region common

    //inputs 
    private Vector2 moveInput;
    private bool isSpacebarPressed;
    public float wheelTurnLerpSpeed = 1;


    #endregion


    [Header("debug")]
    [Range(0.05f, 1f)] public float steerReducingMultiplier = .3f; // use this to steer less when going quicker , prevent crashing

    void Start()
    {
        stateMachine = GetComponent<CarStateMachine>();
        stats = GetComponent<CarStats>();
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue input)
    {
        moveInput = input.Get<Vector2>();
    }

    public void OnJump(InputValue input)
    {

        isSpacebarPressed = input.Get<float>() == 1;

    }

    void FixedUpdate()
    {
        if (!stats) 
            return;


        isSpacebarPressed = Input.GetKey(KeyCode.Space);


        HandleSteering();
        TranslatePowertoWheels();
    }

    void HandleSteering()
    {

        maxSteer = stats.MaxSteerAngle + Mathf.Clamp(steeringModifier, 0, 10);

        if (moveInput.x > 0)
        {
            stateMachine.wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (maxSteer - (trackWidth / 2))) * moveInput.x;
            stateMachine.wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (maxSteer + (trackWidth / 2))) * moveInput.x;
        }
        else if (moveInput.x < 0)
        {
            stateMachine.wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (maxSteer + (trackWidth / 2))) * moveInput.x;
            stateMachine.wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (maxSteer - (trackWidth / 2))) * moveInput.x;
        }
        else
        {
            stateMachine.wheelColliders[0].steerAngle = 0;
            stateMachine.wheelColliders[1].steerAngle = 0;
        }
        steeringModifier = rb.linearVelocity.magnitude * steerReducingMultiplier;
    }

    // this is where we have setters
    public void SetMoveinput(Vector2 _moveInput)
    {
        moveInput = _moveInput;
    }

    public void SetSpacebarPressed(bool _moveInput)
    {
        isSpacebarPressed = _moveInput;
    }


    public void TranslatePowertoWheels()
    {

        switch (stats.driveMode)
        {
            case driveMode.frontWheelDrive:
                stateMachine.wheelColliders[0].motorTorque = moveInput.y * stats.MaxPowerNM;
                stateMachine.wheelColliders[1].motorTorque = moveInput.y * stats.MaxPowerNM;
                break;
            case driveMode.rearWheelDrive:
                stateMachine.wheelColliders[2].motorTorque = isSpacebarPressed ? 0 : moveInput.y * stats.MaxPowerNM;
                stateMachine.wheelColliders[3].motorTorque = isSpacebarPressed ? 0 : moveInput.y * stats.MaxPowerNM;
                break;
            case driveMode.allWheelDrive:
                for (int i = 0; i < stateMachine.wheelColliders.Length; i++)
                {
                    if (i > 1)
                    {
                        // rear wheel
                        stateMachine.wheelColliders[i].motorTorque = isSpacebarPressed ? 0 : moveInput.y * stats.MaxPowerNM;
                        //wheels[i].collider.brakeTorque = !isSpacebarPressed ? 0 : 1000;
                    }
                    else
                    {
                        stateMachine.wheelColliders[i].motorTorque = moveInput.y * stats.MaxPowerNM;
                        //fron wheels for now
                    }
                }
                break;
        }
        stateMachine.wheelColliders[2].brakeTorque = !isSpacebarPressed ? 0 : 1000;
        stateMachine.wheelColliders[3].brakeTorque = !isSpacebarPressed ? 0 : 1000;


    }
}


[System.Serializable]
public enum WheelType
{
    front, rear
}

[System.Serializable]
public enum Playertype
{
    player, npc
}
