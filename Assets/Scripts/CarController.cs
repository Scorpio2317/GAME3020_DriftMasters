using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;


public class CarController : MonoBehaviour
{
    public CarStats carStats;

    public Wheel[] wheels;
    public float maxSteer = 30, wheelBase = 2.5f, trackWidth = 1.5f;

    #region common

    private Vector2 moveInput;
    public float wheelTurnLerpSpeed = 1;
    public bool SpacebarPressed;
    #endregion

    void Start()
    {
        carStats = transform.GetComponent<CarStats>();
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
        SpacebarPressed = Input.GetKey(KeyCode.Space); // this will set the spacebar bool to true depending on keypress space


        if (!carStats)
            return;

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


        float steer = moveInput.x * maxSteer;
        if (moveInput.x > 0)
        {
            wheels[0].collider.steerAngle = Mathf.Lerp(wheels[0].collider.steerAngle, Mathf.Rad2Deg * Mathf.Atan(wheelBase / (trackWidth / 2 + Mathf.Tan(Mathf.Deg2Rad * steer) * wheelBase)), Time.deltaTime * wheelTurnLerpSpeed);
            wheels[1].collider.steerAngle = Mathf.Lerp(wheels[1].collider.steerAngle, steer, Time.deltaTime * wheelTurnLerpSpeed);
        }
        else if (moveInput.x < 0)
        {
            wheels[0].collider.steerAngle = Mathf.Lerp(wheels[0].collider.steerAngle, steer, Time.deltaTime * wheelTurnLerpSpeed);
            wheels[1].collider.steerAngle = Mathf.Lerp(wheels[1].collider.steerAngle, Mathf.Rad2Deg * Mathf.Atan(wheelBase / (-trackWidth / 2 + Mathf.Tan(Mathf.Deg2Rad * steer) * wheelBase)), Time.deltaTime * wheelTurnLerpSpeed);
        }
        else
        {
            wheels[0].collider.steerAngle = wheels[1].collider.steerAngle = Mathf.Lerp(wheels[0].collider.steerAngle = wheels[1].collider.steerAngle, 0, Time.deltaTime * wheelTurnLerpSpeed);
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
