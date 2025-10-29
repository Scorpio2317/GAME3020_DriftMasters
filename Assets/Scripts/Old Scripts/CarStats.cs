using System.Collections.Generic;
using UnityEngine;

public class CarStats : MonoBehaviour
{

    [Range(120, 2500)] public int MaxPowerNM = 1200;
    [Range(0, 5)] public float MaxSteerAngle = 4;
    public float steerDampSpeed = 8f;
    public driveMode driveMode = driveMode.allWheelDrive;
}

[System.Serializable]
public enum driveMode
{
    frontWheelDrive,
    rearWheelDrive,
    allWheelDrive
}
