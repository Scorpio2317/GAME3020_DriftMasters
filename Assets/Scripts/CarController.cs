using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;


public class CarController : MonoBehaviour
{
    public CarStats carStats;
    public Rigidbody rb;
    public Wheel[] wheels;
    public float maxSteer = 30, wheelBase = 2.5f, trackWidth = 1.5f;
    public float steeringModifier = 1;

    #region common

    private Vector2 moveInput;
    public float wheelTurnLerpSpeed = 1;
    public bool SpacebarPressed;

    #endregion

    [Range(0.05f, 1f)] public float steerReducingMultiplier = 0.3f;

    void Start()
    {
        carStats = GetComponent<CarStats>();
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue value)
    {
        //print(value.Get<Vector2>());
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        SpacebarPressed = value.Get<float>() == 1; // this will set the spacebar pressed bool to true when input is 1
    }

    void FixedUpdate()
    {

        if (!carStats)
            return;

        SpacebarPressed = Input.GetKey(KeyCode.Space); // this will set the spacebar bool to true depending on keypress space
        HandleSteering();

        for (int i = 0; i < wheels.Length; i++)
        {
            if (i > 1)
            {
                //rear wheels
                wheels[i].collider.motorTorque = SpacebarPressed ? 0 : moveInput.y * carStats.MaxPowerNM;
                wheels[i].collider.brakeTorque = !SpacebarPressed ? 0 : 1000;
            } else
            {
                //front wheels
                wheels[i].collider.motorTorque = moveInput.y * carStats.MaxPowerNM;
            }
        }

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
        maxSteer = carStats.MaxSteerAngle + Mathf.Clamp(steeringModifier, 0, 10);

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
