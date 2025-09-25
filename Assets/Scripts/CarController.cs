using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

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

public class CarController : MonoBehaviour
{
    public Wheel[] wheels;
    public Vector2 moveInput;
    public float powerMultiplier = 1;
    public float maxSteer = 30, wheelBase = 2.5f, trackWidth = 1.5f;

    public void OnMove(InputValue value)
    {
        //print(value.Get<Vector2>());
        moveInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        foreach (var item in wheels)
        {
            item.collider.motorTorque = moveInput.y * powerMultiplier;
        }

        float steer = moveInput.x * maxSteer;
        if (moveInput.x > 0)
        {
            wheels[0].collider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (trackWidth / 2 + Mathf.Tan(Mathf.Deg2Rad * steer) * wheelBase));
            wheels[1].collider.steerAngle = steer;
        }
        else if (moveInput.x < 0)
        {
            wheels[0].collider.steerAngle = steer;
            wheels[1].collider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (-trackWidth / 2 + Mathf.Tan(Mathf.Deg2Rad * steer) * wheelBase));
        }
        else
        {
            wheels[0].collider.steerAngle = wheels[1].collider.steerAngle = 0;
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
