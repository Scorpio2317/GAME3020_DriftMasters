using UnityEngine;
using UnityEngine.InputSystem;


public class CarController : MonoBehaviour
{
    public CarStats stats;
    public Rigidbody rb;
    public Wheel[] wheels;
    public float maxSteer = 30, wheelBase = 2.5f, trackWidth = 1.5f;
    public float steeringModifier = 1;

    #region common

    private Vector2 moveInput;
    public float wheelTurnLerpSpeed = 1;
    private bool isSpacebarPressed;

    #endregion

    [Range(0.05f, 1f)] public float steerReducingMultiplier = 0.3f;

    void Start()
    {
        stats = GetComponent<CarStats>();
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue value)
    {
        //print(value.Get<Vector2>());
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        isSpacebarPressed = value.Get<float>() == 1; // this will set the spacebar pressed bool to true when input is 1
    }

    void FixedUpdate()
    {

        if (!stats)
            return;

        isSpacebarPressed = Input.GetKey(KeyCode.Space); // this will set the spacebar bool to true depending on keypress space
        HandleSteering();
        TranslatePowertoWheels();

        for (int i = 0; i < wheels.Length; i++)
        {
            Quaternion quaternion;
            Vector3 Pos;
            wheels[i].collider.GetWorldPose(out Pos, out quaternion);
            //wheels[i].collider.transform.rotation = quaternion;

            Transform[] childTransforms = new Transform[wheels[i].collider.transform.childCount];
            int index = 0;
            foreach (var item in childTransforms)
            {
                wheels[i].collider.transform.GetChild(index).position = Pos;
                wheels[i].collider.transform.GetChild(index).rotation = quaternion;
                index++;
            }
            //wheels[i].collider.transform.position = Pos;
            //wheels[i].collider.transform.localRotation = Quaternion.Euler(0, wheels[i].collider.steerAngle, 0);
        }
    }

    void HandleSteering ()
    {
        maxSteer = stats.MaxSteerAngle + Mathf.Clamp(steeringModifier, 0, 10);

        if(moveInput.x > 0)
        {
            wheels[0].collider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (maxSteer - (trackWidth / 2))) * moveInput.x;
            wheels[1].collider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (maxSteer + (trackWidth / 2))) * moveInput.x;
        }
        else if (moveInput.x < 0)
        {
            wheels[0].collider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (maxSteer + (trackWidth / 2))) * moveInput.x;
            wheels[1].collider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (maxSteer - (trackWidth / 2))) * moveInput.x;
        }
        else
        {
            wheels[0].collider.steerAngle = 0;
            wheels[1].collider.steerAngle = 0;
        }

        steeringModifier = rb.linearVelocity.magnitude * steerReducingMultiplier;

    }

    public void setSpacebarPressed(bool _moveInput)
    {
        isSpacebarPressed = _moveInput;
    }

    public void TranslatePowertoWheels()
    {

        switch (stats.driveMode)
        {
            case driveMode.frontWheelDrive:
                wheels[0].collider.motorTorque = moveInput.y * stats.MaxPowerNM;
                wheels[1].collider.motorTorque = moveInput.y * stats.MaxPowerNM;
                break;
            case driveMode.rearWheelDrive:
                wheels[2].collider.motorTorque = isSpacebarPressed ? 0 : moveInput.y * stats.MaxPowerNM;
                wheels[3].collider.motorTorque = isSpacebarPressed ? 0 : moveInput.y * stats.MaxPowerNM;
                break;
            case driveMode.allWheelDrive:
                for (int i = 0; i < wheels.Length; i++)
                {
                    if (i > 1)
                    {
                        // rear wheel
                        wheels[i].collider.motorTorque = isSpacebarPressed ? 0 : moveInput.y * stats.MaxPowerNM;
                        //wheels[i].collider.brakeTorque = !isSpacebarPressed ? 0 : 1000;
                    }
                    else
                    {
                        wheels[i].collider.motorTorque = moveInput.y * stats.MaxPowerNM;

                        //fron wheels for now
                    }
                }
                break;
        }

        wheels[2].collider.brakeTorque = !isSpacebarPressed ? 0 : 1000;
        wheels[3].collider.brakeTorque = !isSpacebarPressed ? 0 : 1000;


    }
}


[System.Serializable]
public class Wheel
{
    public WheelCollider collider;
    public WheelType wheelType;
}


[System.Serializable]
public enum WheelType
{
    front, rear
}
